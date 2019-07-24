using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TestAutothonExercise1.LinkFinders;
using static TestAutothonExercise1.SearchUrlBuilders;

namespace TestAutothonExercise1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChromeDriver chrome = new ChromeDriver(Environment.CurrentDirectory))
            {
                Searcher searcher = new Searcher(driver: chrome,
                                                 urlBuilder: Google,
                                                 FindImdbLinks,FindWikiLinks);

                var result = searcher.Search("iron man").ToList();

                Parallel.ForEach(result, (element) =>
                {
                    using (ChromeDriver chrome2 = new ChromeDriver(Environment.CurrentDirectory))
                    {
                        chrome2.Navigate().GoToUrl(element.GetAttribute("href"));
                    }
                });
            }
        }
    }
}
