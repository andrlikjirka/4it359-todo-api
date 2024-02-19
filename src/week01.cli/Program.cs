namespace week01.cli;

internal static class Program
{
    public static void Main(string[] args)
    {
        foreach (var arg in args)
        {
            FactorizeArgumentNumber(arg);
        }
    }

    /**
     * Method checks if the given argument is valid natural number, calls the factorization method and prints out the result  
     */
    private static void FactorizeArgumentNumber(string arg)
    {
        if (!int.TryParse(arg, out var number))
        {
            Console.WriteLine($"{arg} není platné číslo.");
            return;
        }

        if (number <= 0)
        {
            Console.WriteLine($"{arg} není přirození číslo.");
            return;
        }

        List<int> factors = WheelFactorization(number);

        Console.WriteLine(factors.Count > 1 ? $"{arg} = {string.Join(" * ", factors)}" : $"{arg} je prvočíslo.");
    }

    /**
     * Method for factorizing the number (method built on the trial factorization with improved efficiency, also known as wheel factorization) 
     */
    private static List<int> WheelFactorization(int number)
    {
        List<int> factors = [];
        while (number % 2 == 0)
        {
            factors.Add(2);
            number /= 2;
        }

        for (var i = 3; i * i < number; i += 2)
        {
            while (number % i == 0)
            {
                factors.Add(i);
                number /= i;
            }
        }

        if (number > 1)
        {
            factors.Add(number);
        }
        
        return factors;
    }
}