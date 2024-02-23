using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using week02.Console;


var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
using var file = File.OpenRead("./Data/car-data.json");
var cars = JsonSerializer.Deserialize<List<Car>>(file, options);

var japan = cars
    .Where(car => car.Origin == "Japan" && 
                  DateTime.ParseExact(car.Year, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year >= 1970 &&
                  DateTime.ParseExact(car.Year, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year <= 1980)
    .Select(car => new 
    {
        car.Name,
        Year = DateTime.ParseExact(car.Year, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year
    })
    .Take(20)
    .ToList();

var cylinder = cars
    .Where(car => car.Cylinders == 4)
    .OrderBy(car => car.Acceleration)
    .ThenByDescending(car => car.MilesPerGallon)
    .Select(car => new
    {
        car.Name,
        Year = DateTime.ParseExact(car.Year, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year
    })
    .ToList();


var origin = cars
    .GroupBy(car => car.Origin)
    .Select(group => new
    {   
        Origin = group.Key,
        ModelCount = group.Count()

    })
    .OrderByDescending(group => group.ModelCount)
    .ToList();

Console.WriteLine("Japanese Cars from 1970-1980:");
foreach (var car in japan)
{
    Console.WriteLine($"Name: {car.Name}, Year: {car.Year}" );
}

Console.WriteLine("4-cylinder cars:");
foreach (var car in cylinder)
{
    Console.WriteLine($"Name: {car.Name}, Year: {car.Year}" );
}

Console.WriteLine("Cars group by origin");
foreach (var car in origin)
{
    Console.WriteLine($"Name: {car.Origin}" );
}




