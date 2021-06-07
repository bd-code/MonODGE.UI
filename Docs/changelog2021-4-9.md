## MonODGE.UI - Update 2021.4.9

### Abridged list of changes for 2021-4-9 commit:

*Contains several breaking changes from last update! Client code WILL need changes!*

- Added overloaded OdgeComponent.Draw(SpriteBatch, Rectangle). Draws component relative to parentRect parameter. 
- Sub-components (inner text, buttons, etc.) now drawn relative to parent component rather than at pre-calculated absolute positions.
- Renamed AlignedText to StyledText.
- Components with text now use StyledText.
- Changed Options to Buttons, with an OdgeButton superclass.
- Buttons now require a StyleSheet parameter.
- Added OdgeButtonFactory (usefulness in question, needs work).
- Renamed TextBox to EntryBox.
- EntryBox now uses Style.Borders rather than an internal default outline.
- Menu classes no longer take a Button (was Option) list in constructor. Use List.AddOption() instead.
- Changed how Menu classes position and draw Buttons, text using the new overloaded Draw().
- Added text shadowing.
- StyleSheet's constructor is now empty. Use object initializers instead.
- Various bug fixes.
- Various untracked new features, options, and other changes, including changes to constructor and method signitures.
- LOTS of internal implementation changes.
