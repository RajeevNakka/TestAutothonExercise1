using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAutothonExercise1.Parsers;

namespace TestAutothonExercise1
{
    public static class Problem1
    {
        public static List<string> MovieNames = new List<string>()
        {
            "NonExisting",
            "The Shawshank Redemption",
            "The Godfather",
            "The Dark Knight",
            "Pulp Fiction",
            "Schindler's List",
            "The Lord of the Rings:The Return of the King",//* multiple imdb links in wiki
            "The Good,The Bad,The Ugly", //* Invalid wiki link in bing
            "12 Angry Men",
            "Inception",
            "Forrest Gump",
            "Fight Club",
            "Star Wars:Episode V-The Empire Strikesn Back", //Works, but confused with video game
            "Goodfellas",
            "The Matrix", //The Wachowskis(Combined brother names in wiki)
            "One Flew Over The Cuckoo's Nest", //Novel vs Movie wiki page and special characters in director name
            "Seven Samurai",
            "Avengers:Infinity War", // "Anthony Russo\r\nJoe Russo" as single name in wiki
            "Interstellar",
            "Se7en"
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
            string outputDirectory = Path.Combine(Environment.CurrentDirectory, "Output");
            List<MovieInfo> movies = new List<MovieInfo>();
            Parallel.ForEach(MovieNames, (name) =>
            {
                movies.Add(GetMovieInfo(name, outputDirectory));
            });
            //Extract Info
            //Assert


            //Report
            foreach (var movie in movies)
            {
                Console.WriteLine(string.Join('|', movie.ToArray()));
            }

        }

        public static MovieInfo GetMovieInfo(string movieName, string outputDirectory)
        {
            MovieInfo info = new MovieInfo() { Name = movieName };
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("headless");
            using (ChromeDriver chrome = new ChromeDriver(Environment.CurrentDirectory,options))
            {
                //Search Movie
                Searcher searcher = new Searcher(driver: chrome,
                                                 urlBuilder: SearchUrlBuilders.Bing,
                                                 parsers: (driver) => RankWikiLinks(driver, movieName));

                var result = searcher.Search($"{movieName} film").Select(link=>link.GetAttribute("href")).ToList();

                foreach (var link in result)
                {
                    try
                    {
                        info.WikiLink = link;
                        chrome.Navigate().GoToUrl(info.WikiLink);
                        chrome.WaitForPageLoad();

                        chrome.GetScreenshot().SaveAsFile(Path.Combine(outputDirectory, $"{movieName}_wiki.png"));
                        WikiParser wp = new WikiParser(chrome);
                        info.Directors_Wiki = wp.GetDirectors();

                        var imdbLink = LinkFinders.FindImdb(chrome)?.Reverse()?.First();
                        info.ImdbLink = imdbLink.GetAttribute("href");
                        chrome.Navigate().GoToUrl($"{info.ImdbLink}fullcredits");
                        chrome.GetScreenshot().SaveAsFile(Path.Combine(outputDirectory, $"{movieName}_imdb.png"));
                        ImdbParser ip = new ImdbParser(chrome);
                        info.Directors_Imdb = chrome.WaitForElementsAndGet("//div[contains(@id,'fullcredits_content')]/h4[contains(text(),'Directed by')]/following-sibling::table[1]/tbody/tr/td/a")
                                            .Select(e => e.Text).ToList();

                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{movieName}" + e.Message);
                    }
                }

            }
            return info;
        }
        public class MovieInfo
        {
            public string Name { get; set; }
            public string WikiLink { get; set; }
            public List<string> Directors_Wiki { get; set; } = new List<string>();
            public string ImdbLink { get; set; }
            public List<string> Directors_Imdb { get; set; } = new List<string>();

            public string[] ToArray()
            {
                return new string[]
                {
                    Name,
                    String.Join(',',Directors_Wiki),
                    String.Join(',',Directors_Imdb)
                };
            }
        }
    }
}
