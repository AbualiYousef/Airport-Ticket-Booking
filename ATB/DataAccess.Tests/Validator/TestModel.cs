using System.ComponentModel.DataAnnotations;

namespace ATB.Tests;

public class TestModel
{
    [Required]
    public string RequiredProperty { get; set; }

    [StringLength(10)]
    public string StringProperty { get; set; }

    [Range(1, 100)]
    public int RangeProperty { get; set; }

    [EmailAddress]
    public string EmailProperty { get; set; }
}