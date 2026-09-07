using System;
using Microsoft.EntityFrameworkCore;

namespace GreenTime.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string? GoogleUserId { get; set; }
    public string? AppleUserId { get; set; }
    public string CountryCode { get; set; }
    public bool IsPremium { get; set; }

    public DateTime? PremiumExpiresUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }


}