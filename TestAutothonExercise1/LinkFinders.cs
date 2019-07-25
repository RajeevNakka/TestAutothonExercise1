using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
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
            WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            return waitForElement.Until(c => c.FindElements(By.XPath("//a[contains(@href,'wikipedia')]")))
                                 .Take(1); ;

            //return driver.FindElementsByXPath("//a[contains(@href,'wikipedia')]")
            //                //.Where(e => e.Text.ToLower().Contains("did you mean") == false)
            //                .Take(1);
        }

        public static IEnumerable<IWebElement> FindImdbLinks(RemoteWebDriver driver)
        {
            WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            return waitForElement.Until(c => c.FindElements(By.XPath("//a[contains(@href,'imdb')]")))
                                 .Take(1);

            //return driver.FindElementsByXPath("//a[contains(@href,'imdb')]")
            //                //.Where(e => e.Text.ToLower().Contains("did you mean") == false)
            //                .Take(1);
        }
    }
}
