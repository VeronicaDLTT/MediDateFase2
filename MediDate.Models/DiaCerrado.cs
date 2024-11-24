using System.ComponentModel;
using System.Globalization;

namespace MediDate.Models
{
    public class DiaCerrado
    {

        public int IdDiaCerrado { get; set; }
        public int IdMedico { get; set; }
        [DisplayName("Del día")]
        public DateTime Fecha1 { get; set; }
        [DisplayName("Al día")]
        public DateTime? Fecha2 { get; set; }
    }
}
