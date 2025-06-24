using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TestConsoleApp.Interfaces;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public class Postcodes : IPostcodes
    {
        public List<string> Process(int numOf)
        {
            var result = new List<string>();
            var dictionary = new Dictionary<string, PostcodeTracker>();

            string resourceName = "TestConsoleApp.Data.postcodes.csv";
            // Get the assembly that contains the embedded resource
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Console.WriteLine("Resource not found.");
                    return null;
                }

                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split(',');
                        var prefix = fields[3].Trim('"').Split(new char[] { ' ' }, 2)[0];
                        if (int.TryParse(fields[1].Trim('"'), out var amount))
                        {
                            var tracker = new PostcodeTracker()
                            {
                                Prefix = prefix,
                                Count = 1,
                                Total = amount
                            };
                            if (!dictionary.TryGetValue(tracker.Prefix, out var existing))
                            {
                                dictionary[tracker.Prefix] = new PostcodeTracker()
                                {
                                    Prefix = tracker.Prefix,
                                    Count = tracker.Count,
                                    Total = tracker.Total
                                };
                            }
                            else
                            {
                                existing.Count++;
                                existing.Total += tracker.Total;
                            }
                        }
                    }
                }
            }

            if (dictionary.Any())
            {
                dictionary.Select(keyValue => new
                    {
                        Prefix = keyValue.Key,
                        Average = keyValue.Value.Total / keyValue.Value.Count
                    }).OrderByDescending(sorter => sorter.Average)
                    .Take(numOf)
                    .ToList()
                    .ForEach(item => { result.Add($"{item.Prefix} {item.Average.ToString("C0", new CultureInfo("en-gb"))}"); });
            }

            return result;
        }
    }
}
