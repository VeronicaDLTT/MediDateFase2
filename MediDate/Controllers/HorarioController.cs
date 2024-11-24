using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class HorarioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<HorarioController> _logger;
        public int i;

        public HorarioController(ILogger<HorarioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public JsonResult Create(List<TimeSpan> HoraInicio,
                                    List<TimeSpan> HoraFin,
                                    List<bool> chkLunes,
                                    List<bool> chkMartes,
                                    List<bool> chkMiercoles,
                                    List<bool> chkJueves,
                                    List<bool> chkViernes,
                                    List<bool> chkSabado,
                                    List<bool> chkDomingo,
                                    List<bool> Cerrado)
        {
            i = 0;

            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                //Obtenemos el Id del Medico
                int IdMedico = Int32.Parse(strMedico);

                List<int> DiaSemanaNum = new List<int>();
                List<TimeSpan> HorasInicio = new List<TimeSpan>();
                List<TimeSpan> HorasFin = new List<TimeSpan>();
                List<bool> Estados = new List<bool>();

                //Agregamos los siete horarios de cada dia que pertenecen al Medico
                for (i = 0; i < HoraInicio.Count(); i++)
                {
                    //Recorremos los dias de la semana
                    if (chkLunes[i])
                    {
                        //Horario Lunes Cerrado
                        DiaSemanaNum.Add(1);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkMartes[i])
                    {
                        //Horario Martes Cerrado
                        DiaSemanaNum.Add(2);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkMiercoles[i])
                    {
                        //Horario Miércoles Cerrado
                        DiaSemanaNum.Add(3);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkJueves[i])
                    {
                        //Horario Jueves Cerrado
                        DiaSemanaNum.Add(4);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkViernes[i])
                    {
                        //Horario Viernes Cerrado
                        DiaSemanaNum.Add(5);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkSabado[i])
                    {
                        //Horario Sábado Cerrado
                        DiaSemanaNum.Add(6);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }

                    if (chkDomingo[i])
                    {
                        //Horario Domingo Cerrado
                        DiaSemanaNum.Add(7);
                        HorasInicio.Add(HoraInicio[i]);
                        HorasFin.Add(HoraFin[i]);
                        Estados.Add(Cerrado[i]);
                    }
                }

                //Agregamos todos los horarios
                for (int j = 0; j < DiaSemanaNum.Count(); j++)
                {
                    var result = _database.Horarios.Create(IdMedico, DiaSemanaNum[j], HorasInicio[j], HorasFin[j], Estados[j]);

                    if (!result.Success)
                    {

                        TempData["AlertMessage"] = "Error al registrar el horario. Intente de nuevo. ";
                        i = DiaSemanaNum.Count();
                        return Json(true);
                    }

                    TempData["SuccessMessage"] = result.Message;
                }

                return Json(true);

            }
            else
            {
                TempData["ErrorMessage"] = "Ocurrió un error al registrar el Horario de Atención, intente de nuevo.";
                return Json(false);
            }

        }

        public IActionResult Edit(int IdHorario)
        {
            //Buscamos el Horario por su Id
            var horario = _database.Horarios.GetById(IdHorario);

            if (horario == null)
            {
                return NotFound();
            }
            else
            {
                if (horario.Estado == 0)
                {
                    ViewBag.EstadoBool = false;
                }
                else
                {
                    ViewBag.EstadoBool = true;
                }

                return View(horario);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Horario horario, TimeSpan HoraInicio, TimeSpan HoraFin, bool Cerrado)
        {
            if (Cerrado)
            {
                horario.Estado = 1;
            }
            else
            {
                horario.Estado = 0;
            }

            //Actualizamos el Horario
            var result = _database.Horarios.Edit(horario, HoraInicio, HoraFin);

            if (!result.Success)
            {

                TempData["AlertMessage"] = "Error al modificar el horario. Intente de nuevo. ";
                return View(horario);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Horario");
        }

        public IActionResult Details()
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);
                
                //Buscamos el horario por IdMedico
                var horario = _database.Horarios.GetAllByIdMedico(IdMedico);

                if (horario == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(horario);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar el horario de atención. ";
                return RedirectToAction("IndexMedico", "Cita");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}