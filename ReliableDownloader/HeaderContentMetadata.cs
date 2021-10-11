using System.Net.Http.Headers;

namespace ReliableDownloader
{
    /// <summary>
    /// Subset of http header response (could be used for validating downloaded content)
    /// </summary>
    public readonly struct HeaderContentMetadata
    {
        public HeaderContentMetadata(long? contentLength, byte[] contentMD5, HttpHeaderValueCollection<string>? acceptRanges)
        {
            this.ContentLength = contentLength ?? 0;
            this.ContentMD5 = contentMD5;
            this.AcceptRanges = acceptRanges ?? null;
        }

        public long ContentLength { get; init; }
        public byte[] ContentMD5 { get; init; }
        public HttpHeaderValueCollection<string>? AcceptRanges { get; init; }

        public bool AllowPartialDownload() => (bool)(AcceptRanges?.ToString().Equals("bytes"));
    }
}
