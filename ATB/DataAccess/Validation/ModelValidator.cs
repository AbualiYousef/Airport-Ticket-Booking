using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DataAccess.Validation;

public class ModelValidator<T>
{
    public List<string> Validate(T model)
    {
        var errors = new List<string>();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();

            foreach (var attribute in validationAttributes)
            {
                if (!attribute.IsValid(property.GetValue(model)))
                {
                    errors.Add($"Error in {property.Name}: {attribute.FormatErrorMessage(property.Name)}");
                }
            }
        }

        return errors;
    }

    public Dictionary<string, string> GetValidationDetails()
    {
        var details = new Dictionary<string, string>();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();
            var constraints = new List<string>();

            foreach (var attribute in validationAttributes)
            {
                string constraintDetail = attribute switch
                {
                    RequiredAttribute => "Required",
                    StringLengthAttribute strLength => $"Max length: {strLength.MaximumLength}",
                    RangeAttribute range => $"Range: {range.Minimum} to {range.Maximum}",
                    EmailAddressAttribute => "Email format",
                    _ => attribute.GetType().Name.Replace("Attribute", "")
                };

                constraints.Add(constraintDetail);
            }

            if (constraints.Any())
            {
                details.Add(property.Name, string.Join(", ", constraints));
            }
        }

        return details;
    }
}