using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Downloader
{
    public class PartialDownloader : IContentDownloader
    {
        // Could be set dynamically based on connection speed
        public const long WebPartialReadSize = 1 * 1024 * 1024;
        private readonly long _webPartialReadSize;

        private readonly IWebSystemCalls _webSysCalls;

        // Needs to persist in presence of exceptions
        private static Dictionary<(long, long), Stream> _blockDictioary;

        public PartialDownloader(IWebSystemCalls webSystemCalls, long webPartialReadSize = WebPartialReadSize)
        {
            _webSysCalls = webSystemCalls ?? throw new ArgumentException(null, nameof(webSystemCalls));
            _webPartialReadSize = webPartialReadSize;
        }

        public async Task<List<Stream>> DownloadFileAsync(string contentFileUrl, HeaderContentMetadata headerContentMetadata, CancellationToken cancellationToken)
        {
            if (_blockDictioary is null)
            {
                _blockDictioary = GeneratePartialBlockRanges(headerContentMetadata)
                    .Select(x => new KeyValuePair<(long, long), Stream>((x.Item1, x.Item2), null))
                    .ToDictionary(x => x.Key, y => y.Value);
            }

            while (_blockDictioary.Any(kvp => kvp.Value == null))
            {
                var emptyBlock = _blockDictioary.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key);
                
                foreach (var block in emptyBlock)
                {
                    var body = await _webSysCalls.DownloadPartialContent(contentFileUrl, block.Item1, block.Item2, cancellationToken).ConfigureAwait(false);
                    body.EnsureSuccessStatusCode();

                    _blockDictioary[block] = await body.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            return _blockDictioary.Select(kvp => kvp.Value).ToList();
        }

        private IEnumerable<(long, long)> GeneratePartialBlockRanges(HeaderContentMetadata headerContentMetadata)
        {
            long totalNumberOfBlock = ComputeNumberOfBlocks(headerContentMetadata);
            var numberOfEvenBlocks = totalNumberOfBlock - 1;

            for (var idx = 0; idx < numberOfEvenBlocks; ++idx)
            {
                var currentStartPosition = idx * _webPartialReadSize;
                yield return (currentStartPosition, currentStartPosition + _webPartialReadSize);
            }

            // Handle final block which can be of size (0, headerContentMetadata.ContentLength]
            yield return (numberOfEvenBlocks * _webPartialReadSize, headerContentMetadata.ContentLength);
        }

        private long ComputeNumberOfBlocks(HeaderContentMetadata headerContentMetadata)
        {
            return (long)Math.Ceiling((double)headerContentMetadata.ContentLength / _webPartialReadSize);
        }
    }
}
