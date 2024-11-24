using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

//using Microsoft.Build.Framework;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class CitaController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<CitaController> _logger;

        public CitaController(ILogger<CitaController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult IndexMedico()
        {
            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    //Verificamos si tiene la información validada
                    var medico = _database.Medicos.GetById(IdMedico);
                    if (medico.Estado == 1)
                    {
                        //Obtenemos el Horario de atención por IdMedico
                        var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);
                        ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                        return View(_database.Citas.GetAllByMedico(IdMedico));

                    }
                    else
                    {
                        //No tiene información validada
                        TempData["InfoMessage"] = "No tiene acceso a este apartado, hasta que su información sea validada.";
                        return RedirectToAction("Index", "Medico");
                    }

                    
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                    return RedirectToAction("Index", "Medico");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible Iniciar Sesión. " + e.Message;
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult ObtenerCitas(string opcion, DateTime fecha1, DateTime fecha2)
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                DateTime dateValue = DateTime.Now;

                if (((fecha1.Year == 1) && (fecha1.Day == 1) && (fecha1.Month == 1)) && ((fecha2.Year == 1) && (fecha2.Day == 1) && (fecha2.Month == 1)))
                {
                    fecha1 = dateValue;
                    fecha2 = dateValue.AddDays(6);
                }

                ViewBag.Fecha = fecha1.ToString("dd/MM/yyyy");
                ViewBag.Fecha2 = fecha2.ToString("dd/MM/yyyy");

                // Lógica para obtener citas según la opción seleccionada
                if (opcion == "porSemana")
                {
                    var citasPorSemana = _database.Citas.GetAllByMedicoSemana(IdMedico, fecha1, fecha2);

                    // Obtenemos el Horario de atención por IdMedico
                    var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasPorSemana", citasPorSemana);
                }
                else if (opcion == "porDia")
                {
                    var citasPorDia = _database.Citas.GetAllByMedicoDia(IdMedico, fecha1);

                    // Obtenemos el Horario de atención por IdMedico
                    var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasPorDia", citasPorDia);
                }
                else if (opcion == "todas")
                {
                    var citasTodas = _database.Citas.GetAllByMedico(IdMedico);

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasTodas", citasTodas);
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                    return RedirectToAction("Index", "Medico");
                }

            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Medico");
            }
            
        }

        public IActionResult IndexPaciente()
        {
            if (Request.Cookies.TryGetValue("IdPaciente", out string strPaciente))
            {
                int IdPaciente = Int32.Parse(strPaciente);

                return View(_database.Citas.GetAllByPaciente(IdPaciente));
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Paciente");
            }

        }

        public DateTime ObtenerProximoDiaDisponible(int IdMedico)
        {
            // Obtener el horario del médico y los días cerrados de la base de datos
            var horarios = _database.Horarios.GetAllByIdMedico(IdMedico);
            var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

            // Día actual como punto de partida
            DateTime diaActual = DateTime.Now.AddDays(1);

            bool esDiaCerrado = true;

            // Ciclo para encontrar el próximo día disponible
            while (esDiaCerrado)
            {
                esDiaCerrado = diasCerrados.Any(diaCerrado =>
                    (diaCerrado.Fecha2 == null && diaCerrado.Fecha1.Date == diaActual.Date) ||
                    (diaCerrado.Fecha2 != null && diaActual.Date >= diaCerrado.Fecha1.Date && diaActual.Date <= diaCerrado.Fecha2.Value.Date)
                );

                // Verificar si es un día cerrado
                if (!esDiaCerrado)
                {
                    // Obtener el día de la semana para el día actual
                    string diaSemana = diaActual.ToString("dddd", new CultureInfo("es-ES"));

                    // Verificar si el horario del día está disponible
                    var horarioDia = horarios.FirstOrDefault(h => h.DiaDescripcion.ToLower() == diaSemana);

                    if (horarioDia != null && horarioDia.Estado == 0) // Estado = 0 significa abierto
                    {
                        // Si se cumple que no es día cerrado y hay horario disponible, devolver el día actual
                        esDiaCerrado = false;
                    }
                    else
                    {
                        esDiaCerrado = true;
                        // Si no es un día válido, avanzar al siguiente día
                        diaActual = diaActual.AddDays(1);
                    }
                }
                else
                {
                    // Si no es un día válido, avanzar al siguiente día
                    diaActual = diaActual.AddDays(1);
                }

                
            }
            return diaActual;
        }

        public IActionResult CreatePaciente(String? Fecha, String? Hora)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                //Verificamos si existe un valor en IdMedico
                if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                {
                    int IdPaciente = Int32.Parse(strIdPaciente);
                    int IdMedico = Int32.Parse(strIdMedico);

                    //Buscamos los datos del Paciente y del Medico
                    var paciente = _database.Pacientes.GetById(IdPaciente);
                    var medico = _database.Medicos.GetById(IdMedico);

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);

                    Cita cita = new Cita();
                    cita.NombreMedico = medico.NombreCompleto;

                    ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    var serviciosSL = ViewBag.IdDetServicio as SelectList;

                    if (serviciosSL == null || serviciosSL.Count() == 0)
                    {
                        TempData["AlertMessage"] = "Por el momento el especialista no esta disponible para agendar citas. ";
                        return RedirectToAction("Index", "Paciente");
                    }

                    var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                    var diasCerradosList = diasCerrados.Select(dc => new
                    {
                        FechaInicio = dc.Fecha1.ToString("yyyy-MM-dd"),
                        FechaFin = dc.Fecha2?.ToString("yyyy-MM-dd") // Fecha2 podría ser null, por eso usamos `?.`
                    });

                    ViewBag.DiasCerradosJson = JsonConvert.SerializeObject(diasCerradosList);

                    var horariosList = _database.Horarios.GetAllByIdMedico(IdMedico);

                    // Filtrar los dias cerrados del horario de atención (Estado = 1)
                    var horariosCerrados = horariosList
                        .Where(h => h.Estado == 1) // Estado 1 indica que el día está cerrado
                        .GroupBy(h => h.Dia)
                        .Select(g => new
                        {
                            Dia = g.First().DiaDescripcion,
                            Horarios = g.Select(h => new
                            {
                                HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                HoraFin = h.HoraFin.ToString("HH:mm")
                            }).ToList()
                        }).ToList();

                    ViewBag.HorariosCerradosJson = JsonConvert.SerializeObject(horariosCerrados);

                    // Agrupar los horarios de ateción por día (los dias abiertos)
                    var horariosAbiertos = horariosList
                        .Where(h => h.Estado == 0) // Estado 0 indica que el día está abierto
                        .GroupBy(h => h.Dia)
                        .Select(g => new
                        {
                            Dia = g.First().DiaDescripcion,
                            Horarios = g.Select(h => new
                            {
                                HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                HoraFin = h.HoraFin.AddHours(-1).ToString("HH:mm")
                            }).ToList()
                        }).ToList();

                    ViewBag.HorariosAbiertosJson = JsonConvert.SerializeObject(horariosAbiertos);

                    ViewBag.ProximoDiaDisponible = ObtenerProximoDiaDisponible(IdMedico);

                    if (Fecha == null)
                    {
                        cita.Fecha = ViewBag.ProximoDiaDisponible;
                    }
                    else
                    {
                        cita.Fecha = DateTime.Parse(Fecha);
                    }

                    if (Hora == null)
                    {
                        cita.Hora = horario.HoraInicio;
                    }
                    else
                    {
                        cita.Hora = DateTime.Parse(Hora);
                    }

                    return View(cita);
                }
                else
                {
                    return RedirectToAction("Index", "Paciente");
                }

            }
            else
            {
                TempData["IniciarSesion"] = "Inicia sesión o Registrate para agendar citas. ";
                return RedirectToAction("Login", "Usuario");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePaciente(Cita cita)
        {
            try
            {
                //Verificamos si hay un Paciente que ha iniciado sesion
                if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
                {
                    //Verificamos si existe un valor en IdMedico
                    if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                    {
                        int IdPaciente = Int32.Parse(strIdPaciente);
                        int IdMedico = Int32.Parse(strIdMedico);

                        //Buscamos el horario de atencion
                        var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);

                        cita.IdPaciente = IdPaciente;
                        cita.IdMedico = IdMedico;
                        cita.Estado = 1;

                        //Verificamos si se puede agendar la cita
                        var verificar = _database.Citas.Verificar(cita);

                        if (verificar.Success)
                        {
                            TempData["AlertMessage"] = verificar.Message;
                            ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                            ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                            ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                            var serviciosSL = ViewBag.IdDetServicio as SelectList;

                            if (serviciosSL == null || serviciosSL.Count() == 0)
                            {
                                TempData["AlertMessage"] = "Por el momento el especialista no esta disponible para agendar citas. ";
                                return RedirectToAction("Index", "Paciente");
                            }

                            var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                            var diasCerradosList = diasCerrados.Select(dc => new {
                                FechaInicio = dc.Fecha1.ToString("yyyy-MM-dd"),
                                FechaFin = dc.Fecha2?.ToString("yyyy-MM-dd") // Fecha2 podría ser null, por eso usamos `?.`
                            });

                            ViewBag.DiasCerradosJson = JsonConvert.SerializeObject(diasCerradosList);

                            var horariosList = _database.Horarios.GetAllByIdMedico(IdMedico);

                            // Filtrar los dias cerrados del horario de atención (Estado = 1)
                            var horariosCerrados = horariosList
                                .Where(h => h.Estado == 1) // Estado 1 indica que el día está cerrado
                                .GroupBy(h => h.Dia)
                                .Select(g => new
                                {
                                    Dia = g.First().DiaDescripcion,
                                    Horarios = g.Select(h => new
                                    {
                                        HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                        HoraFin = h.HoraFin.ToString("HH:mm")
                                    }).ToList()
                                }).ToList();

                            ViewBag.HorariosCerradosJson = JsonConvert.SerializeObject(horariosCerrados);

                            // Agrupar los horarios de ateción por día (los dias abiertos)
                            var horariosAbiertos = horariosList
                                .Where(h => h.Estado == 0) // Estado 0 indica que el día está abierto
                                .GroupBy(h => h.Dia)
                                .Select(g => new
                                {
                                    Dia = g.First().DiaDescripcion,
                                    Horarios = g.Select(h => new
                                    {
                                        HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                        HoraFin = h.HoraFin.AddHours(-1).ToString("HH:mm")
                                    }).ToList()
                                }).ToList();

                            ViewBag.HorariosAbiertosJson = JsonConvert.SerializeObject(horariosAbiertos);

                            ViewBag.ProximoDiaDisponible = ObtenerProximoDiaDisponible(IdMedico);

                            return View(cita);
                            
                        }
                        else
                        {
                            //Creamos la cita
                            var result = _database.Citas.Create(cita);

                            //Si no se pudo crear la cita
                            if (!result.Success)
                            {
                                TempData["ErrorMessage"] = "Error al agendar la cita.";
                                ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");
                                return View(cita);
                            }
                            else
                            {
                                TempData["SuccessMessage"] = result.Message;
                                return RedirectToAction("IndexPaciente", "Cita");
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Paciente");
                    }

                }
                else
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("IndexPaciente", "Cita");
            }
        }

        public IActionResult CreateMedico()
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                int IdMedico = Int32.Parse(strIdMedico);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);

                ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico),"IdDetServicio","Servicio");
                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                var selectList = ViewBag.IdDetServicio as SelectList;

                if (selectList == null || selectList.Count() == 0)
                {
                    TempData["AlertMessage"] = "No puedes agendar citas ya que no tienes Servicios registrados. Agrega un Servicio para poder agendar. ";
                    return RedirectToAction("Index", "DetServicio");
                }

                var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                var diasCerradosList = diasCerrados.Select(dc => new {
                    FechaInicio = dc.Fecha1.ToString("yyyy-MM-dd"),
                    FechaFin = dc.Fecha2?.ToString("yyyy-MM-dd") // Fecha2 podría ser null, por eso usamos `?.`
                });

                ViewBag.DiasCerradosJson = JsonConvert.SerializeObject(diasCerradosList);

                var horariosList = _database.Horarios.GetAllByIdMedico(IdMedico);

                // Filtrar los dias cerrados del horario de atención (Estado = 1)
                var horariosCerrados = horariosList
                    .Where(h => h.Estado == 1) // Estado 1 indica que el día está cerrado
                    .GroupBy(h => h.Dia)
                    .Select(g => new
                    {
                        Dia = g.First().DiaDescripcion,
                        Horarios = g.Select(h => new
                        {
                            HoraInicio = h.HoraInicio.ToString("HH:mm"),
                            HoraFin = h.HoraFin.ToString("HH:mm")
                        }).ToList()
                    }).ToList();

                ViewBag.HorariosCerradosJson = JsonConvert.SerializeObject(horariosCerrados);

                // Agrupar los horarios de ateción por día (los dias abiertos)
                var horariosAbiertos = horariosList
                    .Where(h => h.Estado == 0) // Estado 0 indica que el día está abierto
                    .GroupBy(h => h.Dia)
                    .Select(g => new
                    {
                        Dia = g.First().DiaDescripcion,
                        Horarios = g.Select(h => new
                        {
                            HoraInicio = h.HoraInicio.ToString("HH:mm"),
                            HoraFin = h.HoraFin.AddHours(-1).ToString("HH:mm")
                        }).ToList()
                    }).ToList();

                ViewBag.HorariosAbiertosJson = JsonConvert.SerializeObject(horariosAbiertos);

                ViewBag.ProximoDiaDisponible = ObtenerProximoDiaDisponible(IdMedico);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Medico");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMedico(Cita cita, string txtBuscar)
        {
            
            //Verificamos si existe un valor en IdMedico
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {
                var strPaciente = txtBuscar.Split('-');
                string strIdPaciente = strPaciente[0];

                int IdMedico = Int32.Parse(strIdMedico);
                int IdPaciente = Int32.Parse(strIdPaciente);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetHoraMinMaxByIdMedico(IdMedico);

                cita.IdPaciente = IdPaciente;
                cita.IdMedico = IdMedico;
                cita.Estado = 1;

                //Verificamos si se puede agendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    //Creamos la cita
                    var result = _database.Citas.Create(cita);

                    //Si no se pudo crear la cita
                    if (!result.Success)
                    {
                        TempData["ErrorMessage"] = "Error al agendar la cita.";
                        ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                        var selectList = ViewBag.IdDetServicio as SelectList;

                        if (selectList == null || selectList.Count() == 0)
                        {
                            TempData["AlertMessage"] = "No puedes agendar citas ya que no tienes Servicios registrados. Agrega un Servicio para poder agendar. ";
                            return RedirectToAction("Index", "DetServicio");
                        }

                        var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                        var diasCerradosList = diasCerrados.Select(dc => new {
                            FechaInicio = dc.Fecha1.ToString("yyyy-MM-dd"),
                            FechaFin = dc.Fecha2?.ToString("yyyy-MM-dd") // Fecha2 podría ser null, por eso usamos `?.`
                        });

                        ViewBag.DiasCerradosJson = JsonConvert.SerializeObject(diasCerradosList);

                        var horariosList = _database.Horarios.GetAllByIdMedico(IdMedico);

                        // Filtrar los dias cerrados del horario de atención (Estado = 1)
                        var horariosCerrados = horariosList
                            .Where(h => h.Estado == 1) // Estado 1 indica que el día está cerrado
                            .GroupBy(h => h.Dia)
                            .Select(g => new
                            {
                                Dia = g.First().DiaDescripcion,
                                Horarios = g.Select(h => new
                                {
                                    HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                    HoraFin = h.HoraFin.ToString("HH:mm")
                                }).ToList()
                            }).ToList();

                        ViewBag.HorariosCerradosJson = JsonConvert.SerializeObject(horariosCerrados);

                        // Agrupar los horarios de ateción por día (los dias abiertos)
                        var horariosAbiertos = horariosList
                            .Where(h => h.Estado == 0) // Estado 0 indica que el día está abierto
                            .GroupBy(h => h.Dia)
                            .Select(g => new
                            {
                                Dia = g.First().DiaDescripcion,
                                Horarios = g.Select(h => new
                                {
                                    HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                    HoraFin = h.HoraFin.AddHours(-1).ToString("HH:mm")
                                }).ToList()
                            }).ToList();

                        ViewBag.HorariosAbiertosJson = JsonConvert.SerializeObject(horariosAbiertos);

                        ViewBag.ProximoDiaDisponible = ObtenerProximoDiaDisponible(IdMedico);

                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexMedico", "Cita");

                    }
                }
                else
                {
                    TempData["AlertMessage"] = verificar.Message;
                    ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    var selectList = ViewBag.IdDetServicio as SelectList;

                    if (selectList == null || selectList.Count() == 0)
                    {
                        TempData["AlertMessage"] = "No puedes agendar citas ya que no tienes Servicios registrados. Agrega un Servicio para poder agendar. ";
                        return RedirectToAction("Index", "DetServicio");
                    }

                    var diasCerrados = _database.DiasCerrados.GetAllByIdMedico(IdMedico);

                    var diasCerradosList = diasCerrados.Select(dc => new {
                        FechaInicio = dc.Fecha1.ToString("yyyy-MM-dd"),
                        FechaFin = dc.Fecha2?.ToString("yyyy-MM-dd") // Fecha2 podría ser null, por eso usamos `?.`
                    });

                    ViewBag.DiasCerradosJson = JsonConvert.SerializeObject(diasCerradosList);

                    var horariosList = _database.Horarios.GetAllByIdMedico(IdMedico);

                    // Filtrar los dias cerrados del horario de atención (Estado = 1)
                    var horariosCerrados = horariosList
                        .Where(h => h.Estado == 1) // Estado 1 indica que el día está cerrado
                        .GroupBy(h => h.Dia)
                        .Select(g => new
                        {
                            Dia = g.First().DiaDescripcion,
                            Horarios = g.Select(h => new
                            {
                                HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                HoraFin = h.HoraFin.ToString("HH:mm")
                            }).ToList()
                        }).ToList();

                    ViewBag.HorariosCerradosJson = JsonConvert.SerializeObject(horariosCerrados);

                    // Agrupar los horarios de ateción por día (los dias abiertos)
                    var horariosAbiertos = horariosList
                        .Where(h => h.Estado == 0) // Estado 0 indica que el día está abierto
                        .GroupBy(h => h.Dia)
                        .Select(g => new
                        {
                            Dia = g.First().DiaDescripcion,
                            Horarios = g.Select(h => new
                            {
                                HoraInicio = h.HoraInicio.ToString("HH:mm"),
                                HoraFin = h.HoraFin.AddHours(-1).ToString("HH:mm")
                            }).ToList()
                        }).ToList();

                    ViewBag.HorariosAbiertosJson = JsonConvert.SerializeObject(horariosAbiertos);

                    ViewBag.ProximoDiaDisponible = ObtenerProximoDiaDisponible(IdMedico);
                    return View(cita);
                }
            }
            else
            {
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditPaciente(int IdCita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                
                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetHoraMinMaxByIdMedico(cita.IdMedico);

                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                //cita.Fecha = DateTime.Now.AddDays(1);

                return View(cita);
            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPaciente(Cita cita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {

                //Verificamos si se puede reagendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    cita.Estado = 1;

                    //Reagendamos la cita
                    var result = _database.Citas.Edit(cita);

                    if (!result.Success)
                    {
                        //No se logro reagendar la cita
                        TempData["AlertMessage"] = "Error al reagendar la cita, intente de nuevo. ";

                        //Buscamos el horario de atencion
                        var horario = _database.Horarios.GetHoraMinMaxByIdMedico(cita.IdMedico);

                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexPaciente", "Cita");
                    }

                }
                else
                {
                    //No se puede reagendar, ya existe una cita en esa fecha y hora
                    TempData["AlertMessage"] = verificar.Message;

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetHoraMinMaxByIdMedico(cita.IdMedico);

                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    return View(cita);
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditMedico(int IdCita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetHoraMinMaxByIdMedico(cita.IdMedico);

                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                //cita.Fecha = DateTime.Now.AddDays(1);

                return View(cita);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMedico(Cita cita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Verificamos si se puede reagendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    cita.Estado = 1;

                    //Reagendamos la cita
                    var result = _database.Citas.Edit(cita);

                    if (!result.Success)
                    {
                        //No se logro reagendar la cita
                        TempData["AlertMessage"] = "Error al reagendar la cita, intente de nuevo. ";

                        //Buscamos el horario de atencion
                        var horario = _database.Horarios.GetHoraMinMaxByIdMedico(cita.IdMedico);

                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexMedico", "Cita");
                    }

                }
                else
                {
                    //No se puede reagendar, ya existe una cita en esa fecha y hora
                    TempData["AlertMessage"] = verificar.Message;

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    return View(cita);
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditEstado(int Estado, int IdCita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Actualizamos el estadp
                var result = _database.Citas.EditEstado(IdCita, Estado);

                if (!result.Success)
                {
                    //No se logro editar el estado la cita
                    TempData["AlertMessage"] = "Error al cambiar el estado de la cita. ";
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                return RedirectToAction("IndexMedico", "Cita");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeletePaciente(int IdCita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);
                return View(cita);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePacienteConfirmed(int IdCita)
        {
            //Eliminamos cita
            //var result = _database.Citas.Delete(IdCita);

            //Cambiar estado de Cita a Cancelada
            var result = _database.Citas.EditEstado(IdCita, 5);

            if (!result.Success)
            {
                TempData["AlertMessage"] = "Error al cancelar la cita. Intente de nuevo. ";
                return View(IdCita);
            }
            else
            {
                TempData["SuccessMessage"] = "¡Cita cancelada exitosamente!";
                return RedirectToAction("IndexPaciente", "Cita");
            }
        }

        public IActionResult DeleteMedico(int IdCita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdPaciente))
            {
                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);
                return View(cita);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMedicoConfirmed(int IdCita)
        {
            //Eliminamos cita
            //var result = _database.Citas.Delete(IdCita);

            //Cambiar estado de Cita a Cancelada
            var result = _database.Citas.EditEstado(IdCita, 5);

            if (!result.Success)
            {
                TempData["AlertMessage"] = "Error al cancelar la cita. Intente de nuevo. ";
                return View(IdCita);
            }
            else
            {
                TempData["SuccessMessage"] = "¡Cita cancelada exitosamente!";
                return RedirectToAction("IndexMedico", "Cita");
            }
        }

        public IActionResult Historial()
        {
            if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
            {
                ViewBag.TipoUsuario = strTipoUsuario;
                if (strTipoUsuario.Equals("P"))
                {
                    //Obtenemos IdPaciente
                    if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
                    {
                        int IdPaciente = Int32.Parse(strIdPaciente);

                        //Mostramos el historial por Paciente
                        return View(_database.Citas.GetHistorialByPaciente(IdPaciente));
                    }
                    else
                    {
                        //No hay información del Paciente
                        TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                        return RedirectToAction("Index", "Paciente");
                    }

                }
                else if (strTipoUsuario.Equals("M"))
                {
                    //Obtenemos IdMedico
                    if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                    {
                        int IdMedico = Int32.Parse(strIdMedico);

                        //Mostramos el historial por Medico
                        return View(_database.Citas.GetHistorialByMedico(IdMedico));
                    }
                    else
                    {
                        //No hay información del Medico
                        TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                        return RedirectToAction("Index", "Medico");
                    }

                }
                else if (strTipoUsuario.Equals("A"))
                {
                    //Mostramos todas las citas
                    return View(_database.Citas.GetAllHistorial());
                }
                else
                {
                    //No hay información del Usuario
                    TempData["ErrorMessage"] = "No se logró obtener la información del Usuario.";
                    return RedirectToAction("LogOut", "Usuario");
                }
                    
            }
            else
            {
                //No hay información del Usuario
                TempData["ErrorMessage"] = "No se logró obtener la información del Usuario.";
                return RedirectToAction("LogOut", "Usuario");
            }

        }

        [HttpGet]
        public JsonResult GetDropdownData(string txtBuscar)
        {
            var pacientes = _database.Pacientes.GetAllPacientesCorreos(txtBuscar);

            var pacientesStrings = pacientes.Select(e => e.NombreCompleto).ToList();

            var dropdownData = new List<string>();
            dropdownData.AddRange(pacientesStrings);

            return Json(dropdownData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}