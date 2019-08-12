using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestAutothonLib
{
    public static class SeleniumExtensions
    {
        //public static void WaitForPageLoad(this RemoteWebDriver driver, uint timeOutInSeconds = 30)
        //{
        //    IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds));
        //    wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        //}   

        public static void WaitForPageLoad(this RemoteWebDriver driver, uint timeOutInSeconds = 30)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(timeOutInSeconds);
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("driver", "Driver must support javascript execution");

            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript(
                    "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public static IEnumerable<IWebElement> WaitForElementsAndGet(this RemoteWebDriver driver, string xPath, int timeOutInSeconds = 5)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElements(By.XPath(xPath)));

        public static IEnumerable<IWebElement> WaitForElementsAndGet(this RemoteWebDriver driver, By by, int timeOutInSeconds = 5)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElements(by));

        public static IWebElement WaitForElementAndGet(this RemoteWebDriver driver, By by, int timeOutInSeconds = 5)
            => driver.Wait(timeOutInSeconds).Until(c => c.FindElement(by));

        public static WebDriverWait Wait(this RemoteWebDriver driver, int timeOutInSeconds = 5)
            => new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds));
    }
}
