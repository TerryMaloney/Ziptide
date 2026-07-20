# ZIPTIDE Focused Recovery Reference Graph

- Scanned C# files: **722**
- Focused references: **396**

## melee

- References: **101**
- Files: **32**
- Non-declaration caller files: **32**

### `ArenaWeaponKind.BreakerBlade`

- **reference** · `Ziptide.Editor.Patching.ArenaWeaponAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ArenaWeaponAuthor.cs:27` — `made += Ensure("breaker_blade", ArenaWeaponKind.BreakerBlade, cooldown: 0.0f);`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:392` — `case ArenaWeaponKind.BreakerBlade:`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:453` — `case ArenaWeaponKind.BreakerBlade:`

### `ArenaWeaponKind.SonicThumper`

- **reference** · `Ziptide.Editor.Patching.ArenaWeaponAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ArenaWeaponAuthor.cs:24` — `made += Ensure("sonic_thumper", ArenaWeaponKind.SonicThumper, cooldown: 1.2f);`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:378` — `case ArenaWeaponKind.SonicThumper:`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:451` — `case ArenaWeaponKind.SonicThumper: go.AddComponent<SonicThumperRuntime>(); break;`

### `ArenaWeaponKind.TidePike`

- **reference** · `Ziptide.Editor.Patching.ArenaWeaponAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ArenaWeaponAuthor.cs:28` — `made += Ensure("tide_pike", ArenaWeaponKind.TidePike, cooldown: 0.0f);`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:399` — `case ArenaWeaponKind.TidePike:`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:454` — `case ArenaWeaponKind.TidePike: go.AddComponent<MeleeWeaponRuntime>(); break;`
- **reference** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:46` — `private bool IsPike => Def != null && Def.kind == ArenaWeaponKind.TidePike;`

### `HammerTool`

- **reference** · `Ziptide.Editor.Patching.ScenePatcherArena` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:241` — `go.AddComponent<HammerTool>();`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherPvP` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:131` — `// hammer can't be grabbed (this is why pickup failed — HammerTool added the collider too late`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherPvP` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:139` — `go.AddComponent<HammerTool>(); // RequireComponent adds XRGrabInteractable; collider already present`
- **declaration** · `Ziptide.Gameplay.HammerTool` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:13` — `public class HammerTool : MonoBehaviour`
- **reference** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:23` — `/// Hit detection reuses HammerTool/SonicThumper's proven tracked-point velocity idiom (transform`
- **reference** · `Ziptide.Gameplay.SonicThumperRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:13` — `/// Mirrors HammerTool's proven swing detection (head speed threshold + debounce).`

### `HitFromHammer`

- **reference** · `Ziptide.Gameplay.BreakableWall` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:48` — `public void HitFromHammer(Vector3 worldHitPoint)`
- **reference** · `Ziptide.Gameplay.BreakableWall` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:149` — `public void HitFromHammer() { HitFromHammer(transform.position); }`
- **reference** · `Ziptide.Gameplay.HammerTool` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:118` — `if (wall != null) { wall.HitFromHammer(_head.position); _lastBreakAt = Time.time; break; }`
- **reference** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:84` — `wall.HitFromHammer(h.ClosestPoint(tipPos));`
- **reference** · `Ziptide.Gameplay.SonicThumperRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:64` — `wall.HitFromHammer(h.ClosestPoint(transform.position));`

### `MeleeWeaponRuntime`

- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:28` — `bool melee = item.GetComponent<MeleeWeaponRuntime>() != null;`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:454` — `case ArenaWeaponKind.TidePike: go.AddComponent<MeleeWeaponRuntime>(); break;`
- **reference** · `Ziptide.Gameplay.GunLaserSight` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:10` — `/// MeleeWeaponRuntime is present instead of painting a misleading laser down a sword or pike.`
- **reference** · `Ziptide.Gameplay.GunLaserSight` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:41` — `if (GetComponent<MeleeWeaponRuntime>() == null) return false;`
- **declaration** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:28` — `public class MeleeWeaponRuntime : MonoBehaviour`

### `Muzzle`

