using FabProject.Models;
using System.Text.Json;

namespace FabProject
{
    internal class Program
    {
        private static string? _csrfToken;
        private static string? _sessionId;
        private static string? _optanonAlertBoxClosed;
        private static string? _optanonConsent;
        private static string? _clearance;
        private static string? _cfBm;

        private static string? _cookie;

        private static HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://www.fab.com";

        private static async Task Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.WriteLine("Wrong arguments!");
                return;
            }
            _csrfToken = args[0];
            _sessionId = args[1];
            _optanonAlertBoxClosed = args[2];
            _optanonConsent = args[3];
            _clearance = args[4];
            _cfBm = args[5];


            _cookie = $"sb_csrftoken={_csrfToken}; " +
                        $"sb_sessionid={_sessionId}; " +
                        $"OptanonAlertBoxClosed={_optanonAlertBoxClosed}; " +
                        $"OptanonConsent={_optanonConsent}; " +
                        $"cf_clearance={_clearance}; " +
                        $"__cf_bm={_cfBm}";

            Console.WriteLine($"Token: {_csrfToken}");
            Console.WriteLine($"Cookie: {_cookie}{Environment.NewLine}{Environment.NewLine}");

            SetHeaders();
            await ParseAllModelsAsync();
        }

        private static void SetHeaders()
        {
            //_httpClient.DefaultRequestHeaders.Add("X-CsrfToken", _csrfToken);
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"130\", \"Google Chrome\";v=\"130\", \"Not?A_Brand\";v=\"99\"");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8,uk;q=0.7,ro;q=0.6");

            //_httpClient.DefaultRequestHeaders.Add("Cookie", _cookie);
        }

        private static async Task<bool> ParseAllModelsAsync()
        {
            string? url = $"{BaseUrl}/i/listings/search?currency=USD&seller=Quixel";

            do
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(jsonResponse))
                    return false;

                ApiResponse? data = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
                if (data is null || data.Results is null)
                    return false;

                if (data.Results is null || data.Results.Count == 0)
                    return false;

                foreach (Result result in data.Results)
                {

                    if (result.Id is null)
                        continue;

                    string? offerId = await GetOfferIdAsync(result.Id);
                    if (offerId is null)
                        continue;

                    //Console.WriteLine($"Try to add Id {result.Id} where Offer Id {offerId}");
                    await AddToLibraryAsync(result.Id, offerId);
                }

                url = !string.IsNullOrWhiteSpace(data.Next) ? data.Next : null;
            } while (!string.IsNullOrWhiteSpace(url));

            return true;
        }

        private static async Task<string?> GetOfferIdAsync(string uid)
        {
            string url = $"{BaseUrl}/i/listings/{uid}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse))
                return null;

            Item? item = JsonSerializer.Deserialize<Item>(jsonResponse);
            if (item is null || item.Licenses is null)
                return null;

            return item.Licenses[0].OfferId;
        }

        private static async Task AddToLibraryAsync(string uid, string offerId)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add("X-CsrfToken", _csrfToken);
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"130\", \"Google Chrome\";v=\"130\", \"Not?A_Brand\";v=\"99\"");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Origin", BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("Referer", $"{BaseUrl}/listings/{uid}");
            _httpClient.DefaultRequestHeaders.Add("Cookie", _cookie);

            string postUrl = $"{BaseUrl}/i/listings/{uid}/add-to-library";
            //StringContent content = new StringContent("{}", Encoding.UTF8, "application/json");

            var content = new MultipartFormDataContent("----WebKitFormBoundarykY9jx0OdKlXqI6Qb");
            content.Add(new StringContent(offerId), "offer_id");

            HttpResponseMessage response = await _httpClient.PostAsync(postUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Added item {uid} to library.");
            }
            else
            {
                Console.WriteLine($"ERROR: {response.StatusCode}");
            }
        }
    }
}
