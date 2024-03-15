using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Http;

namespace Game.Functions
{
    internal class CookieManager
    {
        List<Cookie> _cookies { get; }

        internal CookieManager(HttpRequestData req, HttpResponseData res)
        {
            _cookies = req.Cookies.Select(cookie => new Cookie(cookie.Name, cookie.Value) { Domain = cookie.Domain, Path = cookie.Path, Secure = true, Expired = ((cookie.Expires != null) && (cookie.Expires < DateTimeOffset.Now)) }).ToList<Cookie>();
            string sessionId = Value("session-id");

            Cookie cookie = GetCookie("session-id");
            if (cookie == null)
            {
                cookie = new Cookie("session-id", Guid.NewGuid().ToString());
                cookie.Domain = req.Url.Host;
                cookie.Path = "/";
                cookie.Secure = true;
            }
            _cookies.Append(cookie);
            res.Cookies.Append(new HttpCookie("session-id", cookie.Value) { Domain = cookie.Domain, Path = cookie.Path, Secure = cookie.Secure });

        }

        string Value(string name)
        {
            Cookie cookie = _cookies.Find(cookie => cookie.Name == name && cookie.Expired == false);
            return cookie?.Value;
        }

        Cookie GetCookie(string name)
        {
            return _cookies.Find(cookie => cookie.Name == name && cookie.Expired == false);
        }
    }
}
