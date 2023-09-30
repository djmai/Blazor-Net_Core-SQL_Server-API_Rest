using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModeloClasesAlumnos
{
    public class UsuarioLogin
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        public string EmailLogin { get; set; }

        [Required(ErrorMessage = "El campo password es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email incorrecto")]
        public string EmailPassword { get; set; }
        public Error error { get; set; }

    }
}
