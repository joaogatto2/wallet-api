using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Wallet.Core.Services;

namespace Wallet.Application.Services;

public class SignedInUserService(IHttpContextAccessor httpContextAccessor) : ISignedInUserService
{
    public int? UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}