using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DTOs;
using Wallet.Core.Entities;
using Wallet.Core.Services;

namespace Wallet.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<User> Create([FromBody] CreateUserRequest request) => await userService.Create(request.Name);

    [Authorize]
    [HttpGet("balance")]
    public async Task<decimal> GetBalance() => await userService.GetBalance();
}