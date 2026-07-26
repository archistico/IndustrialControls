namespace IndustrialControls.Avalonia.Controls;

internal readonly record struct StripChartRenderDiagnostics(
    int SourceSampleCount,
    int VisibleSampleCount,
    int SelectedPointCount,
    int EstimatedSegmentCount,
    int QualityBreakCount,
    int UncertainPointCount);
