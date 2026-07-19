# Operator note — corrected checkpoint branch

**Working branch:** `claude/recovery-quest-weapon-coupler-fix`  
**Target:** `terry-local-wip`  
**Scope:** only the defects demonstrated by Terry's 2026-07-19 Quest checkpoint.

Do not merge this branch based on source review alone. Promotion requires ordinary CI green first. After merge, wait for and inspect the unchanged Recovery PlayMode suite and the exact-SHA Golden Android artifact before authorizing Terry's retry.

The historical branch `recovery/quest-weapon-coupler-fix` stopped at the first source packet and is superseded by the `claude/**` branch above because ordinary CI listens to `claude/**` pushes.
