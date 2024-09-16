using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace WebScrapingWinForms
{
    public partial class WebScrapingLogic
    {
        private static readonly HttpClient client = new()
        {
            DefaultRequestHeaders =
            {
                { "Accept-Language", "en-US,en;q=0.9" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36" },
                { "Accept-Encoding", "gzip, deflate" }
            }
        };

        private readonly ILogger _logger;

        public WebScrapingLogic(ILogger<WebScrapingLogic> logger)
        {
            _logger = logger;
        }

        public async Task ProcessMultipleVideosAsync(List<string> videoUrls)
        {
            for (int i = 0; i < videoUrls.Count; i++)
            {
                var lne = videoUrls[i].Replace("\r", "");
                if (string.IsNullOrWhiteSpace(lne))
                    continue;

                if (!lne.Contains("instagram") && !lne.Contains("facebook"))
                {
                    Process.Start(new ProcessStartInfo("https://godownloader.com/#link=" + lne) { UseShellExecute = true });
                    Process.Start(new ProcessStartInfo("https://snaptik.app/") { UseShellExecute = true });
                    Clipboard.SetText(lne);
                    await MainAsync(lne);
                }
                else if (lne.Contains("instagram"))
                {
                    Process.Start(new ProcessStartInfo("https://snapinsta.app/") { UseShellExecute = true });
                    Clipboard.SetText(lne);
                    MessageBox.Show(lne);
                }
                else if (lne.Contains("facebook"))
                {
                    string lneFacebook = lne.Replace("www.facebook", "mbasic.facebook");
                    Process.Start(new ProcessStartInfo(lneFacebook) { UseShellExecute = true });
                    Clipboard.SetText(lne);
                    MessageBox.Show(lne);
                }

                _logger.LogInformation("Proceso completado para el video {Index}", i + 1);
                MessageBox.Show("Done");
            }
        }

        public async Task MainAsync(string videoUrl)
        {
            try
            {
                videoUrl = videoUrl.Replace("\r", "");

                using var driver = await SetupDriverAsync();
                await driver.NavigateAsync(videoUrl);
                await Task.Delay(TimeSpan.FromSeconds(10));

                await CloseCaptchaIfPresentAsync(driver);

                string fullDescription = await GetFullDescription(driver);
                _logger.LogInformation("Descripción completa extraída: {Description}", fullDescription);

                var videoInfo = await ExtractVideoInfoAsync(driver);
                LogVideoInfo(videoInfo);

                _logger.LogInformation("Proceso de scraping completado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado durante el scraping de la URL: {Url}", videoUrl);
            }
        }

        [GeneratedRegex(@"/video/(\d+)")]
        private static partial Regex VideoIdRegex();

        private async Task<VideoInfo> ExtractVideoInfoAsync(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var videoInfo = new VideoInfo();

            try
            {
                videoInfo.Username = await TryGetElementText(wait, "Username", By.CssSelector("[data-e2e='browse-username']"));
                videoInfo.FullName = await TryGetElementText(wait, "Full Name", By.CssSelector("[data-e2e='browser-nickname'] .css-1xccqfx-SpanNickName"));
                var fullInfo = await TryGetElementText(wait, "Publish Date", By.CssSelector("[data-e2e='browser-nickname']"));
                videoInfo.PublishDate = fullInfo.Split('·').Last().Trim();
                videoInfo.Likes = await TryGetElementText(wait, "Likes", By.CssSelector("[data-e2e='like-count']"));
                videoInfo.Comments = await TryGetElementText(wait, "Comments", By.CssSelector("[data-e2e='comment-count']"));
                videoInfo.Shares = await TryGetElementText(wait, "Shares", By.CssSelector("[data-e2e='share-count']"));
                videoInfo.Description = await TryGetElementText(wait, "Description", By.XPath("//*[@id='main-content-video_detail']/div/div[2]/div[1]/div[1]/div[2]/div[2]/div[1]/div/h1"));
                videoInfo.Tags = ExtractTagsFromDescription(videoInfo.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante la extracción de información del video");
            }

            return videoInfo;
        }

        private async Task<string> GetFullDescription(IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var jsExecutor = (IJavaScriptExecutor)driver;
                jsExecutor.ExecuteScript("window.scrollBy(0, 400);");
                await Task.Delay(1000);

                try
                {
                    var moreButton = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"main-content-video_detail\"]/div/div[2]/div/div[1]/div[2]/div[2]/div[1]/div/div")));
                    if (moreButton != null && moreButton.Displayed)
                    {
                        moreButton.Click();
                        await Task.Delay(1000);
                    }
                }
                catch (NoSuchElementException)
                {
                    _logger.LogInformation("El botón 'Más' no se encontró. La descripción ya estaba completa.");
                }

                var descriptionElement = wait.Until(d => d.FindElement(By.XPath("//*[@id='main-content-video_detail']/div/div[2]/div[1]/div[1]/div[2]/div[2]/div[1]/div/h1")));
                return descriptionElement.Text;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener la descripción completa: {ErrorMessage}", ex.Message);
                return "Descripción no encontrada.";
            }
        }

        private List<string> ExtractTagsFromDescription(string description)
        {
            return Regex.Matches(description, @"#(\w+)")
                        .Cast<Match>()
                        .Select(m => m.Groups[1].Value)
                        .ToList();
        }

        private async Task<IWebDriver> SetupDriverAsync()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            return await Task.FromResult(new ChromeDriver());
        }

        private async Task CloseCaptchaIfPresentAsync(IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var closeButton = await Task.Run(() => wait.Until(d => d.FindElement(By.XPath("//*[@id='verify-bar-close']"))));

                if (closeButton != null && closeButton.Displayed)
                {
                    await Task.Run(() => closeButton.Click());
                    _logger.LogInformation("Captcha cerrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error al manejar el captcha: {ErrorMessage}", ex.Message);
            }
        }

        private async Task<string> TryGetElementText(WebDriverWait wait, string fieldName, By locator)
        {
            try
            {
                var element = await Task.Run(() => wait.Until(d => d.FindElement(locator)));
                var text = await Task.Run(() => element.Text);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogInformation("Successfully extracted {FieldName}: {Text}", fieldName, text);
                    return text;
                }
            }
            catch (WebDriverTimeoutException)
            {
                _logger.LogWarning("{FieldName} not found with locator: {Locator}", fieldName, locator);
            }
            catch (NoSuchElementException)
            {
                _logger.LogWarning("{FieldName} not found with locator: {Locator}", fieldName, locator);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception: {FieldName}", ex.Message);
            }
            return "Not found";
        }

        private void LogVideoInfo(VideoInfo info)
        {
            _logger.LogInformation("Username: {Username}", info.Username);
            _logger.LogInformation("Nombre Completo: {FullName}", info.FullName);
            _logger.LogInformation("Fecha de Publicación: {PublishDate}", info.PublishDate);
            _logger.LogInformation("Likes: {Likes}", info.Likes);
            _logger.LogInformation("Comentarios: {Comments}", info.Comments);
            _logger.LogInformation("Compartidos: {Shares}", info.Shares);
            _logger.LogInformation("Descripción: {Description}", info.Description);
            _logger.LogInformation("Etiquetas: {Tags}", string.Join(", ", info.Tags));
        }
    }

    public static class WebDriverExtensions
    {
        public static async Task NavigateAsync(this IWebDriver driver, string url)
        {
            await Task.Run(() => driver.Navigate().GoToUrl(url));
        }
    }

    public class VideoInfo
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PublishDate { get; set; } = string.Empty;
        public string Likes { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Shares { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class Comment
    {
        public string Text { get; set; } = string.Empty;
        public string CommentLanguage { get; set; } = string.Empty;
        public int DiggCount { get; set; }
        public int ReplyCommentTotal { get; set; }
        public bool AuthorPin { get; set; }
        public long CreateTime { get; set; }
        public string Cid { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
        public string AwemeId { get; set; } = string.Empty;
    }
}
