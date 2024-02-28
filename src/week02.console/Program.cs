using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace week02.console;

public static class Program
{
    private const string PathToFile = "./Data/car-data.json";
    
    public static void Main(string[] args)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var cars = GetCarsData(PathToFile, options);
        if (cars is null) return;
        
        Console.WriteLine("First Query: ");
        FirstQueryResult(cars);
        
        Console.WriteLine("\nSecond Query:");
        SecondQueryResult(cars);

        Console.WriteLine("\nThird Query:");
        ThirdQueryResult(cars);
    }

    private static List<Car>? GetCarsData(string path, JsonSerializerOptions options)
    {
        try
        {
            using var file = File.OpenRead(path);
            return JsonSerializer.Deserialize<List<Car>>(file, options);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: car-data.json file not found.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Error: Unable to parse car-data.json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        return null;
    }

    /*
     List first 20 cars which were manufactured in Japan between years 1970-1980 and pick only Name and Year properties.
     Year property should be parsed in order to show only a year part of the date.
     */
    private static void FirstQueryResult(IEnumerable<Car> cars)
    {
        var carsResult = cars
            .Where(car => car.Origin == "Japan" && ParseYear(car.Year) >= 1970 && ParseYear(car.Year) <= 1980)
            .Take(20)
            .Select(car => new {car.Name, Year = ParseYear(car.Year)});

        foreach (var car in carsResult)
        {
            Console.WriteLine($"{car.Name} - {car.Year}");
        }
    }

    /*
     List all 4-cylinder cars ordered from the fastest to the slowest, and for cars that have identical acceleration,
     order them by their gasoline consumption from the greediest and pick only Name and Year properties.
     */
    private static void SecondQueryResult(IEnumerable<Car> cars)
    {
        var carsResult = cars
            .Where(car => car.Cylinders >= 4)
            .OrderBy(car => car.Acceleration)
            .ThenBy(car => car.MilesPerGallon)
            .Select(car => new {car.Name, Year = ParseYear(car.Year)});

        foreach (var car in carsResult)
        {
            Console.WriteLine($"{car.Name} - {car.Year}");
        }
    }

    /*
      List origins ordered by a sum of all car models manufactured in a given country or origin.
     */
    private static void ThirdQueryResult(IEnumerable<Car> cars)
    {
        var carsResult = cars
            .GroupBy(car => car.Origin)
            .OrderByDescending(group => group.Count());

        foreach (var group in carsResult)
        {
            Console.WriteLine($"{group.Key} ({group.Count()})");
            /*
            foreach (var car in group)
            {
                Console.WriteLine("    " + car.Name);
            }
            */
        }
    }

    private static int ParseYear(string date)
    {
        return DateTime.ParseExact(date, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year;
    }
}
