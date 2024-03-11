public static class InputParser
{
    public delegate bool ParseFunc<TValue>(string input, out TValue value);

    public static TValue GetInput<TValue>(string prompt, ParseFunc<TValue> parseFunc)
    {
        string input;
        TValue parsedInput;

        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (!parseFunc(input, out parsedInput));

        return parsedInput;
    }

    public static TValue? GetOptionalInput<TValue>(string prompt, ParseFunc<TValue> parseFunc) where TValue : struct
    {
        Console.Write(prompt + "(Press Enter to skip): ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (parseFunc(input, out TValue parsedInput))
        {
            return parsedInput;
        }

        Console.WriteLine("Invalid input, please try again.");
        return GetOptionalInput(prompt, parseFunc);
    }

    public static string? GetOptionalStringInput(string prompt)
    {
        Console.Write(prompt + "(Press Enter to skip): ");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        return input;
    }
    
    
    
}