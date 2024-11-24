﻿using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web.Helpers;

namespace MediDate.Controllers
{
    public class MedicoController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<MedicoController> _logger;

        public string archivoBase64;

        public MedicoController(ILogger<MedicoController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
            try
            {
                return View();

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible Iniciar Sesión. " + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }

        public IActionResult Create()
        {
            ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Medico medico, string Email, string Pass, IFormFile archivo)
        {

            try
            {

                //Verificar si el correo ingresado ya existe
                var result = _database.Usuarios.VerificarEmail(Email);

                //Si no existe el correo
                if (!result.Success)
                {
                    //Guardamos los datos del Medico temporalmente
                    TempData["Email"] = Email;
                    TempData["Pass"] = Pass;

                    TempData["Nombre"] = medico.Nombre;
                    TempData["PrimerApellido"] = medico.PrimerApellido;
                    TempData["SegundoApellido"] = medico.SegundoApellido;
                    TempData["IdEspecialidad"] = medico.IdEspecialidad;
                    TempData["NumCedula"] = medico.NumCedula;
                    TempData["Telefono"] = medico.Telefono;

                    //Guardamos los datos del Archivo temporalmente
                    Archivo archivo1 = new Archivo();
                    
                    //MemoryStream ms = new MemoryStream();
                    string ext = Path.GetExtension(archivo.FileName);

                    //if (archivo != null && archivo.Length > 0)
                    //{
                    //    using (var memoryStream = new MemoryStream())
                    //    {
                    //        // Convertir el archivo a byte[]
                    //        archivo.CopyTo(memoryStream);
                    //        byte[] archivoBytes = memoryStream.ToArray();

                    //        // Convertir byte[] a Base64 string
                    //        archivoBase64 = Convert.ToBase64String(archivoBytes);
                    //    }
                    //}

                    if (archivo != null && archivo.Length > 0)
                    {
                        // Ruta temporal para guardar el archivo
                        var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");

                        // Asegúrate de que la carpeta exista
                        if (!Directory.Exists(tempFolder))
                        {
                            Directory.CreateDirectory(tempFolder);
                        }

                        // Crear una ruta única para el archivo
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
                        var filePath = Path.Combine(tempFolder, uniqueFileName);

                        // Guardar el archivo en el servidor
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            archivo.CopyTo(stream);
                        }

                        // Pasar la ruta del archivo a otro controlador usando TempData o RedirectToAction
                        TempData["RutaArchivo"] = filePath;
                    }

                        //archivo1.NombreArchivo = "cedula_profesional_ " + medico.Nombre[0] + "" + medico.PrimerApellido[0] + "" + medico.SegundoApellido[0];
                        //archivo1.Extension = ext;
                        //archivo1.ArchivoByte = archivoBytes;

                    TempData["NombreArchivo"] = "cedula_profesional_ " + medico.Nombre[0] + "" + medico.PrimerApellido[0] + "" + medico.SegundoApellido[0]; ;
                    TempData["ExtArchivo"] = ext;
                    //TempData["ArchivoBase64"] = archivoBase64;

                    //Mostramos la vista para agregar la información del Consultorio
                    return RedirectToAction("Create", "Consultorio");

                }
                else
                {
                    //Si el correo si existe
                    TempData["AlertMessage"] = result.Message;
                    ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                    return View(medico);
                }

                

                ////Crear Usuario en la tabla Usuarios
                //var resultUsuario = _database.Usuarios.Create(Email, Pass, 'M');

                //if (!resultUsuario.Success)
                //{
                //    TempData["AlertMessage"] = "No fue posible crear el Usuario";
                //    ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                //    return View(medico);
                //}
                //else
                //{
                //    //Obtenemos el ultimo IdUsuario que se creó
                //    var usuario = _database.Usuarios.GetLastUser();

                //    medico.IdUsuario = usuario.IdUsuario;

                //    //Crear Medico en la tabla Medicos
                //    var result = _database.Medicos.Create(medico);

                //    if (!result.Success)
                //    {
                //        TempData["AlertMessage"] = "No fue posible crear el Usuario";
                //        ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                //        return View(medico);
                //    }
                //    else
                //    {
                //        TempData["SuccessMessage"] = result.Message;
                //        //Mostramos la vista para agregar la información del Consultorio
                //        return RedirectToAction("Create", "Consultorio");
                //    }
                //}
            }
            catch (Exception e)
            {
                //Console.WriteLine("Excepción: " + e.Message);
                TempData["ErrorMessage"] = "No fue posible crear el Usuario. " + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }

        public IActionResult Details()
        {
            //Guardamos el dato de Email
            if (Request.Cookies.TryGetValue("Email", out string strEmail))
            {
                ViewBag.Email = strEmail;
            }

            //Buscamos la información por el IdMedico
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {
                int IdMedico = Int32.Parse(strIdMedico);

                var medico = _database.Medicos.GetById(IdMedico);

                if (medico == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(medico);
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Error al cargar Configuración de la Cuenta";
                return RedirectToAction("Index", "Medico");
            }
        }

        public IActionResult Edit()
        {

            //Buscamos la información por el IdMedico
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {
                int IdMedico = Int32.Parse(strIdMedico);

                var medico = _database.Medicos.GetById(IdMedico);

                if (medico == null)
                {
                    return NotFound();
                }
                else
                {
                    ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                    return View(medico);
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Error al cargar Editar Cuenta";
                return RedirectToAction("Details", "Medico");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Medico medico)
        {
            try
            {
                //Guardamos el IdMedico
                if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                {
                    medico.IdMedico = Int32.Parse(strIdMedico);

                    var result = _database.Medicos.Edit(medico);

                    if (!result.Success)
                    {
                        TempData["AlertMessage"] = "La información no se pudo actualizar.";
                        return View(medico);
                    }

                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Details", "Medico");

                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar Editar Cuenta";
                    return RedirectToAction("Index", "Medico");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error al actualizar la información. " + e.Message;
                return RedirectToAction("IndexPaciente", "Cita");
            }
        }

        public IActionResult DetailsVerMas(int IdMedico)
        {
            //Buscamos la información por el IdMedico
            var medico = _database.Medicos.GetById(IdMedico);

            if (medico == null)
            {
                return NotFound();
            }
            else
            {
                //Guardamos el valor del IdMedico
                Response.Cookies.Append("IdMedico", IdMedico.ToString());

                //Buscamos la informacion del Consultorio por el IdMedico
                var consultorio = _database.Consultorios.GetByIdMedico(IdMedico);

                //Buscamos la informacion del Horario de atención por el IdMedico
                var horarios = _database.Horarios.GetAllByIdMedico(IdMedico);

                //Obtenemos las citas del Medico despues de este día
                var citas = _database.Citas.GetAllByMedicoHD(IdMedico);

                var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                ViewBag.CitasHD = citas;

                ViewBag.NombreCon = consultorio.Descripcion;
                ViewBag.DireccionCon = consultorio.Calle + ", " + consultorio.Colonia + ", " + consultorio.NumExterior + ", " + consultorio.CodigoPostal;
                
                //ViewBag.HoraInicio = horarios.HoraInicio.ToString("hh:mm tt");
                //ViewBag.HoraFin = horarios.HoraFin.ToString("hh:mm tt");
                //ViewBag.Horario = horarios.HoraInicio.ToString("hh:mm tt") + " - " + horarios.HoraFin.ToString("hh:mm tt");
                ViewBag.Horarios = horarios;

                ViewBag.DiasCerrados = diasCerrados;

                return View(medico);
            }

        }

        public IActionResult DetailsHome(int IdMedico)
        {
            //Buscamos la información por el IdMedico
            var medico = _database.Medicos.GetById(IdMedico);

            if (medico == null)
            {
                return NotFound();
            }
            else
            {
                //Guardamos el valor del IdMedico
                Response.Cookies.Append("IdMedico", IdMedico.ToString());

                //Buscamos la informacion del Consultorio por el IdMedico
                var consultorio = _database.Consultorios.GetByIdMedico(IdMedico);

                //Buscamos la informacion del Horario de atención por el IdMedico
                var horarios = _database.Horarios.GetAllByIdMedico(IdMedico);

                //Obtenemos las citas del Medico despues de este día
                var citas = _database.Citas.GetAllByMedicoHD(IdMedico);

                var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                ViewBag.CitasHD = citas;

                ViewBag.NombreCon = consultorio.Descripcion;
                ViewBag.DireccionCon = consultorio.Calle + ", " + consultorio.Colonia + ", " + consultorio.NumExterior + ", " + consultorio.CodigoPostal;

                //ViewBag.HoraInicio = horarios.HoraInicio.ToString("hh:mm tt");
                //ViewBag.HoraFin = horarios.HoraFin.ToString("hh:mm tt");
                //ViewBag.Horario = horarios.HoraInicio.ToString("hh:mm tt") + " - " + horarios.HoraFin.ToString("hh:mm tt");
                ViewBag.Horarios = horarios;
                ViewBag.DiasCerrados = diasCerrados;

                return View(medico);
            }

        }
    }
}