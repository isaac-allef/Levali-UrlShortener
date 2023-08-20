using System.Text.RegularExpressions;

namespace Levali.Core;

public struct Email
{
    private const string FORMAT_REGEX = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    const string ERROR_MESSAGE = "No valid E-mail format";
    private string _value { get; set; }
    public string Value { get => _value; set => _value = Parse(value); }

    private Email(string value)
    {
        _value = value;
    }

    public static Email Parse(string value)
    {
        if (TryParse(value, out var result))
        {
            return result;
        }

        throw new ArgumentException(ERROR_MESSAGE);
    }

    public static bool TryParse(string value, out Email email)
    {
        email = new Email(value);
        
        return Regex.IsMatch(email._value, FORMAT_REGEX);
    }

    public override string ToString()
        => _value;

    public static implicit operator Email(string input)
        => Parse(input);

    public static implicit operator string(Email input)
        => input.ToString();
}
