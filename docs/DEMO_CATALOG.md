# Demo catalog

The demo is the visual acceptance catalog for the library.

## Tabs

1. Foundation
2. Lamps & LED
3. Gauges
4. Operator controls
5. Trends & screens
6. Alarm indicators
7. Static & release

## Startup diagnostics

Unexpected startup failures are written to:

```text
%LOCALAPPDATA%\IndustrialControls.Avalonia.Demo\startup-error.log
```

If application initialization succeeds but the main catalog window cannot be
constructed, the demo opens a fallback diagnostic window instead of silently
terminating.

## Acceptance gate

- open every tab;
- verify scrolling and resizing;
- operate keyboard-focusable controls;
- apply and clear operator interlocks;
- pause and resume the recorder;
- cycle signal quality;
- raise, acknowledge, clear and reset alarms.


## Lamps & LED RC4 checks

- resize the window while the marquee is running;
- verify that the next message enters from the right LED edge;
- verify that legacy annunciator lenses remain circular;
- verify the neutral grey/black annunciator palette;
- move the pointer over buttons, knobs and switches and verify the hand cursor.
