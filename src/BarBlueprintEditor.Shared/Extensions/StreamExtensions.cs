namespace BarBlueprintEditor.Shared.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Reads the entire stream into a byte[] asynchronously.
    /// When the stream supports seeking and Length is known → direct allocation + single ReadExactlyAsync (very efficient).
    /// Otherwise, falls back to streaming copy (MemoryStream).
    /// </summary>
    public static async Task<byte[]> ToByteArrayAsync(
        this Stream stream,
        CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream is not readable.", nameof(stream));

        // Reset position if possible (common pattern)
        if (stream.CanSeek) 
            stream.Position = 0;

        // ── Fast path: known length ─────────────────────────────────────
        if (stream is { CanSeek: true, Length: >= 0 and <= int.MaxValue })
        {
            var length = (int)stream.Length;
            var buffer = new byte[length];
            await stream.ReadExactlyAsync(buffer, 0, length, cancellationToken);
            return buffer;
        }

        // ── General path: unknown length or too large for single array ──
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, 81920, cancellationToken); // 80 KB buffer is good default
        return memoryStream.ToArray();
    }

    // Optional: sync version with same logic (if you still need it)
    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead)
            throw new ArgumentException("Stream is not readable.", nameof(stream));

        if (stream.CanSeek) 
            stream.Position = 0;

        if (stream is { CanSeek: true, Length: >= 0 and <= int.MaxValue })
        {
            var length = (int)stream.Length;
            var buffer = new byte[length];
            var read = stream.Read(buffer, 0, length);
            if (read != length)
                throw new EndOfStreamException($"Expected {length} bytes, but read only {read}.");

            return buffer;
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}