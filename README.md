# maschine-DualSideDoorBreach

Tarkov doors have opinions. Hinge side only. Swing into your face. Locked for no reason. This mod disagrees.

Breach doors from **either side** and have them swing **away from you** instead of into your face. Works with the normal F-menu breach action and with **[DoorDash by tarkin](https://forge.sp-tarkov.com/mod/2214/doordash)** sprint-ram breaching.

Vanilla only lets you breach from the hinge side and always kicks the door open in a fixed direction. This mod removes that restriction for normal doors while keeping vanilla behaviour for special one-way breach doors.

---

## Features

- **Dual-side breaching** — breach operable doors from the front or back (F-menu and DoorDash)
- **Smart swing direction** — doors open away from the side you are breaching from
- **One-way breach doors preserved** — factory-style breach-only doors, still only work from the correct side
- **Locked doors with keys** — require the matching key in your inventory to breach locked doors, and consume a key charge on breach
- **DoorDash compatible** — optional compatibility patches
- **Lightweight** — logic only runs during door interaction, no map-wide scanning

---

## Highly recommended mod

- [DoorDash by tarkin](https://forge.sp-tarkov.com/mod/2214/doordash) allows sprint-ram breaching of doors

![example](https://github.com/maschine34675/DualSideDoorBreach/blob/main/example.gif?raw=true)

## Installation

- As usual, unzip to your SPT directory

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

---

## Potential quirks

- I think I've covered all edge cases, but if you encounter any doors that behave strangely, let me know.
- Depending on map layout, doors can clip through the walls if opened in the "unintended" direction.