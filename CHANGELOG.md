# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-07-16

### Added

- **Wrong-side resistance** — new `WrongSideKicksNeeded` setting (Breach Attempts, default `1`): breaching a door against its swing direction can take several kicks before it gives in. Failed kicks play the vanilla hit animation, particle effect, and sound; the door stays shut.
- **Force locked doors** — new `AllowBreachWithoutKey` setting (Breach Attempts, default `false`): key-locked doors can be breached without their key. Every failed kick raises the chance of the next one (`NoKeyBaseChance` / `NoKeyChanceIncrease`, defaults `0.2`/`0.2` → guaranteed by the 5th kick). While enabled, keys are never consumed by breaching; use the key to unlock normally instead. Only applies while `RequireMatchingKey` is `true`.
- Both mechanics combine on a key-locked door kicked from the wrong side: the kick-count gate and the chance roll must both pass.
- With all defaults the mod behaves exactly like 1.0.0 — both new mechanics are opt-in.

### Fixed (Fika co-op)

- Key checks and key consumption now only run on the breaching player's own machine. Previously, teammates replaying someone else's breach checked — and consumed — the **spectator's** key.
- Headless hosts no longer reject key-locked breaches performed by clients (the host resolved a null `MainPlayer` and failed the key check, desyncing the door state).
- All breach outcomes are deterministic functions of synced state (door id, breacher profile id, shared attempt counter), so every peer replaying a breach computes the same result. No custom network packets, no Fika assembly reference.

### Notes for Fika raids

- Install the mod on **every machine**, including the headless host.
- The Breach Attempts settings must be **identical on all machines** — they feed the shared outcome calculation.
- DoorDash sprint-ram still requires the matching key on key-locked doors, even in keyless mode, so ramming cannot bypass the attempt mechanic.

## [1.0.0]

### Added

- Initial release.
- **Dual-side breaching** — breach operable doors from the front or back (F-menu and DoorDash sprint-ram).
- **Smart swing direction** (`AdjustOpenDirection`) — doors open away from the breaching player, including doors with inverted `OpenAngle` values (`_Variant` prefabs).
- **One-way breach doors preserved** — non-operatable breach-only doors and doors locked without a key (e.g. Factory `_Variant`) keep their vanilla single-side behaviour.
- **Locked doors with keys** — `RequireMatchingKey` / `ConsumeKeyOnBreach` for doors with a configured `KeyId`.
- **`AllowNonBreachableDoors`** — also breach doors flagged as non-breachable in map data.
- **DoorDash compatibility** — optional patches (swing check, locked-door ram); no fork required.
