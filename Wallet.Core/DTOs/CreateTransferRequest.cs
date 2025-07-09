using System.ComponentModel.DataAnnotations;

namespace Wallet.Core.DTOs;

public class CreateTransferRequest
{
    [Required]
    [Range(1, Double.PositiveInfinity)]
    public int ToUserId { get; set; }

    [Required]
    [Range(0.01, Double.PositiveInfinity)]
    public decimal Value { get; set; }
}