# Reliable Downloader

## Summary
Simple command line application for reliably downloading web resource. Resilience is provided through retry policies, two implemented at present:
- NoRetry: No resilience, if connection fails, the process will end.
- RetryForeverWithIncreasingWait: Will retry forever with increase time between retries.

## Arguments:
url: URL link to resource for downloading

filename: Specify the filename or filepath, on the local machine, to save the downloaded file to.

Example:

./ReliableDownloader --url "https://installerstaging.accurx.com/chain/3.55.11050.0/accuRx.Installer.Local.msi" --filename "myfirstdonwload.msi"
