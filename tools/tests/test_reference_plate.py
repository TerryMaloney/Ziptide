from __future__ import annotations

import json
import struct
import sys
import tempfile
import unittest
import zlib
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import reference_plate


def _chunk(kind: bytes, payload: bytes) -> bytes:
    return (
        struct.pack(">I", len(payload))
        + kind
        + payload
        + struct.pack(">I", zlib.crc32(kind + payload) & 0xFFFFFFFF)
    )


def make_png(width: int, height: int) -> bytes:
    signature = reference_plate.PNG_SIGNATURE
    ihdr = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    row = b"\x00" + b"\x20\x40\x60\xff" * width
    pixels = row * height
    return signature + _chunk(b"IHDR", ihdr) + _chunk(b"IDAT", zlib.compress(pixels)) + _chunk(b"IEND", b"")


class ReferencePlateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.reference = self.root / "keeper.png"
        self.render = self.root / "render.png"
        self.reference.write_bytes(make_png(400, 200))
        self.render.write_bytes(make_png(200, 400))

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def test_png_dimensions_and_hash_are_read(self) -> None:
        data, info = reference_plate.read_png(self.reference)
        self.assertEqual(400, info.width)
        self.assertEqual(200, info.height)
        self.assertEqual(len(data), info.size_bytes)
        self.assertEqual(64, len(info.sha256))

    def test_svg_is_deterministic_and_embeds_both_inputs(self) -> None:
        ref_bytes, ref_info = reference_plate.read_png(self.reference)
        render_bytes, render_info = reference_plate.read_png(self.render)
        first, first_meta = reference_plate.compose_svg(
            reference_bytes=ref_bytes,
            reference_info=ref_info,
            render_bytes=render_bytes,
            render_info=render_info,
        )
        second, second_meta = reference_plate.compose_svg(
            reference_bytes=ref_bytes,
            reference_info=ref_info,
            render_bytes=render_bytes,
            render_info=render_info,
        )
        self.assertEqual(first, second)
        self.assertEqual(first_meta, second_meta)
        self.assertEqual(2, first.count("data:image/png;base64,"))
        self.assertIn("KEEPER CONCEPT", first)
        self.assertIn("BOOTH RENDER", first)

    def test_labels_are_xml_escaped(self) -> None:
        ref_bytes, ref_info = reference_plate.read_png(self.reference)
        render_bytes, render_info = reference_plate.read_png(self.render)
        svg, _ = reference_plate.compose_svg(
            reference_bytes=ref_bytes,
            reference_info=ref_info,
            render_bytes=render_bytes,
            render_info=render_info,
            reference_label='Keeper <A> & "B"',
            render_label="Render > Final",
        )
        self.assertIn("Keeper &lt;A&gt; &amp; &quot;B&quot;", svg)
        self.assertIn("Render &gt; Final", svg)
        self.assertNotIn("Keeper <A>", svg)

    def test_write_plate_outputs_svg_and_metadata(self) -> None:
        output = self.root / "plate.svg"
        metadata_path = self.root / "plate.json"
        metadata = reference_plate.write_plate(
            self.reference,
            self.render,
            output,
            metadata_path=metadata_path,
        )
        self.assertTrue(output.is_file())
        self.assertTrue(metadata_path.is_file())
        payload = json.loads(metadata_path.read_text(encoding="utf-8"))
        self.assertEqual(1, payload["schemaVersion"])
        self.assertEqual(output.as_posix(), payload["output"])
        self.assertEqual(metadata.canvas_width, payload["canvasWidth"])
        self.assertEqual(400, payload["reference"]["width"])
        self.assertEqual(400, payload["render"]["height"])

    def test_invalid_png_is_rejected(self) -> None:
        bad = self.root / "bad.png"
        bad.write_bytes(b"not a png")
        with self.assertRaises(reference_plate.PlateError):
            reference_plate.read_png(bad)

    def test_non_positive_layout_values_are_rejected(self) -> None:
        ref_bytes, ref_info = reference_plate.read_png(self.reference)
        render_bytes, render_info = reference_plate.read_png(self.render)
        with self.assertRaises(reference_plate.PlateError):
            reference_plate.compose_svg(
                reference_bytes=ref_bytes,
                reference_info=ref_info,
                render_bytes=render_bytes,
                render_info=render_info,
                gutter=0,
            )


if __name__ == "__main__":
    unittest.main()
