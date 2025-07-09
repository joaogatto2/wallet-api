using System.ComponentModel.DataAnnotations;

namespace Wallet.Core.DTOs;

public class DepositRequest
{
    [Required]
    [Range(0.01, Double.PositiveInfinity)]
    public decimal Value { get; set; }
}