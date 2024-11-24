using System.Globalization;

namespace MediDate.Models
{
    public class Archivo
    {

        public int IdArchivo { get; set; }
        public string? NombreArchivo { get; set; }
        public byte[] ArchivoByte {  get; set; }
        public string Extension { get; set; }
    }
}