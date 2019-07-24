using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAutothonExercise1
{
    public static class LinkFinders
    {
        public static IEnumerable<IWebElement> FindWikiLinks(RemoteWebDriver driver)
        {
            return driver.FindElementsByPartialLinkText("Wikipedia").Take(1);
        }

        public static IEnumerable<IWebElement> FindImdbLinks(RemoteWebDriver driver)
        {
            return driver.FindElementsByPartialLinkText("IMDb").Take(1);
        }
    }
}
