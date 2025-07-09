using System.ComponentModel.DataAnnotations;

namespace Wallet.Core.DTOs;

public class LoginRequest
{
    [Required]
    [Range(1, Double.PositiveInfinity)]
    public int UserId { get; set; }    
}