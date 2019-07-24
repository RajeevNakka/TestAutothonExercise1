using System;
using System.Collections.Generic;
using System.Text;

namespace TestAutothonExercise1
{
    public class SearchUrlBuilders
    {
        public const string GoogleSearchUrlBase = "https://www.google.co.in";
        public const string BingSearchUrlBase = "https://www.bing.co.in";

        public static Uri Google(string KeyWord) => new Uri($"{GoogleSearchUrlBase}/search?q={KeyWord}");
        public static Uri Bing(string KeyWord) => new Uri($"{BingSearchUrlBase}/search?q={KeyWord}");
    }
}
