using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using week02.Console;
{
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    using var file = File.OpenRead("./Data/car-data.json");
    var cars = JsonSerializer.Deserialize<List<Car>>(file, options);
    void Write(Car c) 
    {
        Console.WriteLine($"Name of car: {c.Name}\nYear of manufacture: {c.Year}\n");
    }

    // Prvních 20 aut vyrobených mezi lety 1970 a 1985, vypisuji jméno a datum výroby
    cars.Where(c =>
        DateTime.ParseExact(c.Year, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year > 1970 &&
        DateTime.ParseExact(c.Year, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year < 1985
    )
    .Take(20)
    .ToList()
    .ForEach(c =>
        Write(c)
    );

    // Všechna ètyøválcová auta seøazena sestupnì dle akcelerace a následnì dle mílí na galon sestupnì
    cars.Where(c =>
        c.Cylinders == 4
    )
    .OrderBy(c => c.Acceleration)
    .ThenBy(c => c.MilesPerGallon)
    .ToList()
    .ForEach(c =>
        Write(c)
    );

    // Poèet vyrobených aut podle místa, kde byly vyrobeny
    var carCountByOrigin = cars
        .GroupBy(c => c.Origin)
        .Select(group =>
            new { Origin = group.Key, CarCount = group.Count() }
        );

    foreach (var item in carCountByOrigin)
    {
        Console.WriteLine($"{item.Origin} - {item.CarCount}");
    }
}