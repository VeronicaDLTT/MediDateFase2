//using ceTe.DynamicPDF;
//using ceTe.DynamicPDF.PageElements;
using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Build.Framework;
using System.Diagnostics;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Previewer;
using QuestPDF.Helpers;
using QuestPDF.Companion;
using System.Data.Common;

namespace MediDate.Controllers
{
    public class RecetaController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<RecetaController> _logger;
        public Receta recetaAux = new Receta();
        public byte[] archivoBytesAux;
        public string fileName;
        public RecetaController(ILogger<RecetaController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Create(int IdCita)
        {
            recetaAux = _database.Recetas.GetByIdCita(IdCita);

            ViewBag.IdCita = recetaAux.IdCita;
            ViewBag.NombreConsultorio = recetaAux.NombreConsultorio;
            ViewBag.DireccionConsultorio = recetaAux.DireccionConsultorio;
            ViewBag.NombreMedico = recetaAux.NombreMedico;
            ViewBag.Especialidad = recetaAux.Especialidad;
            ViewBag.NumCedula = recetaAux.NumCedula;
            ViewBag.NombrePaciente = recetaAux.NombrePaciente;
            ViewBag.NombrePaciente = recetaAux.NombrePaciente;
            ViewBag.Edad = recetaAux.Edad;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Receta receta)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            recetaAux = _database.Recetas.GetByIdCita(receta.IdCita);
            fileName = $"receta_{receta.IdCita}_{recetaAux.NombrePaciente}.pdf";
            var memoryStream = new MemoryStream();

            //Hora actual
            var horaActualTS = DateTime.Now.TimeOfDay;
            var horaActual = DateTime.Now.ToString("hh:mm tt");
            string fechaFormateada = receta.Fecha.HasValue ? receta.Fecha.Value.ToString("dd/MM/yyyy") : "Fecha no disponible";

            receta.Hora = horaActualTS;

            //Generamos el archivo PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Helvetica"));


                    page.Header()
                        .AlignCenter()
                        .Text("Receta Médica\n" + "Dr. " + recetaAux.NombreMedico)
                        .SemiBold().FontSize(18).FontColor(Colors.Black);

                    page.Content()
                        .Column(x =>
                        {
                            x.Spacing(8);
                            x.Item().Text(recetaAux.Especialidad).FontSize(10).AlignCenter();
                            x.Item().Text("Cédula Profesional: " + recetaAux.NumCedula).FontSize(10).AlignCenter();
                            x.Item().Text(recetaAux.NombreConsultorio + " - " + recetaAux.DireccionConsultorio).FontSize(10).SemiBold().AlignCenter();

                            x.Item().Row(row =>
                            {
                                // Datos Paciente
                                row.RelativeItem().Column(col =>
                                {
                                    col.Spacing(8);
                                    col.Item().Text(text =>
                                    {
                                        text.Span("\nPaciente: ").Bold();
                                        text.Span(recetaAux.NombrePaciente);
                                        text.Span(" Edad: ").Bold();
                                        text.Span(recetaAux.Edad);
                                    });

                                });

                                // Fecha y Hora
                                row.ConstantItem(200).Column(col =>
                                {
                                    col.Spacing(8);
                                    col.Item().Text(text =>
                                    {
                                        text.Span("\nFecha: ").Bold();
                                        text.Span(fechaFormateada);
                                        text.Span(" Hora: ").Bold();
                                        text.Span(horaActual);
                                    });
                                });

                            });

                            x.Item().LineHorizontal(1);

                            x.Item().Row(row =>
                            {
                                // Datos recetas izquierda
                                row.ConstantItem(120).PaddingRight(1).Column(col =>
                                {
                                    col.Spacing(8);
                                    col.Item().Text(text =>
                                    {
                                        text.Span("\nPeso: ").Bold();
                                        text.Span(receta.Peso);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("Talla: ").Bold();
                                        text.Span(receta.Talla);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("Temperatura: ").Bold();
                                        text.Span(receta.Temperatura);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("TA: ").Bold();
                                        text.Span(receta.TA);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("FC: ").Bold();
                                        text.Span(receta.FC);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("FR: ").Bold();
                                        text.Span(receta.FR);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("Sat: ").Bold();
                                        text.Span(receta.Saturacion);
                                    });

                                    col.Item().Text(text =>
                                    {
                                        text.Span("Alergias: \n").Bold();
                                        text.Span(receta.Alergias);
                                    });
                                });

                                // Datos receta derecha
                                row.RelativeItem().Border(0.01f).PaddingLeft(10).Column(col =>
                                {
                                    col.Spacing(8);
                                    col.Item().Text("\nDiagnóstico:").Bold();
                                    col.Item().Text(receta.Diagnostico);
                                    col.Item().Text("\nTratamiento:").Bold();
                                    col.Item().Text(receta.Tratamiento);
                                    col.Item().Text("\nObservaciones:").Bold();
                                    col.Item().Text(receta.Observaciones);
                                });

                            });

                        });


                    page.Footer()

                        .Row(row =>
                        {
                            // Columna para el texto "Firma:"
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Firma:").Bold();
                                col.Item().PaddingLeft(40).LineHorizontal(1); // Línea horizontal justo debajo del texto
                            });

                            // Columna para el número de página
                            row.RelativeItem().Text(x =>
                            {
                                x.Span("\n\nPágina ");
                                x.CurrentPageNumber();
                            });
                        });
                });
            }).GeneratePdf(memoryStream);

            // Convertir MemoryStream a byte[]
            archivoBytesAux = memoryStream.ToArray();

            // No olvides cerrar el MemoryStream cuando ya no lo necesites
            memoryStream.Close();

            //Generamos objeto Archivo
            Archivo archivo = new Archivo();
            archivo.NombreArchivo = fileName;
            archivo.Extension = ".pdf";
            archivo.ArchivoByte = archivoBytesAux;

            //Guardamos datos de la Receta en la BD
            var result = _database.Recetas.Create(receta, archivo.NombreArchivo, archivo.ArchivoByte, archivo.Extension);

            if (result.Success)
            {
                TempData["InfoMessage"] = result.Message;
                
                // Retornamos el archivo como una descarga
                return File(archivoBytesAux, "application/pdf", fileName);

            }
            else
            {
                TempData["InfoMessage"] = "Ocurrio un error al generar la Receta Médica, intente de nuevo.";
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