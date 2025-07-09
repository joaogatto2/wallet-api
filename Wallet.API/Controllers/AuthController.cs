using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DTOs;
using Wallet.Core.Services;

namespace Wallet.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginRequest request) => await authService.GenerateToken(request.UserId);
}