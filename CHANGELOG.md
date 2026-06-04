# Changelog

## [0.2.0] - TBD

### Notes

- The TypeRegistry now uses an attribute (`JilwerType`) instead of the
scriptable object (`TypeRegistryAsset`). This should make development a bit
easier and is a better general pattern Jilwer will now follow for future systems.
- Jilwer will use its custom Error enum for its API, and the general pattern
can be used for your own stuff using the Jilwer Error enum.

### Added

- Jilwer Type Attribute
- Help buttons (Gravinium/Jilwer/Help/*)
- Error enum

### Changed

- Swapped asmdef from old `org.gravinium.jilwer` to new `Gravinium.Jilwer`.
- Changed UdonSharp asmdef to match above change.
- Renamed ObjectArrayList to ArrayList
- Changed return types in `ArrayList` (now using Error enum)
- Renamed `ArrayList.Add` to `ArrayList.TryAdd`
- Renamed `ArrayList.Get` to `ArrayList.TryGet`
- Renamed `ArrayList.Remove` to `ArrayList.TryRemove`

### Removed

- Type Registry Asset

## [0.1.0] - 2026-04-27

### Added
- Type Registry
- Init version