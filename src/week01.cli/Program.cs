namespace week01.cli;

internal class Week01
{
    public static void Main(string[] args)
    {
        // Saying Hello to names given as shell args
        foreach (var arg in args)
        {
            Console.WriteLine("Hello, " + arg);   
        }
    }
}
