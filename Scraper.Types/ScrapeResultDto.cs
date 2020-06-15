using System;
using System.Collections.Generic;

namespace Scraper.Types
{
    public class ScrapeResultDto<T>
    {
        public List<T> Result { get; set; } = new List<T>();
        public bool IsSucess { get; set; }
        public int Count => Result.Count;
        public string RequestUrl { get; set; } = string.Empty;
        public string ContinueUrl { get; set; } = string.Empty;
        public Exception Exception { get; set; } = null;
    }
}
