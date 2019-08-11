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
        public static IEnumerable<IWebElement> FindWiki(this RemoteWebDriver driver) => Find(driver, "wikipedia");
        public static IEnumerable<IWebElement> FindImdb(this RemoteWebDriver driver) => Find(driver, "imdb");

        public static IEnumerable<IWebElement> Find(this RemoteWebDriver driver, string hrefSubString) 
            => driver.WaitForElementsAndGet(By.XPath($"//a[contains(@href,'{hrefSubString}')]"));    
    }
}
