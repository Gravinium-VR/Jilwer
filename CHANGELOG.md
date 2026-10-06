# Changelog

## [0.4.0](https://github.com/Gravinium-VR/Jilwer/releases/tag/0.4.0) - TBD

### Notes

- Started the implementation of the event system. Nothing will be done with it
as of now.
- Started the Jilwer compiler.

### Added

- ArrayList Methods
  - `ToArray` - Returns the list as an array.
  - `Insert` - Adds an item to the specified index.
  - `SetResizePercentFactor` - Changes resize factor, default 0.5 (50% inc)
  - `Set` - Replaces the object at the specified index.
  - `Contains` - Returns true of an object is found in the list.
  - `TrimToSize` - Sets the capacity to the length of the list.
  - `Clear` - Empties the list.
  - `IsEmpty` - Returns true of the list is empty.

### Changed

- ArrayList Methods
  - `Insert(object item, int index)` is now `Add(int index, object item)`. 

### Removed

## [0.3.0](https://github.com/Gravinium-VR/Jilwer/releases/tag/0.3.0) - 2026-07-05

### Notes

- `Gravinium.Jilwer.Core.Collections` is now `Gravinium.Jilwer.Collections`

### Added

### Changed

- `Gravinium.Jilwer.Core.Collections` is now moved one level up and has its own
   asmdef to work with.
- Help menu now lives under Tools submenu.

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