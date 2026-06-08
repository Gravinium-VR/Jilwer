# Changelog

## [0.3.0] - TBD

### Notes

- `Gravinium.Jilwer.Core.Collections` is now `Gravinium.Jilwer.Collections`

### Added

### Changed

- `Gravinium.Jilwer.Core.Collections` is now moved one level up and has its own
   asmdef to work with.

### Removed

## [0.2.0](https://github.com/Gravinium-VR/Jilwer/releases/tag/0.2.0) - 2026-06-05

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
- Changed everything to use the new Error pattern.
- Type Registry now stores information in a dictionary.
- Type Registry objects now start disabled.

### Removed

- Type Registry Asset

## [0.1.0](https://github.com/Gravinium-VR/Jilwer/releases/tag/0.1.0) - 2026-04-27

### Added
- Type Registry
- Init version