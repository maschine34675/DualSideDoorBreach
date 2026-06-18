# maschine-DualSideDoorBreach

Breach doors from **either side** and have them swing **away from you** instead of into your face or through the wall. Works with the normal F-menu breach action and with **[DoorDash](https://github.com/bmpq/spt-doordash)** sprint-ram breaching.

Vanilla only lets you breach from the hinge side and always kicks the door open in a fixed direction. This mod removes that restriction for normal doors while keeping vanilla behaviour for special one-way breach doors.

---

## Features

- **Dual-side breaching** — breach operable doors from the front or back (F-menu and DoorDash)
- **Smart swing direction** — doors open toward the side you are breaching from, including doors with inverted `OpenAngle` values (common on `_Variant` prefabs)
- **One-way breach doors preserved** — factory-style doors that are locked without a key, or non-operatable breach-only doors, still only work from the correct side
- **Locked doors with keys** — optionally require the matching key in your inventory to breach locked doors, and optionally consume a key charge on breach
- **DoorDash compatible** — optional compatibility patches; no fork of DoorDash required
- **Lightweight** — logic only runs during door interaction, no map-wide scanning

---

## Requirements

- SPT with BepInEx
- **Optional:** [DoorDash](https://github.com/bmpq/spt-doordash) (`com.tarkin.doordash`) — soft dependency; compatibility patches apply automatically when DoorDash is installed

---

## Installation

1. Download `maschine-DualSideDoorBreach.dll`
2. Place it in `BepInEx/plugins/`
3. Start the game once to generate the config file
4. Adjust settings in `BepInEx/config/com.maschine.DualSideDoorBreach.cfg` if needed

---

## Configuration

| Setting | Section | Default | Description |
|---------|---------|---------|-------------|
| `Enabled` | General | `true` | Master toggle for dual-side breaching |
| `AdjustOpenDirection` | General | `true` | Swing the door away from the breaching player |
| `AllowNonBreachableDoors` | General | `true` | Also allow breaching doors flagged as non-breachable in map data |
| `RequireMatchingKey` | Locked Doors | `true` | Locked doors need the matching key in your inventory to breach |
| `ConsumeKeyOnBreach` | Locked Doors | `true` | Use one charge of the matching key when breaching a locked door |
| `DoorDash` | Compatibility | `true` | Allow DoorDash sprint-ram from both sides (requires DoorDash) |

**Notes:**

- `Enabled` must be `true` for any of the mod's behaviour to apply.
- `AdjustOpenDirection` controls swing correction; turn it off if you only want dual-side breaching without direction changes.
- Locked doors **without** a `KeyId` (breach-only doors) are not treated as key-locked doors.

---

## Compatibility

- **DoorDash** — supported via optional patches (`WillDoorSwingTowardsPlayer`, locked-door ram). A harmless cleanup warning from DoorDash at raid end (`RaycastBreacher` / `LocalPlayer`) is unrelated to this mod.
- Does not modify DoorDash itself; all compat logic lives in DualSideDoorBreach.

---

## Performance

Negligible impact. Patches only run when you interact with or breach a door. DoorDash's sprint raycast only fires above a velocity threshold. Neither mod adds per-frame work across the whole map.
