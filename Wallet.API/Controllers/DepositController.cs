using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DTOs;
using Wallet.Core.Services;

namespace Wallet.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DepositController(IDepositService depositService) : ControllerBase
{
    [HttpPost]
    public async Task Deposit([FromBody] DepositRequest request) => await depositService.Deposit(request.Value);
}