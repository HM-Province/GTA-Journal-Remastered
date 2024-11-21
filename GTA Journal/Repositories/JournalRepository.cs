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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Power;
using Windows.Foundation.Collections;

namespace GTA_Journal.Repositories
{
    public static class JournalRepository
    {
        private const string _journalEndpoint = "https://journal.gtajournal.online";

        public enum UserStatus
        {
            Online, AFK, Offline
        }

        public class UserCredentials
        {
            public int UserId { get; set; }
            public string UsId { get; set; }
            public DateTime? Expires { get; set; }
        }

        public class CurrentUserInfo
        {
            public string Username { get; set; }
            public string AvatarUrl { get; set; }
            public bool IsAdmin { get; set; }
            public UserStatus Status { get; set; }
        }

        public class MainPageUserInfo
        {
            public string Username { get; set; }
            public string AvatarUrl { get; set; }
            public string Link { get; set; }
            public bool IsAdmin { get; set; }
            public UserStatus Status { get; set; }
        }

        public class MainPageInfo
        {
            public int ServerId { get; set; }
            public CurrentUserInfo CurrentUser { get; set; }
            public List<MainPageUserInfo> Users { get; set; }
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

        public static async Task<MainPageInfo> GetMainPageInfo(int userId, string usId)
        {
            try
            {
                using var client = GetHttpClient(userId, usId);
                string response = await client.GetStringAsync(_journalEndpoint + "/dashboard");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                var currentUser = new CurrentUserInfo()
                {
                    Username = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='username']").InnerText,
                    AvatarUrl = _journalEndpoint + htmlDocument.DocumentNode.SelectSingleNode("//*[@class='profile']/div[@class='avatar']/img").GetAttributeValue("src", "/"),
                    IsAdmin = htmlDocument.DocumentNode.SelectSingleNode("//*[@class='profile']").InnerHtml.Contains("/user/add"),
                    Status = GetUserStatus(htmlDocument.DocumentNode.SelectSingleNode("//*[@class='profile']//span[contains(@class, 'active')]").GetClasses().ToList()[1])
                };

                int serverId = 0;
                string serverPattern = @"server(\d+)";
                Match match = Regex.Match(currentUser.AvatarUrl, serverPattern);

                if (match.Success && match.Groups.Count > 1)
                    serverId = int.Parse(match.Groups[1].Value);

                var users = new List<MainPageUserInfo>();
                var userCards = htmlDocument.DocumentNode.SelectNodes("//main//div[@class='col-12 col-lg-4']/div[@class='dash-scroll-block']/div[@class='item']");
                foreach (var userCard in userCards)
                {
                    users.Add(new MainPageUserInfo()
                    {
                        Username = userCard.SelectSingleNode(".//a[@class='username']").InnerText,
                        Status = GetUserStatus(userCard.SelectSingleNode(".//div[contains(@class, 'avatar')]").GetClasses().ToList()[1]),
                        Link = userCard.SelectSingleNode(".//a[@class='username']").GetAttributeValue("href", ""),
                        IsAdmin = userCard.SelectSingleNode(".//div[contains(@class, 'avatar')]/span[@class='admin']") != null,
                        AvatarUrl = _journalEndpoint + userCard.SelectSingleNode(".//div[contains(@class, 'avatar')]/img").GetAttributeValue("src", "/")
                    });
                }

                return new MainPageInfo()
                {
                    ServerId = serverId,
                    CurrentUser = currentUser,
                    Users = users
                };
            } 
            catch (Exception ex)
            {
                Log.Error(ex, "GetMainPageInfo: Failed to get page info");
                return null;
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

        private static UserStatus GetUserStatus(string status)
        {
            switch(status)
            {
                case "online":
                    return UserStatus.Online;
                case "afk":
                    return UserStatus.AFK;
                default:
                    return UserStatus.Offline;
            }
        }
    }
}
