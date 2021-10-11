using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ReliableDownloader
{
    /// <summary>
    /// Quick attempt at sanitising program inputs.
    /// </summary>
    public class InputSanitiser
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<string, Func<string, string>> _mapInputToSanitiser;

        public InputSanitiser(IConfiguration config)
        {
            _config = config ?? throw new ArgumentException(null, nameof(config));
            _mapInputToSanitiser = new Dictionary<string, Func<string, string>>()
            {
                { "url", SanitiseURLInput },
                { "filename", SanitiseFilepathInput}
            };
        }

        public string ReturnSanitisedInput(string input) =>
            _mapInputToSanitiser[input].Invoke(_config[input]);

        private static string SanitiseURLInput(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL input was null or empty.");
            }

            url = url.Trim();

            // ToDo: implement URL santiser (simple regex)

            return url;
        }

        // Naive attempt to catch bad filepath inputs
        private static string SanitiseFilepathInput(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                throw new ArgumentException("Filepath input was null or empty.");
            }

            filepath = filepath.Trim();

            var directory = Path.GetDirectoryName(filepath);
            var filename = Path.GetFileName(filepath);

            if (string.IsNullOrWhiteSpace(directory))
            {
                filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    filename);
            }
            else if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Could not locate: {directory}");
            }

            if (Path.GetInvalidPathChars().Any(c => filepath.Contains(c)) ||
                Path.GetInvalidFileNameChars().Any(c => filename.Contains(c)))
            {
                throw new ArgumentOutOfRangeException($"Filepath provided: \"{filepath}\" contains an invalid character.");
            }

            return filepath;
        }
    }
}
