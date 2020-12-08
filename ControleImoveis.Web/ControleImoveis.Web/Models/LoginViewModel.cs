using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleImoveis.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o Login")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Informe o Senha")]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Display(Name = "Lembrar Me")]
        public bool LembrarMe { get; set; }

    }
}