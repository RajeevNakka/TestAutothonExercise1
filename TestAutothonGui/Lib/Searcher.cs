using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAutothonLib
{
    //public interface ISearchUriBuilder
    //{
    //    Uri GetSearchSite(string keyword);
    //}
    //public class GoogleSearchUriBuilder
    //{
    //    public String BaseUrl { get; } =  ""
    //    public Uri GetSearchUrl(string keyword)
    //    {
    //        return BaseUrl
    //    }
    //}

    //public class SearchSettings
    //{
    //    public Uri SearchSite { get; set; } = new Uri("");
    //    public int Depth { get; set; } = 1;
    //}
    class Searcher
    {
        public RemoteWebDriver Driver { get; private set; }
        public Func<string, Uri> UrlBuilder;
        public Func<RemoteWebDriver, IEnumerable<IWebElement>>[] Parsers { get; private set; } = new Func<RemoteWebDriver, IEnumerable<IWebElement>>[0];

        public Searcher(RemoteWebDriver driver, Func<string, Uri> urlBuilder, params Func<RemoteWebDriver, IEnumerable<IWebElement>>[] parsers)
        {
            this.Driver = driver;
            this.Parsers = parsers;
            this.UrlBuilder = urlBuilder;
        }

        public IEnumerable<IWebElement> Search(string keyword)
        {            
            Driver.Navigate().GoToUrl(UrlBuilder(keyword));

            return from parser in Parsers
                   from element in parser(Driver)
                   select element;
        }
    }

}
