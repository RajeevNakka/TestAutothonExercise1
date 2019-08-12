using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAutothon.ReportGenerator;
using TestAutothonLib.Parsers;

namespace TestAutothonLib
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
       
        static readonly object locker = new object();
        public static void Solve()
        {
            string outputDirectory = Path.Combine(Environment.CurrentDirectory, "Output");
            if (Directory.Exists(outputDirectory) == false)
                Directory.CreateDirectory(outputDirectory);
            List<MovieInfo> movies = new List<MovieInfo>();
            Parallel.ForEach(MovieNames, (name) =>
            {
                var info = GetMovieInfo(name, outputDirectory);
                lock (locker)
                {
                    movies.Add(info);
                }
            });
            
            //Assert
            //Report
            foreach (var movie in movies)
            {
                Console.WriteLine(string.Join("|", movie.ToArray()));
            }
            var templatePath = Path.Combine(Environment.CurrentDirectory, "TestAutothonReport.html");
            var html = ReportGenerator.Generate(GetTableData(movies), templatePath);
            File.WriteAllText(Path.Combine(outputDirectory, "report.html"), html);
        }

        public static IEnumerable<IEnumerable<string>> GetTableData(IEnumerable<MovieInfo> movies)
        {
            yield return new string[] { "Name", "WikiLink", "Wiki_Directors","Wiki_Screenshot", "ImdbLink", "Imdb_Directors", "Wiki_Screenshot", "Result" };
            foreach (var movie in movies)
            {
                yield return movie.ToArray();
            }
        }

        public static MovieInfo GetMovieInfo(string movieName, string outputDirectory)
        {
            MovieInfo info = new MovieInfo() { Name = movieName };
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("headless");
            using (ChromeDriver chrome = new ChromeDriver(Environment.CurrentDirectory, options))
            {
                //Search Movie
                Searcher searcher = new Searcher(driver: chrome,
                                                 urlBuilder: SearchUrlBuilders.Bing,
                                                 parsers: (driver) => LinkFinders.FindWiki(driver).RankLinks(movieName));

                var result = searcher.Search($"{movieName} film").Select(link => link.GetAttribute("href")).ToList();

                foreach (var link in result)
                {
                    try
                    {
                        info.WikiLink = link;
                        chrome.Navigate().GoToUrl(info.WikiLink);
                        chrome.WaitForPageLoad();

                        //Extract Info
                        info.WikiScreenShotPath = Path.Combine(outputDirectory, $"{movieName}_wiki.png");
                        chrome.GetScreenshot().SaveAsFile(info.WikiScreenShotPath);
                        WikiParser wp = new WikiParser(chrome);
                        info.Directors_Wiki = wp.GetDirectors();

                        //Extract Info
                        var imdbLink = LinkFinders.FindImdb(chrome)?.Reverse()?.First();
                        info.ImdbLink = imdbLink.GetAttribute("href");
                        chrome.Navigate().GoToUrl($"{info.ImdbLink}fullcredits");
                        info.ImdbScreenShotPath = Path.Combine(outputDirectory, $"{movieName}_imdb.png");
                        chrome.GetScreenshot().SaveAsFile(info.ImdbScreenShotPath);
                        ImdbParser ip = new ImdbParser(chrome);
                        info.Directors_Imdb = ip.GetDirectors();

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
            public string WikiScreenShotPath { get; set; }
            public string WikiLink { get; set; }
            public List<string> Directors_Wiki { get; set; } = new List<string>();
            public string ImdbScreenShotPath { get; set; }
            public string ImdbLink { get; set; }
            public List<string> Directors_Imdb { get; set; } = new List<string>();

            public bool Passed
            {
                get
                {
                    if (Directors_Wiki.Count != Directors_Imdb.Count)
                        return false;

                    for (int i = 0; i < Directors_Wiki.Count; i++)
                    {
                        if (Directors_Wiki[i] != Directors_Imdb[i])
                            return false;
                    }

                    return true;
                }
            }

            public IEnumerable<string> ToArray()
            {
                return new string[]
                {
                    ReportGenerator.GenerateElementHtml(Name,ReportElementType.Text),
                    ReportGenerator.GenerateElementHtml(WikiLink,ReportElementType.Link,WikiLink),
                    ReportGenerator.GenerateElementHtml(String.Join(",",Directors_Wiki),ReportElementType.Table),
                    ReportGenerator.GenerateElementHtml(WikiScreenShotPath,ReportElementType.Image,WikiScreenShotPath),
                    ReportGenerator.GenerateElementHtml(ImdbLink,ReportElementType.Link,ImdbLink),
                    ReportGenerator.GenerateElementHtml(String.Join(",",Directors_Imdb),ReportElementType.Table),
                    ReportGenerator.GenerateElementHtml(ImdbScreenShotPath,ReportElementType.Image,ImdbScreenShotPath),
                    ReportGenerator.GenerateElementHtml(Passed.ToString(),ReportElementType.Text),
                };
            }
        }
    }
}
