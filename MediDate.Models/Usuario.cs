using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        [DisplayName("Correo")]
        public string Email { get; set; }
        [DisplayName("Contraseña")]
        public string Password { get; set; }
        public char TipoUsuario { get; set; }
    }
}
