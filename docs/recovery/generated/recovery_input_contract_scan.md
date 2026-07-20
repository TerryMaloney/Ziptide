# ZIPTIDE Runtime Input Contract Scan

- Scanned C# files: **710**
- Runtime-created bindings: **18**
- Legacy menu-chord references: **0**
- Controls bound by more than one owner: **5**

## Legacy menu-chord references

None.

## Runtime-created bindings

| Owner | Action | Field | Binding | Source |
|---|---|---|---|---|
| `Ziptide.Gameplay.QuickSwap` | `ZiptideQuickSwap` | `_swap` | `<XRController>{RightHand}/secondaryButton` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:27` |
| `Ziptide.Gameplay.DashLocomotion` | `ZiptideJump` | `_jumpAction` | `<XRController>{RightHand}/primaryButton` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:99` |
| `Ziptide.Gameplay.DashLocomotion` | `ZiptideSprint` | `_sprintAction` | `<XRController>{LeftHand}/thumbstickClicked` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:104` |
| `Ziptide.Gameplay.DashLocomotion` | `ZiptideCrouch` | `_crouchAction` | `<XRController>{RightHand}/thumbstickClicked` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:109` |
| `Ziptide.Gameplay.EmergencyRespawn` | `EmergencyLeft` | `_leftGrip` | `<XRController>{LeftHand}/grip` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:24` |
| `Ziptide.Gameplay.EmergencyRespawn` | `EmergencyRight` | `_rightGrip` | `<XRController>{RightHand}/grip` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:28` |
| `Ziptide.Gameplay.PingTool` | `ZiptidePing` | `_ping` | `<XRController>{LeftHand}/triggerPressed` | `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs:28` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightThrottle` | `_leftStick` | `<XRController>{LeftHand}/thumbstick` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:140` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightSteer` | `_rightStick` | `<XRController>{RightHand}/thumbstick` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:142` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightBoostL3` | `_boostStickClick` | `<XRController>{LeftHand}/thumbstickClicked` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:144` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightBoostA` | `_boostButton` | `<XRController>{RightHand}/primaryButton` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:146` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightRollL` | `_rollLeftButton` | `<XRController>{LeftHand}/primaryButton` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:148` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightRollR` | `_rollRightButton` | `<XRController>{RightHand}/secondaryButton` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:150` |
| `Ziptide.Ship.ShipFlightRuntime` | `ZiptideFlightFire` | `_fireAction` | `<XRController>{RightHand}/trigger` | `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:152` |
| `Ziptide.Ship.VehicleRuntime` | `ZiptideRideThrottle` | `_leftStick` | `<XRController>{LeftHand}/thumbstick` | `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:71` |
| `Ziptide.Ship.VehicleRuntime` | `ZiptideRideSteer` | `_rightStick` | `<XRController>{RightHand}/thumbstick` | `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:73` |
| `Ziptide.Ship.VehicleRuntime` | `ZiptideRideBoostL3` | `_boostL3` | `<XRController>{LeftHand}/thumbstickClicked` | `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:75` |
| `Ziptide.Ship.VehicleRuntime` | `ZiptideRideBoostA` | `_boostA` | `<XRController>{RightHand}/primaryButton` | `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:77` |

## Cross-owner control collisions

### `<XRController>{LeftHand}/thumbstick`

- `Ziptide.Ship.ShipFlightRuntime` → `ZiptideFlightThrottle` at `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:140`
- `Ziptide.Ship.VehicleRuntime` → `ZiptideRideThrottle` at `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:71`

### `<XRController>{LeftHand}/thumbstickClicked`

- `Ziptide.Gameplay.DashLocomotion` → `ZiptideSprint` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:104`
- `Ziptide.Ship.ShipFlightRuntime` → `ZiptideFlightBoostL3` at `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:144`
- `Ziptide.Ship.VehicleRuntime` → `ZiptideRideBoostL3` at `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:75`

### `<XRController>{RightHand}/primaryButton`

- `Ziptide.Gameplay.DashLocomotion` → `ZiptideJump` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:99`
- `Ziptide.Ship.ShipFlightRuntime` → `ZiptideFlightBoostA` at `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:146`
- `Ziptide.Ship.VehicleRuntime` → `ZiptideRideBoostA` at `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:77`

### `<XRController>{RightHand}/secondaryButton`

- `Ziptide.Gameplay.QuickSwap` → `ZiptideQuickSwap` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:27`
- `Ziptide.Ship.ShipFlightRuntime` → `ZiptideFlightRollR` at `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:150`

### `<XRController>{RightHand}/thumbstick`

- `Ziptide.Ship.ShipFlightRuntime` → `ZiptideFlightSteer` at `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:142`
- `Ziptide.Ship.VehicleRuntime` → `ZiptideRideSteer` at `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:73`

