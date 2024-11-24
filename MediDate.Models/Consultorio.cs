using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class Consultorio
    {
        public int IdConsultorio { get; set; }
        public int IdMedico { get; set; }
        [DisplayName("Nombre del Consultorio")]
        public string Descripcion { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        [DisplayName("No. Exterior")]
        public string? NumExterior { get; set; }
        [DisplayName("Código Postal")]
        public string CodigoPostal { get; set; }
        public int IdLocalidad { get; set; }
    }
}