- **reference** · `Ziptide.Content.ItemDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/Items/ItemDefinition.cs:43` — `[Tooltip("Muzzle point (local). Zero = factory default. Bolts/rays originate here.")]`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:20` — `public const string MuzzleDistance = "WEAPON_MUZZLE_DISTANCE_INVALID";`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:29` — `bool hasMuzzle = item.transform.Find("Muzzle") != null;`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:30` — `bool knownWeapon = melee || hasMuzzle`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:69` — `Transform muzzle = weapon.transform.Find("Muzzle");`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:72` — `Vector3 gripToMuzzle = muzzle.position - grip.position;`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:73` — `float distance = gripToMuzzle.magnitude;`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:81` — `report.Blocker(MuzzleDistance,`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:82` — `path + " Grip→Muzzle distance is " + distance.ToString("F3")`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:88` — `float forwardDot = Vector3.Dot(grip.forward.normalized, gripToMuzzle.normalized);`
- **reference** · `Ziptide.Editor.Audit.WeaponPerceptualAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/WeaponPerceptualAuditRules.cs:91` — `path + " Grip forward and Grip→Muzzle direction dot="`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:730` — `name = "MuzzleRing", op = ForgeOp.Tube, segments = 10, wallThickness = 0.007f,`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:775` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.012f, 0.125f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:914` — `new ForgePart { name = "MuzzleRing", op = ForgeOp.Tube, segments = 10, wallThickness = 0.004f,`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:933` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.045f, 0.125f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:991` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.01f, 0.14f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:1048` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.175f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:1104` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.005f, 0.29f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:1179` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.45f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:1187` — `/// ~74 cm (the long one). Grip in the rear third; Muzzle at the point.`
- **reference** · `Ziptide.Editor.Patching.ForgeRecipeLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:1239` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.59f), localEuler = Vector3.zero },`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherC0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:248` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD1` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:691` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:49` — `// Every gun (anything with a Muzzle + grab) gets the aim line (CONTROL_SCHEME "Aim").`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:50` — `if (built != null && built.transform.Find("Muzzle") != null`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:208` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:251` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:260` — `/// forward "Lens" child (NOT "Muzzle", so no laser sight is attached) and a level grip (you`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:341` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:445` — `var muzzleGo = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.PvpComfortHop` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpComfortHop.cs:23` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.GravityGunRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:36` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.GravityGunRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:39` — `var m = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.GunLaserSight` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:7` — `/// The aim line (CONTROL_SCHEME.md "Aim"): a thin ray from the Muzzle to the first hit,`
- **reference** · `Ziptide.Gameplay.GunLaserSight` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:9` — `/// also expose a child named Muzzle as their physical tip, so this component disables itself when`
- **reference** · `Ziptide.Gameplay.GunLaserSight` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:49` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:31` — `private Transform _tip; // the business end (child "Muzzle" from ItemFactory)`
- **reference** · `Ziptide.Gameplay.MeleeWeaponRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:51` — `_tip = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.PistolRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:10` — `/// Hitscan pistol: raycast from Muzzle on trigger, hit TargetRuntime, tracer/muzzle/impact feedback,`
- **reference** · `Ziptide.Gameplay.PistolRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:41` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.PistolRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:44` — `var m = new GameObject("Muzzle");`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:37` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:93` — `Vector3 origin = MuzzlePos();`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:94` — `Vector3 dir = MuzzleDir();`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:114` — `Vector3 origin = MuzzlePos();`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:115` — `Vector3 dir = MuzzleDir();`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:148` — `private Vector3 MuzzlePos() =>`
- **reference** · `Ziptide.Gameplay.PrismBeamRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:151` — `private Vector3 MuzzleDir() =>`
- **reference** · `Ziptide.Gameplay.StaticNetGunRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:32` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.TaserDartGunRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs:43` — `_muzzle = transform.Find("Muzzle");`
- **reference** · `Ziptide.Gameplay.TaserDartGunRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs:46` — `var m = new GameObject("Muzzle");`
- **reference** · `Ziptide.Tests.EditMode.ForgeLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeLifecycleTests.cs:25` — `new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.2f) } };`
- **reference** · `Ziptide.Tests.EditMode.ForgeMeshTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeMeshTests.cs:187` — `new ForgeSocket { name = "Muzzle" }`
- **reference** · `Ziptide.Tests.EditMode.ForgeMeshTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeMeshTests.cs:217` — `Assert.IsNotEmpty(r.Validate(), "Grip without Muzzle");`
- **reference** · `Ziptide.Tests.EditMode.ForgeRecipeLibraryTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeRecipeLibraryTests.cs:11` — `/// handhelds) honors the Grip/Muzzle socket contract with the Quest grip tilt.`
- **reference** · `Ziptide.Tests.EditMode.ForgeRecipeLibraryTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeRecipeLibraryTests.cs:77` — `if (s.name == "Muzzle") muzzle = s;`
- **reference** · `Ziptide.Tests.EditMode.ForgeRecipeLibraryTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeRecipeLibraryTests.cs:80` — `Assert.IsNotNull(muzzle, kv.Key + " handheld without a Muzzle socket");`
- **reference** · `Ziptide.Tests.EditMode.QuestWeaponAndCouplerRegressionTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/QuestWeaponAndCouplerRegressionTests.cs:78` — `itemId + " uses Muzzle as a melee tip and must never display a gun laser.");`
- **reference** · `Ziptide.Tests.EditMode.VfxRecipeTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VfxRecipeTests.cs:94` — `foreach (var k in new[] { VfxKind.Impact, VfxKind.Muzzle, VfxKind.SteamVent, VfxKind.Motes, VfxKind.Sparks })`
- **reference** · `Ziptide.Tests.EditMode.WeaponPerceptualAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs:30` — `public void DeliberatelySidewaysMuzzle_FiresAimAxisBlocker()`
- **reference** · `Ziptide.Tests.EditMode.WeaponPerceptualAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs:105` — `Transform muzzle = new GameObject("Muzzle").transform;`
- **reference** · `Ziptide.Tests.PlayMode.RecoveryForgeVisualOwnershipTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:60` — `var muzzle = new GameObject("Muzzle");`
- **reference** · `Ziptide.Visuals.ForgeQualityState` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:80` — `/// <summary>Named attach point (Grip/Muzzle/Seat/Door/...). Consumers snap existing children here —`
- **reference** · `Ziptide.Visuals.ForgeQualityState` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:317` — `if (names.Contains("Grip") && !names.Contains("Muzzle"))`
- **reference** · `Ziptide.Visuals.ForgeQualityState` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:318` — `issues.Add("handheld contract: a Grip socket requires a Muzzle socket");`
- **reference** · `Ziptide.Visuals.ForgeVisualApplier` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs:93` — `// Snap existing socket-named children (Grip = XR attach, Muzzle = ray origin) to the`
- **reference** · `Ziptide.Visuals.VfxFactory` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:278` — `case VfxKind.Muzzle:`
- **reference** · `Ziptide.Visuals.VfxFactory` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:324` — `bool streak = kind == VfxKind.Muzzle || kind == VfxKind.Sparks;`
- **reference** · `Ziptide.Visuals.VfxFactory` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:430` — `case VfxKind.Muzzle:`
- **reference** · `Ziptide.Visuals.VfxLibrary` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxLibrary.cs:66` — `id = "muzzle_taser", kind = VfxKind.Muzzle, oneShot = true,`
- **reference** · `Ziptide.Visuals.VfxKind` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxRecipeDefinition.cs:18` — `Muzzle, // short flash at a weapon tip`
- **reference** · `Ziptide.Visuals.VfxKind` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxRecipeDefinition.cs:36` — `[Tooltip("One-shot burst size (Impact/Muzzle/Sparks/Drips). 0 = purely continuous.")]`
- **reference** · `Ziptide.Visuals.VfxKind` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxRecipeDefinition.cs:51` — `[Tooltip("Burst then auto-return to the pool (Impact/Muzzle/Sparks). false = looping (Motes/SteamVent).")]`

