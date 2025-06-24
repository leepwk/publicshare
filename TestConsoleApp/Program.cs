using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public class ListComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            int hashcode = 0;
            foreach (T t in obj)
            {
                hashcode ^= t.GetHashCode();
            }
            return hashcode;
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            //PostcodeTest();
            //FruitAggregateTest();
            ScrabbleAggregateTest();
            //SpaceNewsTest();
            //StarwarsBuddiesTest();

            Console.ReadLine();
            return 1;
        }

        public static void SpaceNewsTest()
        {
            var client = new HttpClient();
            var jsonResult = client.GetStringAsync("https://api.spaceflightnewsapi.net/v4/articles/?limit=500").Result;
            var spaceNews = new SpaceNews{ results = new List<Article>() };
            var spaceNewsPage = JsonSerializer.Deserialize<SpaceNews>(jsonResult);
            do
            {
                spaceNews.results.AddRange(spaceNewsPage.results);
                jsonResult = client.GetStringAsync(spaceNewsPage?.next).Result;
                spaceNewsPage = JsonSerializer.Deserialize<SpaceNews>(jsonResult);
            } while (!string.IsNullOrEmpty(spaceNewsPage?.next));

            var result = spaceNews.results.Aggregate(new List<NewsItem>(), (news, article) =>
            {
                if (string.IsNullOrEmpty(article.news_site)) return news;

                var toAddNews = new NewsItem
                {
                    Published = new DateTime(article.published_at.Year, article.published_at.Month, 1),
                    NewsSite = article.news_site,
                    Count = 1
                };

                var existing = news.FirstOrDefault(i => i.NewsSite == toAddNews.NewsSite && i.Published == toAddNews.Published);
                if (existing != null)
                {
                    existing.Count++;
                }
                else
                {
                    news.Add(toAddNews);
                }

                return news;
            });

            result.OrderBy(item => item.Published).Take(20).ToList().ForEach(item => { Console.WriteLine($"{item.Published.ToString("MMMM yyyy")} {item.NewsSite} {item.Count}"); });
        }

        public static void StarwarsBuddiesTest()
        {
            var client = new HttpClient();
            var jsonResult = client.GetStringAsync("https://swapi.dev/api/people").Result;
            var people = new People { results = new List<Person>() };
            var peoplePage = JsonSerializer.Deserialize<People>(jsonResult);
            do
            {
                people.results.AddRange(peoplePage.results);
                jsonResult = client.GetStringAsync(peoplePage?.next).Result;
                peoplePage = JsonSerializer.Deserialize<People>(jsonResult);
            } while (!string.IsNullOrEmpty(peoplePage?.next));

            //var peopleFilms = new Dictionary<List<string>, List<string>>(new ListComparer<string>());

            var result = people.results.Aggregate(
                new Dictionary<List<string>, List<string>>(new ListComparer<string>()), (peopleFilms, person) =>
                {
                    var sortedFilms = person.films.OrderBy(item => item).ToList();
                    if (peopleFilms.TryGetValue(sortedFilms, out var names))
                    {
                        names.Add(person.name);
                    }
                    else
                    {
                        peopleFilms[sortedFilms] = new List<string> { person.name };
                    }

                    return peopleFilms;
                });

            result.Where(i => i.Value.Count > 1).ToList().ForEach(item => { Console.WriteLine($"{string.Join(", ", item.Value)}"); });
        }

        public static void ScrabbleAggregateTest()
        {
            var client = new HttpClient();
            var jsonResult = client.GetStringAsync("https://raw.githubusercontent.com/benjamincrom/scrabble/master/scrabble/dictionary.json").Result;
            var words = JsonSerializer.Deserialize<string[]>(jsonResult);
            
            //var words = new string[]
            //{
            //    "aardvark",
            //    "banana",
            //    "cherry",
            //    "xylophone",
            //    "query",
            //    "eczema",
            //    "tortoise",
            //    "whistle",
            //    "juniper",
            //    "rabbits",
            //    "hello"
            //};

            var isValid = false;

            do
            {
                Console.Write("Enter a number: ");
                string input = Console.ReadLine(); // Reads the user input as a string

                if (int.TryParse(input, out int number)) // Safely tries to parse the string as an integer
                {
                    Console.WriteLine($"Top {number} words in scrabble");
                    isValid = true;
                    var result = new Scrabble().Process(words.ToList(), number);
                    result.ForEach(item => { Console.WriteLine($"{item.Word} {item.Points}"); });
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            } while (!isValid);
        }

        public static void FruitAggregateTest()
        {
            var dictionary = new Dictionary<int, string>
            {
                { 1, "apple" },
                { 2, "banana" },
                { 3, "cherry" },
                { 4, "" },
                { 5, "strawberry" },
                { 6, "grape" },
                { 7, "" },
                { 8, "cherry" },
                { 9, "strawberry" },
                { 10, "cherry" },
                { 11, "cherry" },
                { 12, "banana" },
            };


            //var result = dictionary.Aggregate(new Dictionary<string, Fruit>(), (accumulator, kvp) =>
            //    {
            //        if (string.IsNullOrEmpty(kvp.Value)) return accumulator;
            //        if (!accumulator.TryGetValue(kvp.Value, out var existing))
            //        {
            //            accumulator[kvp.Value] = new Fruit
            //            {
            //                Name = kvp.Value,
            //                Count = 1
            //            };
            //        }
            //        else
            //        {
            //            existing.Count++;
            //        }

            //        return accumulator;
            //    }).Select(fruit => new
            //    {
            //        Name = fruit.Key, fruit.Value.Count
            //    }).OrderByDescending(sorter => sorter.Count)
            //    .ThenBy(sorter => sorter.Name)
            //    .Take(5)
            //    .ToList();

            //result.ForEach(item => { Console.WriteLine($"{item.Name} {item.Count}");});

            var result = new Dictionary<string, Fruit>();

            foreach (var word in dictionary)
            {
                if (!string.IsNullOrEmpty(word.Value))
                {
                    if (!result.TryGetValue(word.Value, out var existing))
                    {
                        result.Add(word.Value, new Fruit
                        {
                            Name = word.Value,
                            Count = 1
                        });
                    }
                    else
                    {
                        existing.Count++;
                    }
                }
            }

            result.Select(fruit => new Fruit
            {
                Name = fruit.Key, Count = fruit.Value.Count,
            }).OrderByDescending(sorter => sorter.Count)
                .ThenBy(sorter => sorter.Name)
                .Take(5)
                .ToList()
                .ForEach(item => { Console.WriteLine($"{item.Name} {item.Count}"); });
        }

        public static void PostcodeTest()
        {
            try
            {
                //var filePath = @"C:\Temp\postcodes.csv";
                //var url = "http://prod.publicdata.landregistry.gov.uk.s3-website-eu-west-1.amazonaws.com/pp-monthly-update-new-version.csv";
                //using (HttpClient client = new HttpClient())
                //using (var response = await client.GetStreamAsync(url))
                //using (var fs = new FileStream(filePath, FileMode.CreateNew))
                //{
                //    await response.CopyToAsync(fs);
                //}

                //using (var client = new HttpClient())
                //using (HttpResponseMessage response = await client.GetAsync(url))
                //using (HttpContent content = response.Content)
                //using (var stream = (MemoryStream)await content.ReadAsStreamAsync())
                //using (var fs = File.Create(filePath))
                //{
                //    await stream.CopyToAsync(fs);
                //}

                var results = new Postcodes().Process(10);
                foreach (var result in results)
                {
                    Console.WriteLine($"{result}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
