using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TestConsoleApp
{
    public class CityTemps
    {
        public string Name { get; set; }
        public List<DateTime> TakenDates { get; set; } = new List<DateTime>();
        public List<double> Temperatures { get; set; } = new List<double>();
    }
    public class Temperature
    {
        int targetYear = 2025;

        public string Process(string[] input, int targetMonth)
        {
            var cityList = new List<CityTemps>();
            foreach (var cityLine in input)
            {
                var item = cityLine.Split(',');

                var existingCity = cityList.FirstOrDefault(s => s.Name == item[0].ToUpper());
                if (existingCity != null)
                {
                    existingCity.TakenDates.Add(DateTime.ParseExact(item[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    existingCity.Temperatures.Add(double.Parse(item[2]));
                }
                else
                {
                    var toAdd = new CityTemps { Name = item[0].ToUpper() };
                    toAdd.TakenDates.Add(DateTime.ParseExact(item[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    toAdd.Temperatures.Add(double.Parse(item[2]));
                    cityList.Add(toAdd);
                }
            }

            var monthlyAverages = new List<(string City, double AvgTemp)>();
            foreach (var city in cityList)
            {
                double tempSum = 0;
                int count = 0;

                for (int i = 0; i < city.TakenDates.Count; i++)
                {
                    var date = city.TakenDates[i];
                    if (date.Year == targetYear && date.Month == targetMonth)
                    {
                        tempSum += city.Temperatures[i];
                        count++;
                    }
                }

                double avgTemp = count > 0 ? tempSum / count : 0;
                monthlyAverages.Add((city.Name, avgTemp));
            }

            var result = monthlyAverages.OrderByDescending(s => s.AvgTemp).FirstOrDefault();
            return $"{result.City}, {result.AvgTemp}";

            //var monthlyAverages = cityList
            //    .Select(city => new
            //    {
            //        City = city.Name,
            //        AvgTemp = city.TakenDates
            //            .Select((date, index) => new { Date = date, Temp = city.Temperatures[index] })
            //            .Where(x => x.Date.Year == targetYear && x.Date.Month == targetMonth)
            //            .Select(x => x.Temp)
            //            .DefaultIfEmpty() // Prevent exception if no data in month
            //            .Average()
            //    }).OrderByDescending(s => s.AvgTemp).ToList();

            //return $"{monthlyAverages.First().City}, {monthlyAverages.First().AvgTemp}";
        }
    }
}
