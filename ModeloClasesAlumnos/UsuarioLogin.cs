﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModeloClasesAlumnos
{
    public class UsuarioLogin
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email incorrecto")]
        public string? EmailLogin { get; set; }

        [Required(ErrorMessage = "El campo password es obligatorio")]
        public string? Password { get; set; }

        public Error? error { get; set; }

    }
}
