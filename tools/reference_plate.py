#!/usr/bin/env python3
"""Compose a standalone SVG concept-vs-render comparison plate.

This is the non-Unity core of Forge booth reference-plate mode. It accepts two
PNG files, embeds them into a deterministic side-by-side SVG, and optionally
writes machine-readable metadata. It uses only the Python standard library so
it can run in fast CI and on Terry's Windows checkout.
"""

from __future__ import annotations

import argparse
import base64
import hashlib
import json
import struct
import sys
from dataclasses import asdict, dataclass
from html import escape
from pathlib import Path
from typing import Sequence

PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"
EXIT_OK = 0
EXIT_ERROR = 1


@dataclass(frozen=True)
class PngInfo:
    path: str
    width: int
    height: int
    sha256: str
    size_bytes: int


@dataclass(frozen=True)
class PlateMetadata:
    schema_version: int
    reference: PngInfo
    render: PngInfo
    panel_width: int
    panel_height: int
    gutter: int
    label_height: int
    canvas_width: int
    canvas_height: int
    output: str

    def to_dict(self) -> dict[str, object]:
        payload = asdict(self)
        payload["schemaVersion"] = payload.pop("schema_version")
        payload["panelWidth"] = payload.pop("panel_width")
        payload["panelHeight"] = payload.pop("panel_height")
        payload["labelHeight"] = payload.pop("label_height")
        payload["canvasWidth"] = payload.pop("canvas_width")
        payload["canvasHeight"] = payload.pop("canvas_height")
        return payload


class PlateError(RuntimeError):
    pass


def read_png(path: Path) -> tuple[bytes, PngInfo]:
    try:
        data = path.read_bytes()
    except OSError as exc:
        raise PlateError(f"Could not read {path}: {exc}") from exc
    if len(data) < 24 or not data.startswith(PNG_SIGNATURE):
        raise PlateError(f"Expected PNG input: {path}")
    if data[12:16] != b"IHDR":
        raise PlateError(f"PNG lacks an IHDR first chunk: {path}")
    width, height = struct.unpack(">II", data[16:24])
    if width <= 0 or height <= 0:
        raise PlateError(f"PNG dimensions must be positive: {path}")
    info = PngInfo(
        path=path.as_posix(),
        width=width,
        height=height,
        sha256=hashlib.sha256(data).hexdigest(),
        size_bytes=len(data),
    )
    return data, info


def _fit(width: int, height: int, box_width: int, box_height: int) -> tuple[float, float]:
    scale = min(box_width / width, box_height / height)
    return width * scale, height * scale


