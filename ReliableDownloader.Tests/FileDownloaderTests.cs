using NUnit.Framework;
using Moq;

using ReliableDownloader.Downloader;
using ReliableDownloader.FileWriter;
using ReliableDownloader.RetryPolicy;

using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;


// ToDO: Test all code paths through interface
namespace ReliableDownloader.Tests
{
    [TestFixture]
    public class FileDownloaderTests
    {
        IFileDownloader _fileDownloader;

        Mock<IWebSystemCalls> m_webSysCalls;
        Mock<IContentDownloader> m_contentDownloader;
        Mock<IFileWriter> m_fileWriter;
        Mock<IRetryPolicy> m_retryPolicy;

        [SetUp]
        public void Setup()
        {
            m_webSysCalls = new Mock<IWebSystemCalls>();
            m_contentDownloader = new Mock<IContentDownloader>();
            m_fileWriter = new Mock<IFileWriter>();
            m_retryPolicy = new Mock<IRetryPolicy>();
        }

        [Test]
        public void TestRequestCancellation()
        {
            _fileDownloader = new FileDownloader(m_webSysCalls.Object, m_contentDownloader.Object, m_fileWriter.Object, m_retryPolicy.Object);

            _fileDownloader.CancelDownloadsAsync();

            Assert.True(_fileDownloader.CancellationToken.IsCancellationRequested);

            _fileDownloader.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>());

            m_retryPolicy.Verify(x => x.InvokeAsync(It.IsAny<Func<Task>>()), Times.Never);
        }

        [Test]
        public void TestHappyPathWithSingleShotDownloader()
        {
            m_webSysCalls.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage()));

            m_webSysCalls.Setup(x => x.DownloadContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage()));

            // No need to mock setup RetryPolicy because setup lambda would mimic implementation.
            _fileDownloader = new FileDownloader(m_webSysCalls.Object, m_contentDownloader.Object, m_fileWriter.Object, new ReliableDownloader.RetryPolicy.NoRetry());

            _fileDownloader.DownloadFileAsync("dummy_url", Path.GetTempPath());

            m_fileWriter.Verify(x => x.WriteStreamsToFile(It.IsAny<List<Stream>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}