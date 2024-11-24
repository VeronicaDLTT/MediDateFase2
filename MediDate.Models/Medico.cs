using System.ComponentModel;
using System.Globalization;

namespace MediDate.Models
{
    public class Medico
    {

        public int IdMedico { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        [DisplayName("Primer Apellido")]
        public string PrimerApellido { get; set; }
        [DisplayName("Segundo Apellido")]
        public string? SegundoApellido { get; set; }
        public string NombreCompleto { get; set; }
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; }
        [DisplayName("No. de Cédula Profesional")]
        public string NumCedula { get; set; }
        [DisplayName("Número de Celular")]
        public string? Telefono { get; set; }
        public int Estado { get; set; }
        public int IdArchivo { get; set; }

    }
}