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
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Net;
using System.Data;

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

    [HttpGet("users")]
    [Authorize]
    public IActionResult GetAllUsers()
    {
        try
        {
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

            if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    [HttpGet("users/{id}")]
    [Authorize]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

            if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
            {
                var user = _userService.GetUserById(id);

                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound(new { Message = "User not found" });
                }
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.RegisterUserAsync(model);

            return Ok(new { Message = "Successful registration" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }
    [HttpDelete("delete/{id}")]
    [Authorize] 
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

            if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
            {
                var deletionResult = await _userService.DeleteUserAsync(id);

                if (deletionResult)
                {
                    return Ok(new { Message = "User deleted successfully" });
                }
                else
                {
                    return NotFound(new { Message = "User not found" });
                }
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, isAdmin) = await _userService.ValidateUserAsync(model.Email, model.PasswordHash);

            if (user != null)
            {
                var token = await GenerateJwtTokenAsync(user.Email, isAdmin);
                return Ok(new { Token = token });
            }


            return Unauthorized(new { Message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    private async Task<string> GenerateJwtTokenAsync(string email, bool isAdmin)
    {
        try
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
            new Claim("IsAdmin", isAdmin.ToString())
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
        catch (Exception ex)
        {
            throw new ApplicationException("Erro na geração do token JWT", ex);
        }
    }
}
