﻿using System.ComponentModel.DataAnnotations;

namespace MagarichEmailService.Core
{
    public class EmailCredentials
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
