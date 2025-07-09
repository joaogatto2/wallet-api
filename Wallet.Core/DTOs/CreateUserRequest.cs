using System.ComponentModel.DataAnnotations;

namespace Wallet.Core.DTOs;

public class CreateUserRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}