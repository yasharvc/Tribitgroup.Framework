﻿using System.ComponentModel.DataAnnotations;
using Tribitgroup.Framework.Shared.DTO;

namespace Tribitgroup.Framework.Identity.Shared.DTO
{
    public class RegisterWithUsernameEmailPasswordInputDTO : InputDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
