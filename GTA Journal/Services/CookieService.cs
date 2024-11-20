using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_Journal.Services
{
    public class Cookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime? Expires { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }

        public override string ToString()
        {
            return $"{Name}={Value}; Domain={Domain}; Path={Path}; Expires={Expires}; HttpOnly={HttpOnly}; Secure={Secure}";
        }
    }

    public static class CookieService
    {
        public static List<Cookie> ParseSetCookieHeader(IEnumerable<string> setCookieHeaders)
        {
            var cookies = new List<Cookie>();

            foreach (var header in setCookieHeaders)
            {
                var cookie = new Cookie();
                var parts = header.Split(';');

                var nameValue = parts[0].Split('=');
                if (nameValue.Length == 2)
                {
                    cookie.Name = nameValue[0].Trim();
                    cookie.Value = nameValue[1].Trim();
                }

                for (int i = 1; i < parts.Length; i++)
                {
                    var part = parts[i].Trim();
                    if (part.StartsWith("Domain=", StringComparison.OrdinalIgnoreCase))
                    {
                        cookie.Domain = part.Substring("Domain=".Length).Trim();
                    }
                    else if (part.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                    {
                        cookie.Path = part.Substring("Path=".Length).Trim();
                    }
                    else if (part.StartsWith("Expires=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.TryParse(part.Substring("Expires=".Length).Trim(), out var expires))
                        {
                            cookie.Expires = expires;
                        }
                    }
                    else if (part.Equals("HttpOnly", StringComparison.OrdinalIgnoreCase))
                    {
                        cookie.HttpOnly = true;
                    }
                    else if (part.Equals("Secure", StringComparison.OrdinalIgnoreCase))
                    {
                        cookie.Secure = true;
                    }
                }

                cookies.Add(cookie);
            }

            return cookies;
        }
    }
}
