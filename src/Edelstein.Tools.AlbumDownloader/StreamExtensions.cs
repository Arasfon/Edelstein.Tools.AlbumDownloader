using System.Buffers;

namespace Edelstein.Tools.AlbumDownloader;

public static class StreamExtensions
{
    public static async Task CopyToWithProgressAsync(this Stream source, Stream destination, long? length,
        IProgress<double> progressPercentage, CancellationToken cancellationToken = default)
    {
        if (!length.HasValue)
        {
            await source.CopyToAsync(destination, cancellationToken);
            return;
        }

        const int bufferSize = 81920;

        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                progressPercentage.Report((double)bytesRead / length.Value * 100);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}