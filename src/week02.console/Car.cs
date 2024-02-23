using System.Reflection.Metadata.Ecma335;

namespace week02.Console;

/// <summary>
/// The class represents a Car with basic information.
/// </summary>
public record Car
{
    /// <summary>
    /// Car name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Distance (in miles) that the car can go per gallon.
    /// </summary>
    public decimal? MilesPerGallon { get; init; }

    /// <summary>
    /// Number of an engine cylinders.
    /// </summary>
    public required int Cylinders { get; init; }

    /// <summary>
    /// Engine cylinder volume in cubic inches.
    /// </summary>
    public required decimal Displacement { get; init; }

    /// <summary>
    /// Power of an engine expressed in horsepower units.
    /// </summary>
    public int? Horsepower { get; init; }

    /// <summary>
    /// Weight of a car in pounds.
    /// </summary>
    public required int WeightInLbs { get; init; }

    /// <summary>
    /// Amount of time (in seconds) the car needs to accelerate from 0 to 60 miles per hour.
    /// </summary>
    public required decimal Acceleration { get; init; }

    /// <summary>
    /// Year when the car was manufactured.
    /// </summary>
    /// <remarks>Expressed in format YYYY-MM-DD, for example: 1979-01-01</remarks>
    public required string Year { get; init; }

    /// <summary>
    /// The country where the car originates.
    /// </summary>
    public required string Origin { get; init; }

    public override string ToString() 
    {
        return $@"Information about vehicle:
Name: {Name}
Miles per gallon: {MilesPerGallon}
Cylinders: {Cylinders}
Displacement: {Displacement}
Horsepower: {Horsepower}
Weight in pounds: {WeightInLbs}
Acceleration: {Acceleration}
Year: {Year}
Origin: {Origin}
";
    }
}

