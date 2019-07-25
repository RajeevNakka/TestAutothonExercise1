using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestAutothonExercise1
{
    public static class SeleniumExtensions
    {
        public static void WaitForPageLoad(this RemoteWebDriver driver, uint timeOutInSeconds = 30)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static IEnumerable<IWebElement> WaitForElements(this RemoteWebDriver driver, string xpath, uint timeOutInSeconds = 30)
        {
            WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            return waitForElement.Until(c => c.FindElements(By.XPath(xpath)));
        }
    }
}
