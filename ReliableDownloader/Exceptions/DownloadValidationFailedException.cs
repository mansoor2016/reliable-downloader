using System;

namespace ReliableDownloader.Exceptions
{
    class DownloadValidationFailedException : Exception
    {
        public DownloadValidationFailedException()
        {
        }

        public DownloadValidationFailedException(string message)
            : base(message)
        {
        }

        public DownloadValidationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
