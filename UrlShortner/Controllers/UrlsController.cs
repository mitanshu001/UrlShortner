using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Helper;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    [Route("api")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly UrlContext _context;

        public UrlsController(UrlContext context)
        {
            _context = context;
        }


        [HttpGet("originalurl/{short_url}")]
        
        public async Task<ActionResult<string>> GetOriginalUrl(string short_url)
        {
            if (string.IsNullOrWhiteSpace(short_url))
            {
                return BadRequest();
            }
            var url = await _context.Urls.FindAsync(short_url);

            if (url == null)
            {
                return NotFound();
            }
            url.HitCount = url.HitCount + 1;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UrlExists(url.ShortUrl))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return url.LongUrl;
        }

        [HttpGet("usecount/{short_url}")]
        public async Task<ActionResult<long>> UrlUsageCount(string short_url)
        {
            if (string.IsNullOrWhiteSpace(short_url))
            {
                return BadRequest();
            }
            var url = await _context.Urls.FindAsync(short_url);

            if (url == null)
            {
                return NotFound();
            }

            return url.HitCount;
        }

        [HttpPost()]
        [Route("shortenedurl")]
        public async Task<ActionResult<string>> GetShortenedUrl(UrlDTO urldto)
       {
            var long_url = urldto.long_url;
            var client_id = urldto.client_id;
            if(string.IsNullOrWhiteSpace(long_url) || string.IsNullOrWhiteSpace(client_id))
            {
                return BadRequest();
            }

            var url = GetUrl(long_url, client_id);
            
            if(url != null && url.Any())
            {
                return url.First().ShortUrl; 
            }
          
            string shortUrl = ShortUrlCreator.CreateShortUrl(long_url, client_id);

            Url newUrl = new Url
            {
                LongUrl = long_url,
                ClientId = client_id,
                ShortUrl = shortUrl,
                HitCount = 0
            };

            _context.Urls.Add(newUrl);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UrlExists(newUrl.ShortUrl))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetShortenedUrl), new { shortUrl = newUrl.ShortUrl }, shortUrl);
        }



        private bool UrlExists(string shortUrl)
        {
            return _context.Urls.Any(e => e.ShortUrl == shortUrl);
        }

        private IEnumerable<Url> GetUrl(string longUrl, string clientId)
        {
            var collection = _context.Urls as IQueryable<Url>;

            var LongUrl = longUrl.Trim();

            collection = collection.Where(a => a.LongUrl == LongUrl);

            var ClientId = clientId.Trim();

            collection = collection.Where(a => a.ClientId == ClientId);

            return collection.ToList();
        }
    }
}
