# maschine-DualSideDoorBreach

Tarkov doors have opinions. Hinge side only. Swing into your face. Locked for no reason. This mod disagrees.

Breach doors from **either side** and have them swing **away from you** instead of into your face or through the wall. Works with the normal F-menu breach action and with **[DoorDash](https://github.com/bmpq/spt-doordash)** sprint-ram breaching.

Vanilla only lets you breach from the hinge side and always kicks the door open in a fixed direction. This mod removes that restriction for normal doors while keeping vanilla behaviour for special one-way breach doors.

---

## Features

- **Dual-side breaching** — breach operable doors from the front or back (F-menu and DoorDash)
- **Smart swing direction** — doors open toward the side you are breaching from, including doors with inverted `OpenAngle` values (common on `_Variant` prefabs)
- **One-way breach doors preserved** — factory-style doors that are locked without a key, or non-operatable breach-only doors, still only work from the correct side
- **Locked doors with keys** — optionally require the matching key in your inventory to breach locked doors, and optionally consume a key charge on breach
- **Wrong-side resistance** *(optional)* — kicking a door against its swing direction can take several kicks before it gives in
- **Force locked doors** *(optional)* — breach key-locked doors without the key; every failed kick raises the chance that the next one succeeds
- **Fika-ready** — breach outcomes are deterministic across peers; failed and successful kicks replay identically for everyone (see [Fika co-op](#fika-co-op))
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
| `WrongSideKicksNeeded` | Breach Attempts | `1` | Kicks needed to breach a door from the wrong side (against its swing direction); `1` = single kick as before |
| `AllowBreachWithoutKey` | Breach Attempts | `false` | Breach key-locked doors without the key, with a rising chance per failed kick |
| `NoKeyBaseChance` | Breach Attempts | `0.2` | Chance for the first keyless kick on a key-locked door |
| `NoKeyChanceIncrease` | Breach Attempts | `0.2` | Added chance per failed keyless kick |
| `DoorDash` | Compatibility | `true` | Allow DoorDash sprint-ram from both sides (requires DoorDash) |

**Notes:**

- `Enabled` must be `true` for any of the mod's behaviour to apply.
- `AdjustOpenDirection` controls swing correction; turn it off if you only want dual-side breaching without direction changes.
- Locked doors **without** a `KeyId` (breach-only doors) are not treated as key-locked doors.
- With default values the mod behaves exactly like v1.0.0 — both attempt mechanics are opt-in.
- `AllowBreachWithoutKey` only applies while `RequireMatchingKey` is `true`. While it is enabled, keys are **never consumed** by breaching (use the key to unlock normally instead), and DoorDash sprint-ram still requires the key so ramming cannot bypass the attempt mechanic.
- With the default chances a keyless breach is guaranteed by the 5th kick (0.2 → 0.4 → 0.6 → 0.8 → 1.0).
- Failed kicks play the vanilla hit animation, particle effect, and sound; the door simply stays shut.

---

## Compatibility

- **DoorDash** — supported via optional patches (`WillDoorSwingTowardsPlayer`, locked-door ram). A harmless cleanup warning from DoorDash at raid end (`RaycastBreacher` / `LocalPlayer`) is unrelated to this mod.
- Does not modify DoorDash itself; all compat logic lives in DualSideDoorBreach.

### Fika co-op

Fika replays every breach on every peer and recomputes the outcome locally. This mod is built for that model:

- All breach decisions (wrong-side kick counts, keyless chances) are **deterministic functions of synced state** — every peer computes the same result for the same kick, with no extra network packets.
- Key checks and key consumption only run on the breaching player's own machine; teammates and the headless host never touch their own inventory when replaying someone else's breach.

Requirements for co-op raids:

1. Install the mod on **every machine**, including the headless host (add it to the `required` list in `fika.jsonc` — peers without the mod compute vanilla outcomes and desync).
2. The **Breach Attempts settings must be identical on all machines** — they feed the shared outcome calculation.
3. DoorDash sprint-rams bypass the breach interaction Fika replicates; whether rams sync in co-op is up to DoorDash, not this mod.

Known limitation: a player reconnecting mid-raid starts with fresh attempt counters, so a door that was mid-sequence may need a different number of kicks on their client until it is breached once.

---

## Performance

Negligible impact. Patches only run when you interact with or breach a door. DoorDash's sprint raycast only fires above a velocity threshold. Neither mod adds per-frame work across the whole map.
