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