def compose_svg(
    *,
    reference_bytes: bytes,
    reference_info: PngInfo,
    render_bytes: bytes,
    render_info: PngInfo,
    reference_label: str = "KEEPER CONCEPT",
    render_label: str = "BOOTH RENDER",
    panel_width: int = 1024,
    panel_height: int = 1024,
    gutter: int = 48,
    label_height: int = 88,
) -> tuple[str, PlateMetadata]:
    for name, value in (
        ("panel_width", panel_width),
        ("panel_height", panel_height),
        ("gutter", gutter),
        ("label_height", label_height),
    ):
        if value <= 0:
            raise PlateError(f"{name} must be positive.")

    canvas_width = panel_width * 2 + gutter * 3
    canvas_height = panel_height + label_height + gutter * 2
    image_y = label_height + gutter
    left_x = gutter
    right_x = gutter * 2 + panel_width

    ref_width, ref_height = _fit(
        reference_info.width, reference_info.height, panel_width, panel_height
    )
    render_width, render_height = _fit(
        render_info.width, render_info.height, panel_width, panel_height
    )
    ref_x = left_x + (panel_width - ref_width) / 2
    ref_y = image_y + (panel_height - ref_height) / 2
    render_x = right_x + (panel_width - render_width) / 2
    render_y = image_y + (panel_height - render_height) / 2

    reference_uri = "data:image/png;base64," + base64.b64encode(reference_bytes).decode("ascii")
    render_uri = "data:image/png;base64," + base64.b64encode(render_bytes).decode("ascii")
    ref_label = escape(reference_label, quote=True)
    out_label = escape(render_label, quote=True)

    svg = f'''<svg xmlns="http://www.w3.org/2000/svg" width="{canvas_width}" height="{canvas_height}" viewBox="0 0 {canvas_width} {canvas_height}">
  <title>{ref_label} versus {out_label}</title>
  <rect width="100%" height="100%" fill="#17191d"/>
  <rect x="{left_x}" y="{image_y}" width="{panel_width}" height="{panel_height}" rx="16" fill="#262a31"/>
  <rect x="{right_x}" y="{image_y}" width="{panel_width}" height="{panel_height}" rx="16" fill="#262a31"/>
  <text x="{left_x + panel_width / 2}" y="{label_height}" text-anchor="middle" font-family="sans-serif" font-size="34" font-weight="700" fill="#f2f3f5">{ref_label}</text>
  <text x="{right_x + panel_width / 2}" y="{label_height}" text-anchor="middle" font-family="sans-serif" font-size="34" font-weight="700" fill="#f2f3f5">{out_label}</text>
  <image x="{ref_x:.3f}" y="{ref_y:.3f}" width="{ref_width:.3f}" height="{ref_height:.3f}" href="{reference_uri}" preserveAspectRatio="xMidYMid meet"/>
  <image x="{render_x:.3f}" y="{render_y:.3f}" width="{render_width:.3f}" height="{render_height:.3f}" href="{render_uri}" preserveAspectRatio="xMidYMid meet"/>
</svg>
'''

    metadata = PlateMetadata(
        schema_version=1,
        reference=reference_info,
        render=render_info,
        panel_width=panel_width,
        panel_height=panel_height,
        gutter=gutter,
        label_height=label_height,
        canvas_width=canvas_width,
        canvas_height=canvas_height,
        output="",
    )
    return svg, metadata


def write_plate(
    reference_path: Path,
    render_path: Path,
    output_path: Path,
    *,
    metadata_path: Path | None = None,
    reference_label: str = "KEEPER CONCEPT",
    render_label: str = "BOOTH RENDER",
) -> PlateMetadata:
    reference_bytes, reference_info = read_png(reference_path)
    render_bytes, render_info = read_png(render_path)
    svg, metadata = compose_svg(
        reference_bytes=reference_bytes,
        reference_info=reference_info,
        render_bytes=render_bytes,
        render_info=render_info,
        reference_label=reference_label,
        render_label=render_label,
    )
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(svg, encoding="utf-8", newline="\n")
    metadata = PlateMetadata(
        schema_version=metadata.schema_version,
        reference=metadata.reference,
        render=metadata.render,
        panel_width=metadata.panel_width,
        panel_height=metadata.panel_height,
        gutter=metadata.gutter,
        label_height=metadata.label_height,
        canvas_width=metadata.canvas_width,
        canvas_height=metadata.canvas_height,
        output=output_path.as_posix(),
    )
    if metadata_path:
        metadata_path.parent.mkdir(parents=True, exist_ok=True)
        metadata_path.write_text(
            json.dumps(metadata.to_dict(), indent=2, sort_keys=True) + "\n",
            encoding="utf-8",
            newline="\n",
        )
    return metadata


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--reference", type=Path, required=True)
    parser.add_argument("--render", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--metadata", type=Path)
    parser.add_argument("--reference-label", default="KEEPER CONCEPT")
    parser.add_argument("--render-label", default="BOOTH RENDER")
    args = parser.parse_args(argv)

    try:
        metadata = write_plate(
            args.reference,
            args.render,
            args.output,
            metadata_path=args.metadata,
            reference_label=args.reference_label,
            render_label=args.render_label,
        )
    except PlateError as exc:
        print(f"REFERENCE_PLATE_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR

    print(
        "REFERENCE_PLATE_WRITTEN "
        f"output={metadata.output} canvas={metadata.canvas_width}x{metadata.canvas_height} "
        f"reference={metadata.reference.width}x{metadata.reference.height} "
        f"render={metadata.render.width}x{metadata.render.height}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
