using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WoWFrameTools;

public ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;

    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        Length = 0;
    }

    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = (Span<char>)_arrayToReturnToPool;
        Length = 0;
    }

    public int Length { get; set; }

    public int Capacity => _chars.Length;

    public void EnsureCapacity(int capacity)
    {
        if ((uint)capacity <= (uint)_chars.Length)
            return;
        Grow(capacity - Length);
    }

    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = char.MinValue;
        }

        return ref MemoryMarshal.GetReference(_chars);
    }

    public ref char this[int index] => ref _chars[index];

    public override string ToString()
    {
        var str = _chars.Slice(0, Length).ToString();
        Dispose();
        return str;
    }

    public Span<char> RawChars => _chars;

    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = char.MinValue;
        }

        return _chars.Slice(0, Length);
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return _chars.Slice(0, Length);
    }

    public ReadOnlySpan<char> AsSpan(int start)
    {
        return _chars.Slice(start, Length - start);
    }

    public ReadOnlySpan<char> AsSpan(int start, int length)
    {
        return _chars.Slice(start, length);
    }

    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars.Slice(0, Length).TryCopyTo(destination))
        {
            charsWritten = Length;
            Dispose();
            return true;
        }

        charsWritten = 0;
        Dispose();
        return false;
    }

    public void Insert(int index, char value, int count)
    {
        if (Length > _chars.Length - count)
            Grow(count);
        var length = Length - index;
        _chars.Slice(index, length).CopyTo(_chars.Slice(index + count));
        _chars.Slice(index, count).Fill(value);
        Length += count;
    }

    public void Insert(int index, string? s)
    {
        if (s == null)
            return;
        var length1 = s.Length;
        if (Length > _chars.Length - length1)
            Grow(length1);
        var length2 = Length - index;
        _chars.Slice(index, length2).CopyTo(_chars.Slice(index + length1));
        s.CopyTo(_chars.Slice(index));
        Length += length1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = Length;
        var chars = _chars;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            Length = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s == null)
            return;
        var pos = Length;
        if (s.Length == 1 && (uint)pos < (uint)_chars.Length)
        {
            _chars[pos] = s[0];
            Length = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = Length;
        if (pos > _chars.Length - s.Length)
            Grow(s.Length);
        s.CopyTo(_chars.Slice(pos));
        Length += s.Length;
    }

    public void Append(char c, int count)
    {
        if (Length > _chars.Length - count)
            Grow(count);
        var span = _chars.Slice(Length, count);
        for (var index = 0; index < span.Length; ++index)
            span[index] = c;
        Length += count;
    }

    public unsafe void Append(char* value, int length)
    {
        if (Length > _chars.Length - length)
            Grow(length);
        var span = _chars.Slice(Length, length);
        for (var index = 0; index < span.Length; ++index)
            span[index] = *value++;
        Length += length;
    }

    public void Append(scoped ReadOnlySpan<char> value)
    {
        if (Length > _chars.Length - value.Length)
            Grow(value.Length);
        value.CopyTo(_chars.Slice(Length));
        Length += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var pos = Length;
        if (pos > _chars.Length - length)
            Grow(length);
        Length = pos + length;
        return _chars.Slice(pos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        var destination = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(Length + additionalCapacityBeyondPos), Math.Min((uint)(_chars.Length * 2), 2147483591U)));
        _chars.Slice(0, Length).CopyTo((Span<char>)destination);
        var arrayToReturnToPool = _arrayToReturnToPool;
        _chars = (Span<char>)(_arrayToReturnToPool = destination);
        if (arrayToReturnToPool == null)
            return;
        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var arrayToReturnToPool = _arrayToReturnToPool;

        // Reset the internal state of ValueStringBuilder
        _arrayToReturnToPool = null;
        Length = 0;

        // Return the array to the pool if it exists
        if (arrayToReturnToPool != null) ArrayPool<char>.Shared.Return(arrayToReturnToPool);
    }

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Dispose()
    {
      char[] arrayToReturnToPool = this._arrayToReturnToPool;
      *(ValueStringBuilder*) ref this = new ValueStringBuilder();
      if (arrayToReturnToPool == null)
        return;
      ArrayPool<char>.Shared.Return(arrayToReturnToPool);
    }
    */
}