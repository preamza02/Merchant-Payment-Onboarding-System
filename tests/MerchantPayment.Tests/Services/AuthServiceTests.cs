using FluentAssertions;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Application.Services;
using MerchantPayment.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace MerchantPayment.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(x => x["Jwt:Secret"]).Returns("ThisIsASecretKeyForTestingPurposesOnly12345678901234567890");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
        _configurationMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");

        _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameNotExists_ShouldCreateUser()
    {
        var request = new RegisterUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(request.Username)).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _authService.RegisterAsync(request);

        result.Should().NotBeNull();
        result.Username.Should().Be(request.Username);
        result.Email.Should().Be(request.Email);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ShouldThrowInvalidOperationException()
    {
        var request = new RegisterUserRequest
        {
            Username = "existinguser",
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(request.Username)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ShouldReturnToken()
    {
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = "testuser@example.com",
            PasswordHash = hashedPassword,
            Role = "User"
        };
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be(request.Username);
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedAccessException()
    {
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ShouldThrowUnauthorizedAccessException()
    {
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "WrongPassword!"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!");
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = "testuser@example.com",
            PasswordHash = hashedPassword
        };
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(request));
    }
}
