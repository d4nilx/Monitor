using Microsoft.Maui.Graphics;

namespace Monitor.Maui;

public class LineChartDrawable : IDrawable
{
    private readonly List<double> _values;
    private readonly Color _lineColor;
    private readonly Color _fillColor;
    private readonly double _maxValue;

    public LineChartDrawable(List<double> values, Color lineColor, Color fillColor, double maxValue)
    {
        _values = values;
        _lineColor = lineColor;
        _fillColor = fillColor;
        _maxValue = maxValue;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_values.Count < 2) return;

        float w = dirtyRect.Width;
        float h = dirtyRect.Height;
        float step = w / (_values.Count - 1);

        // Fill path
        var path = new PathF();
        path.MoveTo(0, h);

        for (int i = 0; i < _values.Count; i++)
        {
            float x = i * step;
            float y = h - (float)(_values[i] / _maxValue * h);
            if (i == 0) path.LineTo(x, y);
            else path.LineTo(x, y);
        }

        path.LineTo(w, h);
        path.Close();

        canvas.FillColor = _fillColor;
        canvas.FillPath(path);

        // Line
        canvas.StrokeColor = _lineColor;
        canvas.StrokeSize = 2;

        var line = new PathF();
        for (int i = 0; i < _values.Count; i++)
        {
            float x = i * step;
            float y = h - (float)(_values[i] / _maxValue * h);
            if (i == 0) line.MoveTo(x, y);
            else line.LineTo(x, y);
        }

        canvas.DrawPath(line);

        // Last value label
        if (_values.Count > 0)
        {
            canvas.FontColor = _lineColor;
            canvas.FontSize = 11;
            canvas.DrawString($"{_values[^1]:F1}", w - 40, 4, 40, 16, HorizontalAlignment.Right, VerticalAlignment.Top);
        }
    }
}
