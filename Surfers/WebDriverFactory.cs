using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace HappreeTool.Surfers
{
    public class WebDriverFactory : IDisposable
    {
        private readonly ConcurrentDictionary<string, ChromeDriver> _driverCache = new ConcurrentDictionary<string, ChromeDriver>();

        /// <summary>
        /// 创建或附加到一个 Chrome 浏览器实例，并通过 ChromeDriver 控制它。如果指定的远程调试端口已经被使用，则附加到该实例。
        /// </summary>
        /// <param name="chromeLocation">chrome exe的路径</param>
        /// <param name="driverPath">chrome driver的路径</param>
        /// <param name="remoteDebuggingPort">远程调试端口，默认为 9222</param>
        /// <param name="needProxy">是否需要代理</param>
        /// <param name="proxyServer">代理服务器地址，例如 http://127.0.0.1:7890</param>
        /// <returns>一个 ChromeDriver 实例</returns>
        public ChromeDriver GetOrCreateChromeDriver(string chromeLocation, string driverPath,
            int remoteDebuggingPort = 9222, bool needProxy = false, string? proxyServer = "http://127.0.0.1:7890")
        {
            string debuggerAddress = $"127.0.0.1:{remoteDebuggingPort}";

            // 先检查缓存中是否已有该端口对应的 ChromeDriver 实例
            if (_driverCache.TryGetValue(debuggerAddress, out ChromeDriver? existingDriver))
            {
                return existingDriver; // 如果已经存在指定端口的实例，返回它
            }

            // 检查指定的端口是否被使用
            if (IsPortInUse("127.0.0.1", remoteDebuggingPort))
            {
                // 如果指定端口被使用，附加到已有的 Chrome 实例
                ChromeOptions options = new ChromeOptions();
                options.DebuggerAddress = debuggerAddress;

                // 指定 chromedriver 的路径
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
                var driver = new ChromeDriver(service, options);

                // 将附加的实例添加到缓存中
                _driverCache[debuggerAddress] = driver;

                return driver;
            }
            else
            {
                // 否则创建一个新的 Chrome 实例并开启调试模式
                ChromeOptions options = new ChromeOptions();
                options.BinaryLocation = chromeLocation;

                options.PageLoadStrategy = PageLoadStrategy.Eager;

                // 设置远程调试端口
                options.AddArgument($"--remote-debugging-port={remoteDebuggingPort}");

                // 避免 WebDriver 被检测的设置
                options.AddExcludedArgument("enable-automation");
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

                // 指定 chromedriver 的路径
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);

                var driver = new ChromeDriver(service, options);

                // 页面加载超时 20 秒
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

                // 设置隐式等待时间
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                // 将新创建的实例添加到缓存中
                _driverCache[debuggerAddress] = driver;

                return driver;
            }
        }

        /// <summary>
        /// 检查指定的端口是否正在使用。
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口号</param>
        /// <returns>如果端口正在使用，则返回 true，否则返回 false</returns>
        private bool IsPortInUse(string host, int port)
        {
            try
            {
                using (var client = new TcpClient(host, port))
                {
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            foreach (var driver in _driverCache.Values)
            {
                driver.Quit();
                driver.Dispose();
            }
            _driverCache.Clear();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services)
        {
            services.AddSingleton<WebDriverFactory>();
            return services;
        }
    }
}
