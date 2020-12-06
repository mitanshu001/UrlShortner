using Base62;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortner.Helper
{
    public class ShortUrlCreator
    {
        public static long UrlCount = 0;

        public static string CreateShortUrl(string longUrl, string clientId)
        {
            var base62Converter = new Base62Converter();

            var shortUrl = base62Converter.Encode(UrlCount.ToString());
            UrlCount = UrlCount + 1;
            return shortUrl;
        }
    }
}
