using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Model
{
    public class AuthorEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string ContextUrl { get; set; }
    }
}
