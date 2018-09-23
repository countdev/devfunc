using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models.UserViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O Email de usuário é obrigatório."), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Password { get; set; }
    }
}