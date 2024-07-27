using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HappreeTool.Surfers
{
    public static class WebDriverFactory
    {

        public static ChromeDriver CreateChromeDriver(string binaryLocation, string driverPath, bool needProxy, string proxyServer)
        {
            // 创建 ChromeOptions 实例
            ChromeOptions options = new ChromeOptions();

            // 设置 Chrome 的安装路径
            options.BinaryLocation = binaryLocation;
            // 设置页面加载策略
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            // 修改 User-Agent
            //options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            // 避免 WebDriver 被检测的设置
            options.AddExcludedArgument("enable-automation");

            // 启用隐身模式
            options.AddArgument("--incognito");

            // 使用代理
            if (needProxy)
            {
                options.AddArgument($"--proxy-server={proxyServer}");
            }

            // 禁用扩展
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-gpu");

            // 关闭自动化提示
            options.AddArgument("--disable-blink-features=AutomationControlled");

            // 禁止加载图像
            options.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

            // 指定 chromedriver 的路径
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);

            // 创建 ChromeDriver 实例
            ChromeDriver driver = new ChromeDriver(service, options);

            // 页面加载超时 20 秒
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

            // 设置隐式等待时间
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // 执行 JavaScript 代码来覆盖 navigator.webdriver 属性
            driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

            return driver;
        }

        public static ChromeDriver AttachToExistingChromeInstance(string driverPath, string debuggerAddress)
        {
            // 创建 ChromeOptions 实例
            ChromeOptions options = new ChromeOptions();
            options.DebuggerAddress = debuggerAddress; // 设置远程调试端口

            // 指定 chromedriver 的路径
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
            //service.EnableVerboseLogging = true;
            //service.SuppressInitialDiagnosticInformation = true;

            // 创建 ChromeDriver 实例
            ChromeDriver driver = new ChromeDriver(service, options);

            // 页面加载超时 20 秒
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

            // 设置隐式等待时间
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            return driver;
        }

    }
}
