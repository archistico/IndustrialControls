# Performance and memory

## Bounded buffers

`TrendChart` and `StripChartRecorder` retain at most `MaxSamplesPerSeries` samples for each series.

`OscilloscopeDisplay` retains at most `MaxSamples` values.

When a capacity is reduced, existing buffers are trimmed immediately.

## Rendering density

`TrendChart` decimates only the visual sample set when the visible density exceeds the available horizontal pixels. Stored data remains bounded by the configured capacity.

## Benchmark smoke suite

```powershell
.\scripts\benchmark.ps1
```

The suite measures:

- bounded trend-buffer ingestion;
- gauge state updates;
- selector state transitions.

The benchmark is diagnostic rather than a hard timing gate because timing varies by hardware and runtime environment.


## RC2 allocation optimization

The RC2 hot paths use:

- circular sample buffers with O(1) overwrite;
- dictionary-based series lookup;
- optional direct-series handles;
- lazy cursor readout generation;
- cached brushes and pens;
- cached selector labels;
- coalesced accessibility updates for rapidly changing values.

The benchmark reports both total allocation and allocation per operation. Results must be compared on the same machine, runtime and build configuration.


## RC6-C rendering hardening

### LED marquee

The marquee caches the complete scroll source. A timer tick:

- advances an integer offset;
- fills a reusable character buffer;
- creates only the new displayed string required by the Avalonia property.

Source padding is rebuilt only when text, visible capacity or end pause changes.

### Strip-chart recorder

Dense series are decimated according to plot width. The renderer:

- avoids LINQ;
- reuses frame, paper, grid and trace pens;
- preserves uncertain points and bad/unavailable discontinuities;
- limits ordinary good-quality points to approximately one per horizontal
  pixel.

The benchmark includes a 100,000-sample render-plan diagnostic.


### Quality-aware render diagnostics

`StripChartRenderDiagnostics` distinguishes:

- selected drawable points;
- estimated drawable segments;
- `Bad`/`Unavailable` quality breaks;
- uncertain points retained by the decimator.

This prevents point-count assumptions from being used as a proxy for trace
continuity.


## Final strip-chart scale contract

`TimeWindowSeconds` is the authoritative horizontal time span and
`MajorGridSeconds` is the authoritative temporal grid interval. No physical
screen-millimetre conversion is claimed by the control.
