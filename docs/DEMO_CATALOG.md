# Demo catalog

The demo is the visual acceptance catalog for the library.

## Tabs

1. Architecture diagram — plant overview, animated media flows, selection,
   node positioning, synthetic fault/ACK and versioned layout round trip.
2. Foundation
3. Lamps & LED
4. Gauges
5. Operator controls
6. Trends & screens
7. Alarm indicators
8. Static & release

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
