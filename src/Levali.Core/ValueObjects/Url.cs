using System.Text.RegularExpressions;

namespace Levali.Core;

public struct Url
{
    private const string FORMAT_REGEX = @"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$";
    const string ERROR_MESSAGE = "No valid URL format";
    private string _value { get; set; }
    public string Value { get => _value; set => _value = Parse(value); }

    private Url(string value)
    {
        _value = value;
    }

    public static Url Parse(string value)
    {
        if (TryParse(value, out var result))
        {
            return result;
        }

        throw new ArgumentException(ERROR_MESSAGE);
    }

    public static bool TryParse(string value, out Url url)
    {
        url = new Url(value);

        if (!Regex.IsMatch(url._value, FORMAT_REGEX))
        {
            url._value = Uri.UriSchemeHttps + "://" + url;
            return Regex.IsMatch(url._value, FORMAT_REGEX);
        }

        return true;
    }

    public override string ToString()
        => _value;

    public static implicit operator Url(string input)
        => Parse(input);

    public static implicit operator string(Url input)
        => input.ToString();
}
