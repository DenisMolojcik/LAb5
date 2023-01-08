﻿using System.ComponentModel.DataAnnotations;

namespace MedicalInstitution.ViewsModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Почта введена неверно")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пароль введен неверно")]     
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfrimPassword { get; set; }
    }
}
