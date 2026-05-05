namespace HappreeTool.Surfers.Configurations
{
    public class HappreeServersSettings
    {
        public BrowserPioneerSettigns BrowserPioneer { get; set; } = default!;
    }

    public class BrowserPioneerSettigns
    {
        public string WebpageContent { get; set; } = default!;
        public string DownloadImage { get; set; } = default!;
    }
}
