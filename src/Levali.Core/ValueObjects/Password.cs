using System.Text.RegularExpressions;

namespace Levali.Core;

public struct Password
{
    private const string FORMAT_REGEX = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
    const string ERROR_MESSAGE = "No valid password format";
    private string _value { get; set; }
    public string Value { get => _value; set => _value = Parse(value); }

    private Password(string value)
    {
        _value = value;
    }

    public static Password Parse(string value)
    {
        if (TryParse(value, out var result))
        {
            return result;
        }

        throw new ArgumentException(ERROR_MESSAGE);
    }

    public static bool TryParse(string value, out Password password)
    {
        password = new Password(value);
        
        return Regex.IsMatch(password._value, FORMAT_REGEX);
    }

    public override string ToString()
        => _value;

    public static implicit operator Password(string input)
        => Parse(input);

    public static implicit operator string(Password input)
        => input.ToString();
}
