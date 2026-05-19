using Service.Models.User.Payload;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class UpdateAdminSettingsValidatorTests
{
    private readonly UpdateAdminSettingsValidator _validator;
    
    public UpdateAdminSettingsValidatorTests()
    {
        _validator = new UpdateAdminSettingsValidator();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000000")]
    [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001")]
    public void UserId_ShouldFail(Guid adminId, Guid userId)
    {        
        // Arrange
        var request = new UpdateAdminSettingsDtoRequest
        {
            AdminId = adminId,
            UserId = userId
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("User Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void RoleId_ShouldFail(Guid roleId)
    {
        // Arrange
        var request = new UpdateAdminSettingsDtoRequest
        {
            AdminId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RoleId = roleId
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Role Id", result.Errors.FirstOrDefault().ErrorMessage);
    }
}
