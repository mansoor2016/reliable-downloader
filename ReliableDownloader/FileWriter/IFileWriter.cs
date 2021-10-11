using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.FileWriter
{
    public interface IFileWriter
    {
        public Task WriteStreamsToFile(List<Stream> contentStreams, string localFilePath, CancellationToken cancellation);
    }
}
