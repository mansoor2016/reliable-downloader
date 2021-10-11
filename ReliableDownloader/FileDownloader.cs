using System;
using System.Threading;
using System.Threading.Tasks;
using ReliableDownloader.Downloader;
using ReliableDownloader.FileWriter;
using ReliableDownloader.RetryPolicy;

namespace ReliableDownloader
{
    public class FileDownloader : IFileDownloader
    {
        private readonly IWebSystemCalls _webSysCalls;
        private IContentDownloader _fileDownloader;
        private readonly IFileWriter _fileWriter;
        private readonly IRetryPolicy _retryPolicy;

        private readonly CancellationTokenSource cancellationTokenSource;
        
        public CancellationToken CancellationToken { get; private set; }

        public FileDownloader(IWebSystemCalls webSysCalls, IContentDownloader fileDownloader, IFileWriter filewriter, IRetryPolicy retryPolicy)
        {
            _webSysCalls = webSysCalls ?? throw new ArgumentException(null, nameof(webSysCalls));
            _fileDownloader = fileDownloader ?? throw new ArgumentException(null, nameof(fileDownloader));
            _fileWriter = filewriter ?? throw new ArgumentException(null, nameof(filewriter));
            _retryPolicy = retryPolicy ?? throw new ArgumentException(null, nameof(retryPolicy));

            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = cancellationTokenSource.Token;
        }

        public async Task DownloadFileAsync(string contentFileUrl, string localFilePath)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _retryPolicy.InvokeAsync(async () =>
                  {
                      var headerContentMetadata = await GetHeadersAsync(contentFileUrl, CancellationToken).ConfigureAwait(false);

                      if (!headerContentMetadata.AllowPartialDownload())
                      {
                          // Fallback strategy if partial content download is not enabled
                          _fileDownloader = new SingleShotDownloader(_webSysCalls);
                      }

                      var contentStream = await _fileDownloader.DownloadFileAsync(contentFileUrl, headerContentMetadata, CancellationToken).ConfigureAwait(false);

                      // ToDo: Implement validation of downloaded data (check MD5 hash)

                      await _fileWriter.WriteStreamsToFile(contentStream, localFilePath, CancellationToken);
                  }
            );
        }

        public void CancelDownloadsAsync()
        {
            if (CancellationToken.CanBeCanceled)
            {
                cancellationTokenSource.Cancel();
                
                Console.WriteLine("Cancellation requested.");
            }
        }

        private async Task<HeaderContentMetadata> GetHeadersAsync(string contentFileUrl, CancellationToken cancellationToken)
        {
            var header = await _webSysCalls.GetHeadersAsync(contentFileUrl, cancellationToken).ConfigureAwait(false);
            header?.EnsureSuccessStatusCode();

            return new HeaderContentMetadata(
                header.Content.Headers.ContentLength,
                header.Content.Headers.ContentMD5,
                header.Headers.AcceptRanges);
        }
    }
}