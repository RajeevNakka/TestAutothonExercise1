using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAutothonExercise1.Parsers
{
    public class Parser
    {
        public RemoteWebDriver Driver { get; }

        public Parser(RemoteWebDriver driver)
        {
            this.Driver = driver;
        }
    }
    public class WikiParser : Parser
    {
        public WikiParser(RemoteWebDriver driver) : base(driver) { }

        public List<string> GetDirectors()
        {
            return Driver.WaitForElementsAndGet("//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a")
                                                          .SelectMany(e => e.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }

    public class ImdbParser : Parser
    {
        public ImdbParser(RemoteWebDriver driver) : base(driver) { }

        public List<string> GetDirectors()
        {
            return Driver.WaitForElementsAndGet("//div[contains(@id,'fullcredits_content')]/h4[contains(text(),'Directed by')]/following-sibling::table[1]/tbody/tr/td/a")
                                                .Select(e => e.Text).ToList();
        }
    }
}
