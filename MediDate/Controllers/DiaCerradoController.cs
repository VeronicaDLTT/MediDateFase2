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
    public class DiaCerradoController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<DiaCerradoController> _logger;
        
        public DiaCerradoController(ILogger<DiaCerradoController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    return View(_database.DiasCerrados.GetAllByIdMedico(IdMedico));
                }
                else
                {
                    TempData["ErrorMessage"] = "No fue posible cargar la información.";
                    return RedirectToAction("Index", "Medico");
                }
            }
            catch (Exception e)
            {

                TempData["ErrorMessage"] = "No fue posible cargar la información. " + e.Message;
                return RedirectToAction("Index", "Medico");
            }
            
        }

        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DiaCerrado diaCerrado)
        {

            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    diaCerrado.IdMedico = IdMedico;

                    //Agregamos el dia cerrado
                    var result = _database.DiasCerrados.Create(diaCerrado);

                    if (result.Success)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Index","DiaCerrado");
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Ocurrio un error al realizar el registro, intente de nuevo.";
                        return View(diaCerrado);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No fue posible cargar el registro.";
                    return RedirectToAction("Index", "DiaCerrado");
                }
            }
            catch (Exception e)
            {
                
                TempData["ErrorMessage"] = "No fue posible cargar la información. " + e.Message;
                return RedirectToAction("Index", "DiaCerrado");
            }
        }

        public IActionResult Edit(int IdDiaCerrado)
        {
            //Buscamos el DiaCerrado por su Id
            var DiaCerrado = _database.DiasCerrados.GetById(IdDiaCerrado);

            if (DiaCerrado == null)
            {
                return NotFound();
            }
            else
            {
                return View(DiaCerrado);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DiaCerrado diaCerrado)
        {

            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    //Editamos el dia cerrado
                    var result = _database.DiasCerrados.Edit(diaCerrado);

                    if (result.Success)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Index", "DiaCerrado");
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Ocurrio un error al guardar la información, intente de nuevo.";
                        return View(diaCerrado);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No fue posible cargar la información.";
                    return RedirectToAction("Index", "DiaCerrado");
                }
            }
            catch (Exception e)
            {

                TempData["ErrorMessage"] = "No fue posible cargar la información. " + e.Message;
                return RedirectToAction("Index", "DiaCerrado");
            }
        }

        public IActionResult Details(int IdDiaCerrado)
        {
            //Buscamos el DiaCerrado por su Id
            var DiaCerrado = _database.DiasCerrados.GetById(IdDiaCerrado);

            if (DiaCerrado == null)
            {
                return NotFound();
            }
            else
            {
                return View(DiaCerrado);
            }
        }

        public IActionResult Delete(int IdDiaCerrado)
        {
            //Buscamos el DiaCerrado por su Id
            var DiaCerrado = _database.DiasCerrados.GetById(IdDiaCerrado);

            if (DiaCerrado == null)
            {
                return NotFound();
            }
            else
            {
                return View(DiaCerrado);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDiaCerrado(int IdDiaCerrado)
        {

            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    //Eliminamos el dia cerrado
                    var result = _database.DiasCerrados.Delete(IdDiaCerrado);

                    if (result.Success)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Index", "DiaCerrado");
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Ocurrio un error al eliminar la información, intente de nuevo.";
                        return View(IdDiaCerrado);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No fue posible cargar la información.";
                    return RedirectToAction("Index", "DiaCerrado");
                }
            }
            catch (Exception e)
            {

                TempData["ErrorMessage"] = "No fue posible cargar la información. " + e.Message;
                return RedirectToAction("Index", "DiaCerrado");
            }
        }
    }
}