### `SonicThumperRuntime`

- **reference** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:451` — `case ArenaWeaponKind.SonicThumper: go.AddComponent<SonicThumperRuntime>(); break;`
- **declaration** · `Ziptide.Gameplay.SonicThumperRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:16` — `public class SonicThumperRuntime : MonoBehaviour`

## repairObjective

- References: **197**
- Files: **49**
- Non-declaration caller files: **48**

### `CastOffArming`

- **declaration** · `Ziptide.Gameplay.CastOffArming` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CastOffArming.cs:5` — `/// PRIORITIES #3 fragment). One law, pinned by CastOffArmingTests: the gate only ever blocks`
- **declaration** · `Ziptide.Gameplay.CastOffArming` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CastOffArming.cs:10` — `public static class CastOffArming`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:17` — `/// <see cref="CastOffArming"/> — a missing machine never strands the launch. Blocked presses flash the`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:152` — `// but never cache absence: a truly machine-less scene stays armed (CastOffArming law).`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:156` — `bool armed = CastOffArming.IsArmed(gateConfigured, _armingMachine != null,`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:11` — `public class CastOffArmingTests`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:16` — `Assert.IsFalse(CastOffArming.IsArmed(gateConfigured: true, machineFound: true, machineRepaired: false),`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:23` — `Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: true, machineFound: true, machineRepaired: true));`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:29` — `Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: true, machineFound: false, machineRepaired: false),`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:36` — `Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: false, machineFound: false, machineRepaired: false));`
- **reference** · `Ziptide.Tests.EditMode.CastOffArmingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CastOffArmingTests.cs:37` — `Assert.IsTrue(CastOffArming.IsArmed(gateConfigured: false, machineFound: true, machineRepaired: false));`

### `JobDirector`

