using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader.FileWriter
{
    public class StreamToFileWriter : IFileWriter
    {
        public const long DefaultBufferSize = 4 * 1024 * 1024;
        public const bool OverwriteExistingFileDefault = true;

        private readonly long _writeBufferSize;
        private bool _overwriteExistingFile;

        public StreamToFileWriter(long writeBufferSize = DefaultBufferSize, bool overwriteExistingFile = OverwriteExistingFileDefault)
        {
            _writeBufferSize = writeBufferSize;
            _overwriteExistingFile = overwriteExistingFile;
        }

        public async Task WriteStreamsToFile(List<Stream> contentStreams, string localFilePath, CancellationToken cancellation)
        {
            HandleExistingFile(localFilePath);

            byte[] buffer = new byte[_writeBufferSize];

            long length = 0, totalLength = 0;
            using (var destinationFileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            {
                foreach (var stream in contentStreams)
                {
                    while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalLength += length;
                        await destinationFileStream.WriteAsync(buffer, cancellation).ConfigureAwait(false);
                    }
                }
            }
        }

        private void HandleExistingFile(string localFilePath)
        {
            if (_overwriteExistingFile)
            {
                File.Delete(localFilePath);
            }
        }
    }
}
