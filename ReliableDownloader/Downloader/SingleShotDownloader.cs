using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Downloader
{
    public class SingleShotDownloader : IContentDownloader
    {
        private readonly IWebSystemCalls _webSysCalls;

        public SingleShotDownloader(IWebSystemCalls webSystemCalls)
        {
            _webSysCalls = webSystemCalls ?? throw new ArgumentException(null, nameof(webSystemCalls));
        }

        public async Task<List<Stream>> DownloadFileAsync(string contentFileUrl, HeaderContentMetadata headerContentMetadata, CancellationToken cancellationToken)
        {
            var body = await _webSysCalls.DownloadContent(contentFileUrl, cancellationToken).ConfigureAwait(false);
            body.EnsureSuccessStatusCode();

            return new List<Stream>() { await body.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false) };
        }
    }
}
