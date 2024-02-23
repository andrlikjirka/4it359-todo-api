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

    // Prvn�ch 20 aut vyroben�ch mezi lety 1970 a 1985, vypisuji jm�no a datum v�roby
    cars.Where(c =>
        DateTime.ParseExact(c.Year, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year > 1970 &&
        DateTime.ParseExact(c.Year, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year < 1985
    )
    .Take(20)
    .ToList()
    .ForEach(c =>
        Write(c)
    );

    // V�echna �ty�v�lcov� auta se�azena sestupn� dle akcelerace a n�sledn� dle m�l� na galon sestupn�
    cars.Where(c =>
        c.Cylinders == 4
    )
    .OrderBy(c => c.Acceleration)
    .ThenBy(c => c.MilesPerGallon)
    .ToList()
    .ForEach(c =>
        Write(c)
    );

    // Po�et vyroben�ch aut podle m�sta, kde byly vyrobeny
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