using HappreeTool.Surfers.Configurations;
using HappreeTool.Utils.HttpUtils;
using Microsoft.Extensions.Options;

namespace HappreeTool.Surfers
{
    public class BrowserPioneer(IOptions<BrowserPioneerSettigns> settigns,
                                HttpWrapper httpWrapper)
    {
        private readonly string remoteDownloadServer = settigns.Value.DownloadImage;
        private readonly string webpageContentServer = settigns.Value.WebpageContent;

        public async Task<byte[]> DownloadImageFromWebsitePageAsync(string imageXPath,
                                                                    string? pageUrl,
                                                                    byte downCount = 8)
        {
            RemoteImageDownloadRequest command = new RemoteImageDownloadRequest
            {
                PageUrl = pageUrl,
                ImageXPath = imageXPath,
                DownCount = downCount,
            };

            HttpResponseMessage response = await httpWrapper.PostJsonAsync(remoteDownloadServer, command);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        internal class RemoteImageDownloadRequest
        {
            public string ImageXPath { get; set; } = default!;

            public string? PageUrl { get; set; }

            public byte DownCount { get; set; }
        }

        public async Task<string> GetWebpageContentAsync(string pageUrl)
        {
            RemoteWebpageContentRequest request = new RemoteWebpageContentRequest
            {
                PageUrl = pageUrl
            };

            HttpResponseMessage response = await httpWrapper.PostJsonAsync(webpageContentServer, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        internal class RemoteWebpageContentRequest
        {
            public string PageUrl { get; set; } = default!;
        }
    }
}