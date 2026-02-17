using AppCanzoniere.Backend.Controllers;
using AppCanzoniere.Backend.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AppCanzoniere.Backend.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
    private readonly IConfiguration _configuration;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Mock SignInManager
        var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
            _mockUserManager.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null, null, null, null);

        // Configuration for JWT
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:SecretKey", "test-secret-key-must-be-at-least-32-characters-long"},
            {"Jwt:Issuer", "https://test.com"},
            {"Jwt:Audience", "https://test.com"},
            {"Jwt:ExpirationMinutes", "60"}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new RegisterDto("test@test.com", "Test123!", "Test123!");
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<AuthResponseDto>();
        var response = okResult.Value as AuthResponseDto;
        response!.Token.Should().NotBeNullOrEmpty();
        response.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task Register_PasswordMismatch_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("test@test.com", "Test123!", "Different!");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_UserCreationFails_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("test@test.com", "Test123!", "Test123!");
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto("test@test.com", "Test123!");
        var user = new IdentityUser { Email = "test@test.com", UserName = "test@test.com" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<AuthResponseDto>();
        var response = okResult.Value as AuthResponseDto;
        response!.Token.Should().NotBeNullOrEmpty();
        response.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent@test.com", "Test123!");
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("test@test.com", "WrongPassword!");
        var user = new IdentityUser { Email = "test@test.com", UserName = "test@test.com" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
