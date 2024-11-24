using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class FAQ
    {
        public int IdFAQ { get; set; }
        [DisplayName("Categoría")]
        public int? IdCategoria { get; set; }
        [DisplayName("Pregunta")]
        public String? DescripcionFAQ { get; set; }
        public String? Respuesta { get; set; }
        public String? Accion {  get; set; }
    }
}
