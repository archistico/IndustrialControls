# Accessibility

## Metadata

The release candidate supplies:

- accessible names;
- help text containing value or state;
- stable automation IDs;
- automation-tree visibility;
- assertive live-region metadata for new or returned unacknowledged alarms.

## Keyboard controls

- `IndustrialSlider`: standard slider keyboard behavior;
- `RotaryKnob`: arrow keys, `Home`, `End`;
- `SelectorSwitch`: arrow keys, `Home`, `End`;
- `IndustrialToggleSwitch`: standard toggle-button keyboard behavior;
- `IndustrialRockerSwitch`: standard toggle-button keyboard behavior;
- `SpringReturnSwitch`: hold an arrow key for the momentary command; release it to return to center;
- `IlluminatedPushButton`: standard button keyboard behavior.
- `IndustrialDiagram`: arrows select equipment, Enter invokes details, Home
  fits the scene, plus/minus zoom and Escape clears selection. The selected
  element exposes its readout, operational state and alarm acknowledgement in
  automation help text. The diagram draws a visible keyboard-focus border.

## Focus visibility

Interactive Industrial90 controls use the shared `Industrial90.FocusAdorner`.

## Application responsibilities

Applications should:

- provide clear `Title`, `Text`, `SignalName` and `AlarmId` values;
- avoid relying only on color;
- preserve visible state text;
- test with Narrator, NVDA or the platform-equivalent assistive technology;
- verify tab order in the final application layout.
