using APIsDaTKSH.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public AuthController(IConfiguration configuration, UserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _userService.RegisterUser(model);

        return Ok(new { Message = "Registro bem-sucedido" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _userService.ValidateUser(model.Email, model.Password);

        if (user != null)
        {


            var token = await GenerateJwtTokenAsync(user.Email, user.IsAdmin);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { Message = "Credenciais inválidas" });
    }

    private async Task<string> GenerateJwtTokenAsync(string email, bool isAdmin)
    {
        var clientId = _configuration["AzureAd:ClientId"];
        var clientSecret = _configuration["AzureAd:ClientSecret"];
        var tenantId = _configuration["AzureAd:TenantId"];
        var vaultUri = new Uri(_configuration["KeyVaultName:VaultUri"]);
        var secretName = _configuration["KeyVaultName:SecretName"];

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var keyVaultClient = new SecretClient(vaultUri, credential);

        var secret = await keyVaultClient.GetSecretAsync(secretName);

        var key = Convert.FromBase64String(secret.Value.Value);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
