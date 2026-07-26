using System.Collections;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Buffer circolare a capacità fissa che espone i campioni in ordine cronologico.
/// </summary>
internal sealed class BoundedSampleCollection : IReadOnlyList<SignalSample>
{
    private SignalSample[] _buffer = Array.Empty<SignalSample>();
    private int _start;
    private int _count;

    public int Count => _count;

    public SignalSample this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _buffer[(_start + index) % _buffer.Length];
        }
    }

    public void Add(
        SignalSample sample,
        int capacity)
    {
        EnsureCapacity(capacity);

        if (_count < _buffer.Length)
        {
            var targetIndex = (_start + _count) % _buffer.Length;
            _buffer[targetIndex] = sample;
            _count++;
            return;
        }

        _buffer[_start] = sample;
        _start = (_start + 1) % _buffer.Length;
    }

    public void TrimToCapacity(int capacity) =>
        EnsureCapacity(capacity);

    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        _start = 0;
        _count = 0;
    }

    public IEnumerator<SignalSample> GetEnumerator()
    {
        for (var index = 0; index < _count; index++)
        {
            yield return this[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EnsureCapacity(int requestedCapacity)
    {
        var capacity = Math.Max(1, requestedCapacity);
        if (_buffer.Length == capacity)
        {
            return;
        }

        var retainedCount = Math.Min(_count, capacity);
        var firstRetainedIndex = _count - retainedCount;
        var replacement = new SignalSample[capacity];

        for (var index = 0; index < retainedCount; index++)
        {
            replacement[index] = this[firstRetainedIndex + index];
        }

        _buffer = replacement;
        _start = 0;
        _count = retainedCount;
    }
}
