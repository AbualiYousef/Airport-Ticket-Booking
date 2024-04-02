using AutoFixture;
using DataAccess.Validation;

namespace ATB.Tests.Validator;

public class ModelValidatorTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly ModelValidator<TestModel> _validator = new();

    [Fact]
    public void Validate_NoErrors_ReturnsEmptyList()
    {
        var model = _fixture.Build<TestModel>()
            .With(x => x.RequiredProperty, "Test")
            .With(x => x.StringProperty, "Short")
            .With(x => x.RangeProperty, 50)
            .With(x => x.EmailProperty, "test@example.com")
            .Create();
        var result = _validator.Validate(model);
        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WithErrors_ReturnsErrorList()
    {
        var model = new TestModel();
        var result = _validator.Validate(model);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetValidationDetails_ReturnsCorrectDetails()
    {
        var details = _validator.GetValidationDetails();
        Assert.Contains("RequiredProperty", details.Keys);
        Assert.Contains("StringProperty", details.Keys);
        Assert.Contains("RangeProperty", details.Keys);
        Assert.Contains("EmailProperty", details.Keys);
    }
}