using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DTOs;
using Wallet.Core.Services;

namespace Wallet.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TransferController(ITransferService transferService) : ControllerBase
{
    [HttpPost]
    public async Task Create([FromBody] CreateTransferRequest request) => await transferService.Create(request);

    [HttpGet]
    public async Task<GetTransfersResponse> GetBalance(GetTransfersRequest request) => await transferService.Get(request);
}