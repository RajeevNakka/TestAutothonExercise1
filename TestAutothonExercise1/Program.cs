using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
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
        public static List<string> Movies = new List<string>()
        {
            //"NonExisting",
            //"The Shawshank Redemption",
            //"The Godfather",
            //"The Dark Knight",
            //"Pulp Fiction",
            //"Schindler's List",
            "The Lord of the Rings:The Return of the King",
            "The Good,The Bad,The Ugly",
            "12 Angry Men",
            //"Inception",
            //"Forrest Gump",
            //"Fight Club",
            "Star Wars:Episode V-The Empire Strikesn Back",
            //"Goodfellas",
            "The Matrix",
            "One Flew Over The Cuckoo's Nest",
            //"Seven Samurai",
            "Avengers:Infinity War",
            //"Interstellar",
            //"Se7en"
        };
        static void Main(string[] args)
        {
            foreach (var movie in Movies.Take(1))
            {
                using (ChromeDriver chrome = new ChromeDriver(Environment.CurrentDirectory))
                {
                    Searcher searcher = new Searcher(driver: chrome,
                                                     urlBuilder: Bing,
                                                     FindWikiLinks);

                    //var result = searcher.Search("The Shawshank Redemption film").ToList();
                    var result = searcher.Search($"{movie} film").ToList();

                    //foreach (var element in result)
                    //{
                    //    Console.WriteLine(element.GetAttribute("href"));
                    //}

                    Parallel.ForEach(result, (element) =>
                    {
                        try
                        {
                            using (ChromeDriver chrome2 = new ChromeDriver(Environment.CurrentDirectory))
                            {
                                chrome2.Navigate().GoToUrl(element.GetAttribute("href"));
                                chrome2.WaitForPageLoad();
                                var wikiDir = chrome2.WaitForElements("//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a")
                                                      .Select(e => e.Text).ToList();
                                var imdbLink = FindImdbLinks(chrome2)?.First();
                                chrome2.Navigate().GoToUrl($"{imdbLink.GetAttribute("href")}fullcredits");
                                var imdbDir = chrome2.WaitForElements("//div[contains(@id,'fullcredits_content')]/h4[contains(text(),'Directed by')]/following-sibling::table[1]/tbody/tr/td/a")
                                                    .Select(e => e.Text).ToList(); ;

                                if (wikiDir.Count != imdbDir.Count)
                                    return;

                                for (int i = 0; i < wikiDir.Count; i++)
                                {
                                    if (wikiDir[i] != imdbDir[i])
                                        return;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + movie);
                        }
                    });
                }
            }

            Console.ReadKey();
        }

        static void Problem1()
        {
            //Search Movie
            //Open Wiki             
            //Extract Info
            //Assert
        }

    }
}
