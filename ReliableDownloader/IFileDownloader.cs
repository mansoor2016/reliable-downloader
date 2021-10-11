using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    public interface IFileDownloader
    {
        public CancellationToken CancellationToken { get; }

        /// <summary>Downloads a file, trying to use reliable downloading if possible</summary>
        /// <param name="contentFileUrl">The url which the file is hosted at</param>
        /// <param name="localFilePath">The local file path to save the file to</param>
        /// <returns>True or false, depending on if download completes and writes to file system okay</returns>
        Task DownloadFileAsync(string contentFileUrl, string localFilePath);
        
        /// <summary>
        /// Cancels any in progress downloads
        /// </summary>
        void CancelDownloadsAsync();
    }
}