- **reference** · `Ziptide.Content.JobRewards` · `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobRewards.cs:9` — `/// <c>JobDirector</c> calls this once when a job's last step finishes.`
- **reference** · `Ziptide.Content.WorldSpec` · `Ziptide/Assets/Ziptide/Content/Runtime/Spec/WorldSpec.cs:74` — `[Header("Pack contents (spawned by JobDirector at scene start)")]`
- **reference** · `Ziptide.Content.BuildSocketSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/BuildSocketSpawnDefinition.cs:8` — `/// JobDirector spawns a BuildSocketRuntime per entry at the MachineSite POI's socket plinth. Pay`
- **reference** · `Ziptide.Content.ChoiceSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/ChoiceSpawnDefinition.cs:7` — `/// A two-option story choice set-piece authored as PACK DATA (GAME_PLAN M1): JobDirector spawns a`
- **reference** · `Ziptide.Content.CollectibleSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/CollectibleSpawnDefinition.cs:7` — `/// A physical pickup authored as PACK DATA (GAME_PLAN M1): JobDirector spawns a grabbable`
- **reference** · `Ziptide.Content.GardenSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/GardenSpawnDefinition.cs:7` — `/// A garden planter authored as PACK DATA (Quality Bar P3): JobDirector spawns a GardenPlotRuntime`
- **reference** · `Ziptide.Content.MachineSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/MachineSpawnDefinition.cs:7` — `/// A repairable machine authored as PACK DATA (GAME_PLAN M2): JobDirector spawns a RepairableMachine`
- **reference** · `Ziptide.Content.MineSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/MineSpawnDefinition.cs:7` — `/// A placed extractor authored as PACK DATA (GAME_PLAN M2): JobDirector spawns a MiningRigRuntime`
- **reference** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:9` — `/// (<c>JobDirector</c> for granting; the travel/offer UI for the requirement check) calls into it.`
- **reference** · `Ziptide.Content.WorldPackDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs:8` — `/// Data-driven world pack: scene name, themes, jobs, and spawn markers. Used by JobDirector and travel.`
- **reference** · `Ziptide.Content.WorldPackDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs:38` — `"Pure data — JobDirector materializes CollectibleRuntime objects, like spawnMarkers.")]`
- **reference** · `Ziptide.Content.WorldPackDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs:43` — `"Pure data — JobDirector materializes ChoiceStation objects, like spawnMarkers.")]`
- **reference** · `Ziptide.Content.WorldPackDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs:48` — `"power). Pure data — JobDirector materializes RepairableMachine objects, like spawnMarkers.")]`
- **reference** · `Ziptide.Content.WorldPackValidator` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackValidator.cs:9` — `/// no-opping jobs/travel. Headless (data-only) — EditMode-tested; JobDirector logs the results.`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:15` — `/// Idempotent scene patcher for D0 City: ensures D0_City scene exists, blockout (plaza, terraces, alley, railings), and runtime objects (JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle). Call from BuildAndroid or menu.`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:162` — `EnsureJobDirector(worldPack);`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:212` — `private static void EnsureJobDirector(WorldPackDefinition worldPack)`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:214` — `var go = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:215` — `var director = PatcherUtil.EnsureComponent<JobDirector>(go);`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherToxicCity` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs:283` — `var jdGo = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherToxicCity` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs:284` — `var director = PatcherUtil.EnsureComponent<JobDirector>(jdGo);`
- **reference** · `Ziptide.Editor.Patching.ToxicCityContractBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityContractBuilder.cs:19` — `/// that GameObject name (JobDirector.CheckGoToMarker).`
- **reference** · `Ziptide.Editor.Patching.ToxicCityContractBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityContractBuilder.cs:72` — `+ "WorldPack as job 0.\n\nStill needed (T-Dog/runtime): JobDirector -> JobRewards.Grant on "`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:16` — `/// target (JobDirector materializes them as Marker_&lt;id&gt; at runtime — pure data, no scene objects).`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:54` — `// A physical pickup in the world (JobDirector spawns a CollectibleRuntime from pack data).`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:73` — `// A repairable machine in the world (JobDirector spawns a RepairableMachine from pack data).`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:94` — `// A placed extractor (idle economy made visible; JobDirector spawns a MiningRigRuntime).`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:393` — `// The marker itself is pack DATA — JobDirector creates Marker_<id> at runtime.`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:434` — `// Physical pickups + machines + mines are PACK data (JobDirector spawns the runtimes at`
- **reference** · `Ziptide.Editor.Patching.WorldStubGenerator` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:18` — `/// WorldPackDefinition (+ exit pack) · spawn · JobDirector/kiosk/board · Build Settings entry.`
- **reference** · `Ziptide.Editor.Patching.WorldStubGenerator` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:454` — `var jdGo = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);`
- **reference** · `Ziptide.Editor.Patching.WorldStubGenerator` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:455` — `var director = PatcherUtil.EnsureComponent<JobDirector>(jdGo);`
- **reference** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:64` — `var jd = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.BeltPadSpawner` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:9` — `/// <see cref="BeltFloorSpawnDefinition"/> entries at scene start (the JobDirector`
- **reference** · `Ziptide.Gameplay.DeliveryCradleSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DeliveryCradleSocketInteractor.cs:9` — `/// Socket that accepts delivery items (by itemId) and notifies JobDirector for DeliverToSocketStep.`
- **reference** · `Ziptide.Gameplay.DeliveryCradleSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DeliveryCradleSocketInteractor.cs:13` — `[Tooltip("Socket id reported to JobDirector (e.g. delivery_cradle).")]`
- **reference** · `Ziptide.Gameplay.DeliveryCradleSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DeliveryCradleSocketInteractor.cs:19` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.DeliveryCradleSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DeliveryCradleSocketInteractor.cs:21` — `public void Bind(JobDirector director)`
- **reference** · `Ziptide.Gameplay.DeliveryCradleSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DeliveryCradleSocketInteractor.cs:30` — `_director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.DispatchKiosk` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:7` — `/// XR interactable kiosk to accept/start a job. Assign JobDirector or it will be found at runtime.`
- **reference** · `Ziptide.Gameplay.DispatchKiosk` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:14` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.DispatchKiosk` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:16` — `public void Bind(JobDirector director)`
- **reference** · `Ziptide.Gameplay.DispatchKiosk` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:24` — `_director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.JobCollectible` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs:8` — `/// Place on a grabbable item. When the player grabs it, reports collect to JobDirector for CollectItemIdCountStep.`
- **reference** · `Ziptide.Gameplay.JobCollectible` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs:14` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.JobCollectible` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs:25` — `_director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.JobCollectible` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs:38` — `public void Bind(JobDirector director)`
- **declaration** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:10` — `public class JobDirector : MonoBehaviour`
- **reference** · `Ziptide.Gameplay.JobTarget` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobTarget.cs:6` — `/// Place on a target with TargetRuntime. When the target is hit, notifies JobDirector for ShootTargetsCountStep.`
- **reference** · `Ziptide.Gameplay.JobTarget` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobTarget.cs:12` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.JobTarget` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobTarget.cs:22` — `_director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.JobTarget` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobTarget.cs:33` — `public void Bind(JobDirector director)`
- **reference** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:12` — `[SerializeField] private JobDirector jobDirector;`
- **reference** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:16` — `public void Bind(JobDirector director)`
- **reference** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:32` — `jobDirector = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:13` — `/// (ProfileEconomy resolves idle accrual). Spawned by JobDirector from`
- **reference** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:30` — `/// <summary>Build + bind. Called by JobDirector right after AddComponent (runtime only).</summary>`
- **reference** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:12` — `/// Self-built at runtime by JobDirector from <see cref="ChoiceSpawnDefinition"/> pack data (never in`
- **reference** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:11` — `/// (<see cref="JobDirector.ReportCollect"/>), sets its story flag (this is how a Transmission`
- **reference** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:13` — `/// Transmission clarity tier, then absorbs (destroys). Spawned at runtime by JobDirector from`
- **reference** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:23` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:29` — `public void Init(CollectibleSpawnDefinition def, JobDirector director)`
- **reference** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:115` — `if (_director == null) _director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:19` — `/// Spawned by JobDirector from <see cref="GardenSpawnDefinition"/> pack data at the HarvestGrove`
- **reference** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:40` — `/// <summary>Build + bind. Called by JobDirector right after AddComponent (runtime only).</summary>`
- **reference** · `Ziptide.Gameplay.MiningRigRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:13` — `/// you). Spawned by JobDirector from <see cref="MineSpawnDefinition"/> pack data.`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:12` — `/// <see cref="JobDirector.ReportRepair"/>. Spawned by JobDirector from`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:28` — `private JobDirector _director;`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:58` — `public void Init(MachineSpawnDefinition def, JobDirector director)`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:259` — `if (_director == null) _director = FindObjectOfType<JobDirector>();`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:151` — `// The machine is spawned at runtime by JobDirector, so keep looking until found —`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:160` — `// observes and its repaired state — divergence from the JobDirector-spawned machine`
- **reference** · `Ziptide.Gameplay.TransmissionConsole` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:12` — `/// screen. Spawned by JobDirector next to any fragment pickup (the playback device lives where the`
- **reference** · `Ziptide.Gameplay.HazardZoneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:10` — `/// Player detection is a cheap poll against the rig position (same style as JobDirector's marker`

