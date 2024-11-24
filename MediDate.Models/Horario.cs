using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class Horario
    {
        public int IdHorario { get; set; }
        public int IdMedico { get; set; }
        public int Dia { get; set; }
        public string DiaDescripcion { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public int Estado { get; set; }
    }
}
