using Avalonia;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Diagrams;

/// <summary>Optional equipment symbol extension. Return false to use the built-in symbol.</summary>
public interface IDiagramSymbolRenderer
{
    bool Render(DrawingContext context, DiagramNode node, Rect iconBounds, IBrush stateBrush);
}
