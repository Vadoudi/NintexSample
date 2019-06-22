using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nintex.WebApi.Models
{
    public class UrlShorteningAddModel
    {
        public UrlShorteningAddModel()
        {
            UserAlias = string.Empty;
        }

        public string UserUrl { get; set; }

        public string UserAlias { get; set; }

    }
}
