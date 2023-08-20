using System.ComponentModel.DataAnnotations;

namespace Levali.Api;

public sealed class Token
{
    [Required(AllowEmptyStrings = false)]
    public string Value { get; set; } = string.Empty;
    
    [Required()]
    public DateTime ExpiresIn { get; set; }
}
