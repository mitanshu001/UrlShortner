using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortner.Models
{
    public class Url
    {
        public string LongUrl { get; set; }

        [Key]
        public string ShortUrl { get; set; }

        public string ClientId { get; set; }

        public long HitCount { get; set; }

        public Url()
        {
            HitCount = 0;
        }
    }
}
