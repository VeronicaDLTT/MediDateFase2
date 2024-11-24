using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Crypto;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediDate.Controllers
{
    public class FAQController : Controller
    {

        private readonly Database _database;
        private readonly ILogger<DetServicioController> _logger;

        public FAQController(ILogger<DetServicioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            //ViewBag.Categorias = _database.Categorias.GetAll();
            //ViewBag.IntIdCategoria = 0;
            //ViewBag.TxtBuscar = "";
            //return View(_database.FAQs.GetAll());

            ViewBag.Categorias = _database.Categorias.GetAll();
            ViewBag.IntIdCategoria = 0;
            ViewBag.TxtBuscar = "";

            var totalPreguntas = _database.FAQs.GetAll().Count();
            var totalPages = (int)Math.Ceiling((double)totalPreguntas / pageSize);
            var preguntas = _database.FAQs.GetAll()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
            .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(preguntas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index( int intIdCategoria, string txtBuscar, int page = 1)
        {
            ViewBag.Categorias = _database.Categorias.GetAll();
            ViewBag.IntIdCategoria = intIdCategoria;
            ViewBag.TxtBuscar = txtBuscar;

            var totalPreguntas = _database.FAQs.GetSearch(intIdCategoria, txtBuscar).Count();
            var totalPages = (int)Math.Ceiling((double)totalPreguntas / 10);
            var preguntas = _database.FAQs.GetSearch(intIdCategoria, txtBuscar)
                .Skip((page - 1) * 10)
                .Take(10)
            .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(preguntas);
        }

        public IActionResult IndexAdministrador()
        {
            ViewBag.Categorias = _database.Categorias.GetAll();
            return View(_database.FAQs.GetAll());
        }

        public IActionResult Create()
        {
            ViewBag.IdCategoria = new SelectList(_database.Categorias.GetAll(), "IdCategoria", "DescripcionCategoria");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FAQ faq)
        {
            
            //Agregamos la Pregunta Frecuente
            var result = _database.FAQs.Create(faq);

            if (!result.Success)
            {

                TempData["AlertMessage"] = "Error al registrar la Pregunta Frecuente. Intente de nuevo. ";
                ViewBag.IdCategoria = new SelectList(_database.Categorias.GetAll(), "IdCategoria", "DescripcionCategoria");

                return View(faq);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("IndexAdministrador", "FAQ");
            
        }

        public IActionResult Edit(int IdFAQ)
        {
            var faq = _database.FAQs.GetById(IdFAQ);

            if (faq == null)
            {
                return NotFound();
            }
            
            return View(faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FAQ faq)
        {

            //Editamos la Pregunta Frecuente
            var result = _database.FAQs.Edit(faq);

            if (!result.Success)
            {

                TempData["AlertMessage"] = "Error al editar la Pregunta Frecuente. Intente de nuevo. ";
                return View(faq);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("IndexAdministrador", "FAQ");

        }

        public IActionResult CargarVistaParcial(string accion, int idfaq)
        {
            Console.WriteLine($"Accion: {accion}, IdFAQ: {idfaq}"); // Agrega esto para depurar
            // Valida 'accion' y determina cuál vista cargar
            if (string.IsNullOrEmpty(accion))
            {
                return NotFound();
            }

            if (accion.Equals("FAQBase"))
            {
                var faq = _database.FAQs.GetById(idfaq);
                if (faq == null)
                {
                    return NotFound();
                }
                ViewBag.FAQ = faq;
                
            }
            
            // Carga la vista parcial correspondiente
            return PartialView($"~/Views/Shared/FAQsPV/{accion}.cshtml");
            
        }
    }
}
