namespace Levali.Core;

public sealed class CodeGenerator
{
    private static readonly Random random = new();
    private const string Base36Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Generate()
    {
        var randomNumber = random.Next(46656, int.MaxValue);
        return EncodeBase36(randomNumber);
    }

    private static string EncodeBase36(long number)
    {
        string encodedString = string.Empty;

        if (number <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "The value cannot be negative or zero.");
        }

        do
        {
            encodedString = Base36Characters[(int)(number % 36)] + encodedString;
            number /= 36;
        } while (number > 0);

        return encodedString;
    }
}
