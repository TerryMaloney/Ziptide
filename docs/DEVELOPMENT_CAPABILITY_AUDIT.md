# ZIPTIDE DEVELOPMENT CAPABILITY AUDIT

When Terry is back at the Windows computer, run:

```powershell
.\tools\check_dev_capabilities.ps1
```

This is read-only. It checks and reports:

- ZIPTIDE checkout and Unity project version;
- Git, active branch, and whether the working tree is clean;
- GitHub CLI installation and authentication;
- Python 3;
- Unity Editor discovery and whether 2022.3.62f3 is present;
- Java;
- Android SDK/ADB;
- whether exactly one authorized Quest/device is connected;
- availability of the ZIPTIDE fast-preflight wrapper.

The report is written to:

```text
Builds/Reports/dev_capabilities.json
```

Device serials and authentication secrets are not written to the report. The Quest result records
only counts/state: ready, authorization pending, offline, multiple devices, or no connected device.

Useful options:

```powershell
# Print JSON as well as writing the report
.\tools\check_dev_capabilities.ps1 -Json

# Return a non-zero exit when a required capability is missing
.\tools\check_dev_capabilities.ps1 -Strict
```

After the audit is READY, the intended local loop is:

```text
edit
→ .\tools\dev_preflight.ps1
→ targeted local Unity batchmode test
→ push and confirm CI
→ build/install exact intended APK
→ headset route
→ save checkpoint/logcat evidence
```

This audit does not install or repair tools automatically. It gives one grounded report so an
operator fixes only the missing capability instead of guessing through setup.
