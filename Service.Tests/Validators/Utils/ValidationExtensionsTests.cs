using FluentValidation;
using Valeting.Core.Validators.Utils;

namespace Service.Tests.Validators.Utils;

public class ValidationExtensionsTests
{
    private class TestRequest { public string Name { get; set; } }
    private class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }

    [Fact]
    public void ValidateRequest_ShouldNotThrow_WhenValid()
    {
        // Arrange
        var request = new TestRequest { Name = "Valid Name" };

        // Act & Assert
        request.ValidateRequest(new TestRequestValidator());
    }

    [Fact]
    public void ValidateRequest_ShouldThrowValidationException_WhenInvalid()
    {
        // Arrange
        var request = new TestRequest { Name = "" };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => request.ValidateRequest(new TestRequestValidator()));

        Assert.Contains("Name", exception.Message);
    }
}
