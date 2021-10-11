using System;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using ReliableDownloader.Downloader;
using ReliableDownloader.FileWriter;
using ReliableDownloader.RetryPolicy;

namespace ReliableDownloader
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder().AddCommandLine(args);
                var config = builder.Build();

                var inputSanitiser = new InputSanitiser(config);
                var exampleUrl = inputSanitiser.ReturnSanitisedInput("url");
                var exampleFilePath = inputSanitiser.ReturnSanitisedInput("filename");

                var webSystemCalls = new WebSystemCalls();
                var fileDownloader = new FileDownloader(webSystemCalls, new PartialDownloader(webSystemCalls), new StreamToFileWriter(), new RetryForeverWithIncreasingWait());

                await fileDownloader.DownloadFileAsync(exampleUrl, exampleFilePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Terminating application: {Assembly.GetExecutingAssembly()}.");
                Console.Error.WriteLine($"Unexpected exception encountered: {ex}.");
            }
        }
    }
}