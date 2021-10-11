using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.Downloader
{
    public interface IContentDownloader
    {
        public Task<List<Stream>> DownloadFileAsync(string contentFileUrl, HeaderContentMetadata headerContentMetadata, CancellationToken cancellationToken);
    }
}
