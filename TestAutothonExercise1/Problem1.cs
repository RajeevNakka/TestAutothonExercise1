using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAutothonExercise1
{
    public static class Problem1
    {
        public static List<string> Movies = new List<string>()
        {
            //"NonExisting",
            //"The Shawshank Redemption",
            //"The Godfather",
            //"The Dark Knight",
            //"Pulp Fiction",
            //"Schindler's List",
            //"The Lord of the Rings:The Return of the King",//* multiple imdb links in wiki
            //"The Good,The Bad,The Ugly", //* Invalid wiki link in bing
            //"12 Angry Men",
            ////"Inception",
            ////"Forrest Gump",
            ////"Fight Club",
            //"Star Wars:Episode V-The Empire Strikesn Back", //Works, but confused with video game
            ////"Goodfellas",
            //"The Matrix", //The Wachowskis(Combined brother names in wiki)
            //"One Flew Over The Cuckoo's Nest", //Novel vs Movie wiki page and special characters in director name
            ////"Seven Samurai",
            //"Avengers:Infinity War", // "Anthony Russo\r\nJoe Russo" as single name in wiki
            ////"Interstellar",
            ////"Se7en"
        };


        public static IEnumerable<IWebElement> RankWikiLinks(this RemoteWebDriver driver, string key)
        {
            var wikiLinks = LinkFinders.FindWiki(driver);

            return (from link in wikiLinks
                    orderby link.Text.FindSimilarity(key) descending
                    select link);
        }

        public static void Solve()
        {
            List<MovieInfo> movies = new List<MovieInfo>();
            foreach (var movie in Movies.Take(1))
            {
                try
                {
                    MovieInfo info = new MovieInfo() { Name = movie };
                    movies.Add(info);
                    using (ChromeDriver chrome = new ChromeDriver(Environment.CurrentDirectory))
                    {
                        //Search Movie
                        Searcher searcher = new Searcher(driver: chrome,
                                                         urlBuilder: SearchUrlBuilders.Google,
                                                         parsers: (driver) => RankWikiLinks(driver, movie));

                        var result = searcher.Search($"{movie} film");

                        //Open Wiki in seperate browser 
                        using (ChromeDriver wikiChrome = new ChromeDriver(Environment.CurrentDirectory))
                        {
                            foreach (var link in result)
                            {
                                try
                                {
                                    wikiChrome.Navigate().GoToUrl(link.GetAttribute("href"));
                                    wikiChrome.WaitForPageLoad();

                                    info.Directors_Wiki = wikiChrome.WaitForElementsAndGet("//table[contains(@class,'infobox vevent')]/tbody/tr/th[contains(text(),'Directed by')]/following-sibling::td/a")
                                                                  .Select(e => e.Text).ToList();

                                    var imdbLink = LinkFinders.FindImdb(wikiChrome)?.Reverse()?.First();
                                    wikiChrome.Navigate().GoToUrl($"{imdbLink.GetAttribute("href")}fullcredits");
                                    info.Directors_Imdb = wikiChrome.WaitForElementsAndGet("//div[contains(@id,'fullcredits_content')]/h4[contains(text(),'Directed by')]/following-sibling::table[1]/tbody/tr/td/a")
                                                        .Select(e => e.Text).ToList();

                                    break;
                                }
                                catch(Exception e) {
                                    Console.WriteLine($"{movie}" + e.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine($"{movie}" + e.Message);
                }
            }
            //Extract Info
            //Assert
        }
        public class MovieInfo
        {
            public string Name { get; set; }
            public List<string> Directors_Wiki { get; set; }
            public List<string> Directors_Imdb { get; set; }
        }
    }
}
