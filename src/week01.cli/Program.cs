namespace week01.cli;

internal class Program
{
    public static void Main(string[] args)
    {
        // Saying Hello to people with names given as shell args
        foreach (var arg in args)
        {
            Console.WriteLine("Hello, " + arg);   
        }
    }
}
