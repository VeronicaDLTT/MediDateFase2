using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class Receta
    {
        public int IdReceta { get; set; }
        public int IdCita { get; set; }
        public int? IdArchivo { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public String? Peso { get; set; }
        public String? Talla { get; set; }
        public String? Temperatura { get; set; }
        public String? TA { get; set; }
        public String? FC { get; set; }
        public String? FR { get; set; }

        [DisplayName("Saturación")]
        public String? Saturacion { get; set; }
        public String? Alergias { get; set; }

        [DisplayName("Diagnóstico")]
        public String? Diagnostico { get; set; }
        public String? Tratamiento { get; set; }
        public String? Observaciones { get; set; }

        //Campos adinicionales
        public String? NombreConsultorio { get; set; }
        public String? DireccionConsultorio { get; set; }
        public String? NombreMedico { get; set; }
        public String? Especialidad { get; set; }
        public String? NumCedula { get; set; }
        public String? NombrePaciente { get; set; }
        public String? Edad {  get; set; }

    }
}
