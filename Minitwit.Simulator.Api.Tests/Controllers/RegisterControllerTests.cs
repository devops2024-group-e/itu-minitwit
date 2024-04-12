using System.Net.Http.Headers;
using Minitwit.Infrastructure;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Utils;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection(nameof(SequentialControllerTestCollectionDefinition))]
public class RegisterControllerTests
{
    private const string USERNAME_PREFIX = "RegisterApi";
    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");
    }

    [Theory]
    [InlineData($"{USERNAME_PREFIX}a", "a@a.a", "a", 1)]
    [InlineData($"{USERNAME_PREFIX}b", "b@b.b", "b", 5)]
    [InlineData($"{USERNAME_PREFIX}c", "c@c.c", "c", 6)]
    public async Task Register_WithValidForm_ReturnsNoContentStatusCode(string username, string email, string password, int latest)
    {
        var response = await _client.RegisterUserAsync(username, email, password, latest: latest);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(NoContent, response.StatusCode);
    }

    [Theory]
    [InlineData(@"|{[6I:*{*|", @"SHA512$61D373AC08318F093DEA7A7261D7F4F2$21D123F3C209A6E806510B7F53D493E2BB2D3C7FBDC2B0B8B723CAEF7F6D2C8F")]
    [InlineData(@"^(@{Je=oj", @"SHA512$DE99F7A0D269092104B185371EDDD3DC$9EDEFE4D46DB8FD2A2E3D3DAD1A87084E56B9114F1DC3888BFEA19DB2F1C0F73")]
    [InlineData(@"as<Beb`?", @"SHA512$C74E7AC69F1731CDD889FA6BBE7F6F1D$4AB3EE1858AA18A292F31766A94E245260D3B8FEB8F69091646F5EC5F1FBDFE3")]
    public void PasswordHash_GeneratePassword_algo512_returnsTrue(string password, string passwordHash)
    {
        Assert.True(PasswordHash.CheckPasswordHash(password, passwordHash));
    }
    [Theory]
    [InlineData(@"|{[6I:*{*|", @"47F7BA030E703B84604323A58561D50A$559E79B9EC47D9F8425994977636C6DC61DD900581E5766D88816B0C8411A255")]
    [InlineData(@"^(@{Je=oj", @"EE4FA899AA7EE58BBB5EABD599B19B75$EAE78CF593D71572C618917ADC4979C1708D94C380FFD3A87A0FCB9BCB245647")]
    [InlineData(@"as<Beb`?", @"285A98F9E13DE95B31176C20A68884C4$35603E3A3EF0A97746888228AED10999AA65E04C35FD1C1E873E3E487937E102")]
    public void PasswordHash_GeneratePassword_algo256_returnsTrue(string password, string passwordHash)
    {
        Assert.True(PasswordHash.CheckPasswordHash(password, passwordHash));
    }
    [Theory]
    [InlineData(@"|{[6I:*{*|", @"SHA512$47F7BA030E703B84604323A58561D50A$559E79B9EC47D9F8425994977636C6DC61DD900581E5766D88816B0C8411A255")]
    [InlineData(@"^(@{Je=oj", @"SHA512$EE4FA899AA7EE58BBB5EABD599B19B75$EAE78CF593D71572C618917ADC4979C1708D94C380FFD3A87A0FCB9BCB245647")]
    [InlineData(@"as<Beb`?", @"SHA512$285A98F9E13DE95B31176C20A68884C4$35603E3A3EF0A97746888228AED10999AA65E04C35FD1C1E873E3E487937E102")]
    public void PasswordHash_GeneratePassword_algo512_returnsFalse(string password, string passwordHash)
    {
        Assert.False(PasswordHash.CheckPasswordHash(password, passwordHash));
    }

    [Theory]
    [InlineData(@"|{[6I:*{*|", @"61D373AC08318F093DEA7A7261D7F4F2$21D123F3C209A6E806510B7F53D493E2BB2D3C7FBDC2B0B8B723CAEF7F6D2C8F")]
    [InlineData(@"^(@{Je=oj", @"DE99F7A0D269092104B185371EDDD3DC$9EDEFE4D46DB8FD2A2E3D3DAD1A87084E56B9114F1DC3888BFEA19DB2F1C0F73")]
    [InlineData(@"as<Beb`?", @"C74E7AC69F1731CDD889FA6BBE7F6F1D$4AB3EE1858AA18A292F31766A94E245260D3B8FEB8F69091646F5EC5F1FBDFE3")]
    public void PasswordHash_GeneratePassword_algo256_returnsFalse(string password, string passwordHash)
    {
        Assert.False(PasswordHash.CheckPasswordHash(password, passwordHash));
    }
    
}