### `JobRuntime`

- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:8` — `/// Scene-level owner of JobRuntime. References WorldPackDefinition, updates ObjectiveBoard, creates spawn markers, and wires DispatchKiosk and delivery/target callbacks.`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:21` — `private JobRuntime _runtime = new JobRuntime();`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:26` — `public JobRuntime Runtime => _runtime;`
- **declaration** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:11` — `public class JobRuntime`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:13` — `public class JobRuntimeCollectTests`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:42` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:57` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:74` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:84` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeCollectTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeCollectTests.cs:102` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:13` — `public class JobRuntimeRepairTests`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:42` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:52` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:66` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:79` — `var rt = new JobRuntime();`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:89` — `var rt = new JobRuntime();`

### `ObjectiveBoard`

- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:15` — `/// Idempotent scene patcher for D0 City: ensures D0_City scene exists, blockout (plaza, terraces, alley, railings), and runtime objects (JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle). Call from BuildAndroid or menu.`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:164` — `EnsureObjectiveBoard();`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:234` — `private static void EnsureObjectiveBoard()`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:236` — `var go = PatcherUtil.EnsureRootObject("ObjectiveBoard", new Vector3(-1f, 4.0f, 1.5f));`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherD0` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:237` — `PatcherUtil.EnsureComponent<ObjectiveBoard>(go);`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherToxicCity` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs:295` — `var boardGo = PatcherUtil.EnsureRootObject("ObjectiveBoard", spawnPos + new Vector3(-1.5f, 1.6f, 1.5f));`
- **reference** · `Ziptide.Editor.Patching.ScenePatcherToxicCity` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs:296` — `PatcherUtil.EnsureComponent<ObjectiveBoard>(boardGo);`
- **reference** · `Ziptide.Editor.Patching.ToxicCityContractBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityContractBuilder.cs:73` — `+ "completion, and ObjectiveBoard/RILL text.", "OK");`
- **reference** · `Ziptide.Editor.Patching.WorldStubGenerator` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:466` — `var boardGo = PatcherUtil.EnsureRootObject("ObjectiveBoard", spawnPos + new Vector3(-1.5f, 1.6f, 1.5f));`
- **reference** · `Ziptide.Editor.Patching.WorldStubGenerator` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:467` — `PatcherUtil.EnsureComponent<ObjectiveBoard>(boardGo);`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:8` — `/// Scene-level owner of JobRuntime. References WorldPackDefinition, updates ObjectiveBoard, creates spawn markers, and wires DispatchKiosk and delivery/target callbacks.`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:16` — `[SerializeField] private ObjectiveBoard objectiveBoard;`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:284` — `objectiveBoard = FindObjectOfType<ObjectiveBoard>();`
- **declaration** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:9` — `public class ObjectiveBoard : MonoBehaviour`

### `REPAIR_TRACE`

- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:123` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=director id=" + GetInstanceID()`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:87` — `// but the objective still asks" must show its cause — every branch logs REPAIR_TRACE.`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:90` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=runtime machine=" + machineId`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:100` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=runtime machine=" + machineId`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:114` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=runtime machine=" + machineId`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:223` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=bank_drain step=" + CurrentStepIndex`
- **reference** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:67` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=board director=" + jobDirector.GetInstanceID()`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:166` — `Debug.Log("ZIPTIDE: REPAIR_TRACE hop=castoff armed=" + armed`
- **reference** · `Ziptide.Tests.EditMode.RecoveryCheckpointEvidenceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryCheckpointEvidenceTests.cs:72` — `int repair = CountRuntimeLogCalls(root, "REPAIR_TRACE");`
- **reference** · `Ziptide.Tests.EditMode.RecoveryCheckpointEvidenceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryCheckpointEvidenceTests.cs:78` — `"REPAIR_TRACE no longer covers enough hops to distinguish state from presentation.");`

### `RepairMachineCountStepDefinition`

