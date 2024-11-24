using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class ConsultorioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<ConsultorioController> _logger;
        //Variables para almacenar los datos de MedicoController
        public Medico medico1 = new Medico();
        public Archivo archivo1 = new Archivo();
        public string Email1;
        public string Pass1;
        public byte[] archivoBytes;
        public ConsultorioController(ILogger<ConsultorioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
           return View();
        }

        
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Consultorio consultorio)
        {

            try
            {
                // Recuperar la ruta del archivo desde TempData
                string rutaArchivo = TempData["RutaArchivo"] as string;

                if (!string.IsNullOrEmpty(rutaArchivo) && System.IO.File.Exists(rutaArchivo))
                {
                    // Aquí puedes leer el archivo, procesarlo, o moverlo a otro directorio
                    archivoBytes = System.IO.File.ReadAllBytes(rutaArchivo);

                    // (Opcional) Eliminar el archivo temporal
                    System.IO.File.Delete(rutaArchivo);
                }
                //Obtenemos los datos temporales de MedicoController
                Email1 = TempData["Email"] as string;
                Pass1 = TempData["Pass"] as string;

                medico1.Nombre = TempData["Nombre"] as string;
                medico1.PrimerApellido = TempData["PrimerApellido"] as string;
                medico1.SegundoApellido = TempData["SegundoApellido"] as string;
                medico1.IdEspecialidad = (int)TempData["IdEspecialidad"];
                medico1.NumCedula = TempData["NumCedula"] as string;
                medico1.Telefono = TempData["Telefono"] as string;

                archivo1.NombreArchivo = TempData["NombreArchivo"] as string;
                archivo1.Extension = TempData["ExtArchivo"] as string;
                //string archBase64 = TempData["ArchivoBase64"] as string;
                //byte[] archBytes = Convert.FromBase64String(archBase64);
                archivo1.ArchivoByte = archivoBytes;
                
                //Creamos el Usuario Medico
                var result = _database.Usuarios.CreateMedicos(Email1, Pass1, medico1, consultorio, archivo1);

                if (!result.Success)
                {
                    TempData["AlertMessage"] = "No fue posible guardar la información del Usuario.";
                    return View(consultorio);
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "Home");
                }

                ////Obtenemos el ultimo IdMedico que se creó
                //var medico = _database.Medicos.GetLastUser();

                //consultorio.IdMedico = medico.IdMedico;

                ////Crear Consultorio en la tabla Consultorios
                //var result = _database.Consultorios.Create(consultorio);

            }
            catch (Exception e)
            {
                //Console.WriteLine("Excepción: " + e.Message);
                TempData["ErrorMessage"] = "No fue posible guardar la información del Usuario. " + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }

        public IActionResult Edit(int IdConsultorio)
        {
            //Buscamos el Consultorio por su Id
            var consultorio = _database.Consultorios.GetById(IdConsultorio);

            if (consultorio == null)
            {
                return NotFound();
            }
            else
            {
                return View(consultorio);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Consultorio consultorio)
        {
            //Actualizamos el Horario
            var result = _database.Consultorios.Edit(consultorio);

            if (!result.Success)
            {

                TempData["AlertMessage"] = "Error al modificar la información del Consultorio. Intente de nuevo. ";
                return View(consultorio);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Consultorio");
        }

        public IActionResult Details()
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                //Buscamos el Consultorio por IdMedico
                var consultorio = _database.Consultorios.GetByIdMedico(IdMedico);

                if (consultorio == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(consultorio);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la información del Consultorio. ";
                return RedirectToAction("IndexMedico", "Cita");
            }

        }
    }
}