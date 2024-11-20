using GTA_Journal.Services;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace GTA_Journal.Repositories
{
    public static class JournalRepository
    {
        private const string _journalEndpoint = "https://journal.gtajournal.online";

        public class UserCredentials
        {
            public int UserId { get; set; }
            public string UsId { get; set; }
            public DateTime? Expires { get; set; }
        }

        public static async Task<UserCredentials> GetUserCredentials(string login, string password)
        {
            using (var client = GetHttpClient())
            {
                var bodyParams = new Dictionary<string, string>() {
                    { "login", login },
                    { "password", password }
                };

                var body = new FormUrlEncodedContent(bodyParams);
                HttpResponseMessage response = await client.PostAsync(_journalEndpoint + "/api.login", body);

                if (response.IsSuccessStatusCode)
                {
                    if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                    {
                        var cookieList = CookieService.ParseSetCookieHeader(cookies);

                        string rawUserId = "";
                        string rawUsId = "";
                        DateTime? expires = DateTime.Now;

                        foreach (var cookie in cookieList)
                        {
                            if (cookie.Name == "id")
                            {
                                rawUserId = cookie.Value;
                                expires = cookie.Expires;
                            }
                            else if (cookie.Name == "usid")
                                rawUsId = cookie.Value;
                        }

                        if (rawUserId.Length == 0 || rawUsId.Length == 0) {
                            Log.Error("GetUserCredentials: No required cookie values");
                        } else
                        {
                            GetMainPageInfo(int.Parse(rawUserId), rawUsId);

                            return new UserCredentials() 
                            {
                                UserId = int.Parse(rawUserId),
                                UsId = rawUsId,
                                Expires = expires
                            };
                        }
                    }
                    else
                    {
                        Log.Error("GetUserCredentials: No Set-Cookie header found");
                    }
                }
                else
                {
                    Log.Error($"GetUserCredentials: Failed to get response: {response.StatusCode}");
                }

                return null;
            }

        }

        public static async void GetMainPageInfo(int userId, string usId)
        {
            using (var client = GetHttpClient(userId, usId))
            {
                string response = await client.GetStringAsync(_journalEndpoint + "/dashboard");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                var isAdmin = htmlDocument.DocumentNode.SelectSingleNode("//*[@class='profile']").InnerHtml.Contains("/user/add");
                var username = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='username']").InnerText;
                var userStatus = htmlDocument.DocumentNode.SelectSingleNode("//*[@class='profile']//span[contains(@class, 'active')]").GetClasses().ToList()[1];

                Debug.WriteLine(isAdmin);
                Debug.WriteLine(userStatus);
                Debug.WriteLine(username);
            }

        }

        private static HttpClient GetHttpClient() {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:132.0) Gecko/20100101 Firefox/132.0");

            return client;
        }

        private static HttpClient GetHttpClient(int userId, string usId)
        {
            var client = GetHttpClient();

            client.DefaultRequestHeaders.Add("Cookie", $"id={userId}; usid={usId}");

            return client;
        }
    }
}