- **declaration** · `Ziptide.Content.RepairMachineCountStepDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/RepairMachineCountStepDefinition.cs:8` — `public class RepairMachineCountStepDefinition : JobStepDefinition`
- **reference** · `Ziptide.Content.MachineSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/MachineSpawnDefinition.cs:16` — `[Tooltip("Machine id — matched by RepairMachineCountStepDefinition.machineId.")]`
- **reference** · `Ziptide.Content.WorldPackValidator` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackValidator.cs:132` — `if (job.steps[s] is RepairMachineCountStepDefinition rm)`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:417` — `var step = LoadOrCreate<RepairMachineCountStepDefinition>(stepPath);`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:95` — `if (step is RepairMachineCountStepDefinition repair &&`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:196` — `var step = GetCurrentStep() as RepairMachineCountStepDefinition;`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:298` — `else if (step is RepairMachineCountStepDefinition repair)`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:23` — `private static RepairMachineCountStepDefinition Repair(string machineId, int count = 1)`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:25` — `var s = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();`
- **reference** · `Ziptide.Tests.EditMode.WorldPackValidatorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldPackValidatorTests.cs:182` — `var rm = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();`
- **reference** · `Ziptide.Tests.EditMode.WorldPackValidatorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldPackValidatorTests.cs:195` — `var rm = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();`

### `RepairableMachine`

- **reference** · `Ziptide.Content.RepairMachineCountStepDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/RepairMachineCountStepDefinition.cs:6` — `/// Step: repair a count of machines (hands-on RepairableMachine stages: panel → part → power).`
- **reference** · `Ziptide.Content.MachineSpawnDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/MachineSpawnDefinition.cs:7` — `/// A repairable machine authored as PACK DATA (GAME_PLAN M2): JobDirector spawns a RepairableMachine`
- **reference** · `Ziptide.Content.WorldPackDefinition` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs:48` — `"power). Pure data — JobDirector materializes RepairableMachine objects, like spawnMarkers.")]`
- **reference** · `Ziptide.Editor.Audit.PerceptualCoverageAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/PerceptualCoverageAuditRules.cs:30` — `foreach (RepairableMachine machine in Object.FindObjectsOfType<RepairableMachine>(true))`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:73` — `// A repairable machine in the world (JobDirector spawns a RepairableMachine from pack data).`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:117` — `/// Called by RepairableMachine when its final repair stage completes. For RepairMachineCountStep.`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:217` — `go.AddComponent<RepairableMachine>().Init(m, this);`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:83` — `/// <summary>A machine finished its hands-on repair (RepairableMachine's final stage).</summary>`
- **reference** · `Ziptide.Gameplay.RepairStage` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:6` — `/// Public read-only vocabulary for the existing RepairableMachine state. Numeric order preserves`
- **reference** · `Ziptide.Gameplay.RepairStage` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:29` — `RepairableMachine designatedMachine)`
- **reference** · `Ziptide.Gameplay.RepairStage` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:46` — `RepairableMachine designatedMachine,`
- **reference** · `Ziptide.Gameplay.RepairStage` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:47` — `Action<RepairableMachine> publish)`
- **declaration** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:16` — `public class RepairableMachine : MonoBehaviour, IScannable`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:27` — `[Tooltip("RepairableMachine id that must be RUNNING before PUNCH IT arms (empty = no gate).")]`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:31` — `private RepairableMachine _armingMachine; // cached once found; absence is re-checked per press`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:153` — `foreach (var m in FindObjectsOfType<RepairableMachine>())`
- **reference** · `Ziptide.Tests.EditMode.QuestWeaponAndCouplerRegressionTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/QuestWeaponAndCouplerRegressionTests.cs:123` — `var machine = machineRoot.AddComponent<RepairableMachine>();`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:12` — `public class RepairableMachineSignalTests`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:40` — `RepairableMachine machine = CreateMachine("machine");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:67` — `RepairableMachine designated = CreateMachine("same_id");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:68` — `RepairableMachine lookalike = CreateMachine("same_id");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:94` — `RepairableMachine designated = CreateMachine("designated");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:95` — `RepairableMachine other = CreateMachine("other");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:97` — `RepairableMachine seen = null;`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:149` — `"RepairableMachine.cs");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:155` — `"public class RepairableMachine : MonoBehaviour, IScannable",`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:238` — `private RepairableMachine CreateMachine(string name)`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:242` — `return go.AddComponent<RepairableMachine>();`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:245` — `private static WristScanTarget Target(RepairableMachine machine)`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:255` — `private static void SetStage(RepairableMachine machine, RepairStage stage)`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:257` — `FieldInfo field = typeof(RepairableMachine).GetField(`

### `ReportRepair`

- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:119` — `public void ReportRepair(string machineId)`
- **reference** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:125` — `_runtime.ReportRepair(machineId);`
- **reference** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:84` — `public void ReportRepair(string machineId)`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:12` — `/// <see cref="JobDirector.ReportRepair"/>. Spawned by JobDirector from`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:260` — `if (_director != null) _director.ReportRepair(_def.machineId);`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:45` — `rt.ReportRepair("cistern_pump");`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:55` — `rt.ReportRepair("fuel_rig");`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:59` — `rt.ReportRepair("cistern_pump");`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:69` — `rt.ReportRepair("cistern_pump"); // fixed before reaching the gate`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:80` — `rt.ReportRepair("cistern_pump"); // fixed before visiting the kiosk`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:90` — `rt.ReportRepair("fuel_rig"); // banked under its own id`
- **reference** · `Ziptide.Tests.EditMode.JobRuntimeRepairTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRuntimeRepairTests.cs:94` — `rt.ReportRepair("cistern_pump");`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:178` — `int reportRepair = source.IndexOf("_director.ReportRepair(_def.machineId);", runningAssign, StringComparison.Ordinal);`
- **reference** · `Ziptide.Tests.EditMode.RepairableMachineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:205` — `Assert.AreEqual(1, Count(source, "_director.ReportRepair(_def.machineId);"));`

### `ShipCastOffRuntime`

- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:381` — `if (kit.sceneName == "W000_DriftIn" && ship.GetComponent<ShipCastOffRuntime>() == null)`
- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:382` — `ship.gameObject.AddComponent<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Editor.FirstHourSurfaceAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs:36` — `ShipCastOffRuntime castOff = FindInScene<ShipCastOffRuntime>(scene);`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:41` — `/// Queried by ShipCastOffRuntime's arming gate.</summary>`
- **declaration** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:20` — `public class ShipCastOffRuntime : MonoBehaviour`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:10` — `/// destination into ShipCastOffRuntime; it never launches or calls TravelCoordinator itself.`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:16` — `private ShipCastOffRuntime _castOff;`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:19` — `public void Configure(ShipCastOffRuntime castOff)`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:26` — `if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:32` — `if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:113` — `var castOff = go.AddComponent<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:195` — `string castOff = Read("Gameplay", "Runtime", "Story", "ShipCastOffRuntime.cs");`

### `gate_coupler`

- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:138` — `.Machine("gate_coupler", new Vector3(6, 0.1f, 14), "coupler_cell",`
- **reference** · `Ziptide.Editor.Patching.WorldJobLibrary` · `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:140` — `.Repair("gate_coupler")`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:31` — `/// <summary>Stable machine id from pack data (e.g. "gate_coupler"). Null before Init.</summary>`
- **reference** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:28` — `[SerializeField] private string armingMachineId = "gate_coupler";`

## shipPresentation

- References: **98**
- Files: **19**
- Non-declaration caller files: **16**

### `Fuselage_Aft`

- **reference** · `Ziptide.Editor.Audit.FullSendPresentationAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs:43` — `"Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip",`
- **reference** · `Ziptide.Editor.Patching.ShipHullBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:42` — `Part(ship, "Fuselage_Aft", PrimitiveType.Cube,`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:27` — `private static readonly string[] FuselageParts = { "Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip", "DorsalSpine" };`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:23` — `"Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip",`

### `ShipCastOffRuntime`

- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:381` — `if (kit.sceneName == "W000_DriftIn" && ship.GetComponent<ShipCastOffRuntime>() == null)`
- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:382` — `ship.gameObject.AddComponent<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Editor.FirstHourSurfaceAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs:36` — `ShipCastOffRuntime castOff = FindInScene<ShipCastOffRuntime>(scene);`
- **reference** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:41` — `/// Queried by ShipCastOffRuntime's arming gate.</summary>`
- **declaration** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:20` — `public class ShipCastOffRuntime : MonoBehaviour`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:10` — `/// destination into ShipCastOffRuntime; it never launches or calls TravelCoordinator itself.`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:16` — `private ShipCastOffRuntime _castOff;`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:19` — `public void Configure(ShipCastOffRuntime castOff)`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:26` — `if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:32` — `if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:113` — `var castOff = go.AddComponent<ShipCastOffRuntime>();`
- **reference** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:195` — `string castOff = Read("Gameplay", "Runtime", "Story", "ShipCastOffRuntime.cs");`

### `ShipChassisPreset`

- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:26` — `public sealed class ShipChassisPreset`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:41` — `public static readonly ShipChassisPreset[] All =`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:43` — `new ShipChassisPreset { Id = "interceptor", DisplayName = "Interceptor",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:49` — `new ShipChassisPreset { Id = "hauler", DisplayName = "Hauler",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:55` — `new ShipChassisPreset { Id = "gunship", DisplayName = "Gunship",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:61` — `new ShipChassisPreset { Id = "explorer", DisplayName = "Explorer",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:67` — `new ShipChassisPreset { Id = "salvager", DisplayName = "Salvager",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:73` — `new ShipChassisPreset { Id = "racer", DisplayName = "Racer",`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:81` — `public static ShipChassisPreset Find(string id)`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:136` — `public static ShipStats Resolve(ShipChassisPreset chassis, IEnumerable<ShipModulePreset> modules)`
- **declaration** · `Ziptide.Content.Ship.ShipSilhouette` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:138` — `if (chassis == null) chassis = ShipChassisPreset.Find(null);`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:59` — `for (int i = 0; i < ShipChassisPreset.All.Length; i++)`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:61` — `var c = ShipChassisPreset.All[i];`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:109` — `var chassis = ShipChassisPreset.Find(ShipLocker.GetEquipped(profile, "chassis"));`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:11` — `/// · CHASSIS — reproportions the named ShipHullBuilder parts per ShipChassisPreset (fuselage`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:36` — `var chassis = ShipChassisPreset.Find(chassisId);`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:47` — `private static void ApplyProportions(Transform root, ShipChassisPreset c)`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:87` — `private static Color EngineGlowOf(ShipChassisPreset c)`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:176` — `private static void ApplyNameplate(Transform root, PlayerProfile profile, ShipChassisPreset chassis)`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:196` — `private static void ApplyHum(GameObject shipRoot, ShipChassisPreset chassis)`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:210` — `private static AudioClip MakeHum(ShipChassisPreset chassis)`
- **reference** · `Ziptide.Ship.ShipFlightRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:118` — `var chassis = ShipChassisPreset.Find(equipped);`
- **reference** · `Ziptide.Tests.EditMode.ShipFlightParamsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipFlightParamsTests.cs:77` — `var racer = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("racer"), null);`
- **reference** · `Ziptide.Tests.EditMode.ShipFlightParamsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipFlightParamsTests.cs:102` — `foreach (var chassis in ShipChassisPreset.All)`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:14` — `Assert.AreEqual(6, ShipChassisPreset.All.Length, "the spec's >=6 chassis");`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:17` — `foreach (var c in ShipChassisPreset.All)`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:33` — `foreach (var c in ShipChassisPreset.All)`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:47` — `var chassis = ShipChassisPreset.Find("interceptor");`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:63` — `var racer = ShipChassisPreset.Find("racer");`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:81` — `var a = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("gunship"), null);`
- **reference** · `Ziptide.Tests.EditMode.ShipLoadoutCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLoadoutCoreTests.cs:82` — `var b = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("gunship"), null);`

### `ShipHullBuilder`

- **reference** · `Ziptide.Editor.Audit.FullSendPresentationAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs:36` — `if (renderers.Length < ShipHullBuilder.MinimumHeroRenderers)`
- **reference** · `Ziptide.Editor.Audit.FullSendPresentationAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs:39` — `+ ShipHullBuilder.MinimumHeroRenderers + ".", path);`
- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:372` — `// real ~19-part silhouette now (ShipHullBuilder), same bounding box, so the boarding`
- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:377` — `ShipHullBuilder.Build(ship, s.shipSize, kit.palette);`
- **declaration** · `Ziptide.Editor.Patching.ShipHullBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:14` — `public static class ShipHullBuilder`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:11` — `/// · CHASSIS — reproportions the named ShipHullBuilder parts per ShipChassisPreset (fuselage`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:14` — `/// baked Forge meshes supersede this through the same parent later (ShipHullBuilder's own`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:10` — `public sealed class HeroShipHullBuilderTests`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:19` — `ShipHullBuilder.Build(ship.transform, new Vector3(5f, 3f, 12f), new GlobalPalette());`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:38` — `Assert.That(renderers.Length, Is.GreaterThanOrEqualTo(ShipHullBuilder.MinimumHeroRenderers));`

### `ShipHullBuilder.Build`

- **reference** · `Ziptide.Editor.Patching.CityBuilder` · `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:377` — `ShipHullBuilder.Build(ship, s.shipSize, kit.palette);`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:19` — `ShipHullBuilder.Build(ship.transform, new Vector3(5f, 3f, 12f), new GlobalPalette());`

### `ShipLocker`

- **declaration** · `Ziptide.Core.ShipLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:11` — `public static class ShipLocker`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:13` — `/// equips via ShipLocker (profile-flag persistence) and re-runs ShipRefit LIVE on the berth hull —`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:98` — `ShipLocker.Equip(profile, slot, id);`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:109` — `var chassis = ShipChassisPreset.Find(ShipLocker.GetEquipped(profile, "chassis"));`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:111` — `foreach (var id in ShipLocker.EquippedModules(profile, new[] { "engine", "wings", "hardpoint", "cargo" }))`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:21` — `/// · NAMEPLATE — the ship's name (ShipLocker "name") on the bow.`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:35` — `string chassisId = ShipLocker.GetEquipped(profile, "chassis");`
- **reference** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:180` — `string shipName = ShipLocker.GetEquipped(profile, "name");`
- **reference** · `Ziptide.Ship.ShipFlightRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:115` — `string equipped = profile != null ? ShipLocker.GetEquipped(profile, "chassis") : null;`
- **reference** · `Ziptide.Ship.ShipFlightRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:121` — `foreach (var id in ShipLocker.EquippedModules(profile, chassis.SlotIds))`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:8` — `public class ShipLockerTests`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:14` — `ShipLocker.Equip(p, "chassis", "interceptor");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:15` — `Assert.AreEqual("interceptor", ShipLocker.GetEquipped(p, "chassis"));`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:16` — `ShipLocker.Equip(p, "chassis", "racer");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:17` — `Assert.AreEqual("racer", ShipLocker.GetEquipped(p, "chassis"), "swap replaces");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:27` — `ShipLocker.Equip(p, "chassis", "hauler");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:28` — `ShipLocker.Equip(p, "engine", "engine_tide");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:29` — `ShipLocker.Equip(p, "name", "LOW TIDE");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:30` — `Assert.AreEqual("hauler", ShipLocker.GetEquipped(p, "chassis"));`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:31` — `Assert.AreEqual("engine_tide", ShipLocker.GetEquipped(p, "engine"));`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:32` — `Assert.AreEqual("LOW TIDE", ShipLocker.GetEquipped(p, "name"));`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:39` — `ShipLocker.Equip(p, "engine", "engine_ion");`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:40` — `var mods = ShipLocker.EquippedModules(p, new[] { "engine", "wings", "hardpoint" });`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:48` — `Assert.IsNull(ShipLocker.GetEquipped(null, "chassis"));`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:49` — `ShipLocker.Equip(null, "chassis", "racer"); // no throw`
- **reference** · `Ziptide.Tests.EditMode.ShipLockerTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:50` — `Assert.AreEqual(0, ShipLocker.EquippedModules(null, null).Count);`

### `ShipRefit`

- **reference** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:372` — `/// Public sibling, never a nested MonoBehaviour (the ShipRefitBaseXf lesson).</summary>`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:13` — `/// equips via ShipLocker (profile-flag persistence) and re-runs ShipRefit LIVE on the berth hull —`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:19` — `[Tooltip("The hull root ShipRefit applies to (the boarding station's ship).")]`
- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:100` — `if (shipRoot != null) ShipRefit.Apply(shipRoot);`
- **reference** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:55` — `ShipRefit.Apply(gameObject);`
- **declaration** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:25` — `public static class ShipRefit`
- **declaration** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:106` — `private static ShipRefitBaseXf Remember(Transform t)`
- **declaration** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:108` — `var b = t.GetComponent<ShipRefitBaseXf>();`
- **declaration** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:111` — `b = t.gameObject.AddComponent<ShipRefitBaseXf>();`
- **declaration** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:232` — `public class ShipRefitBaseXf : MonoBehaviour`
- **reference** · `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:29` — `anchor + " is required by ShipRefit and must stay a direct child.");`

### `ShipRefit.Apply`

- **reference** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:100` — `if (shipRoot != null) ShipRefit.Apply(shipRoot);`
- **reference** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:55` — `ShipRefit.Apply(gameObject);`

