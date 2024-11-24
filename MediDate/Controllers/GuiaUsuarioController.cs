using MediDate.Models;
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
    public class GuiaUsuarioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<GuiaUsuarioController> _logger;

        public GuiaUsuarioController(ILogger<GuiaUsuarioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut","Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }
                
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }
            
        }

        public IActionResult AgregarHorarioAtencion()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult AgregarPrimerServicio()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult ValidacionCuenta()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult AgendarCita()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult ReagendarCita()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult CancelarCita()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult GenerarReceta()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EstadosCita()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult ListaHistorial()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditarHorarioAtencion()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult AgregarDiaCerrado()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditarServicio()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EliminarServicio()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditarDiaCerrado()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EliminarDiaCerrado()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditarConsultorio()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditarCuenta()
        {
            try
            {
                //Verificamos si hay un Usuario que Inició Sesión
                if (Request.Cookies.TryGetValue("TipoUsuario", out string strTipoUsuario))
                {
                    string TipoUsuario = strTipoUsuario;

                    //Verificamos si el Usuario es de tipo Medico o Administrador
                    if ((TipoUsuario.Equals("M")) || (TipoUsuario.Equals("A")))
                    {
                        return View();
                    }
                    else
                    {
                        //No tiene permiso para ingresar a la página
                        return RedirectToAction("LogOut", "Usuario");
                    }
                }
                else
                {
                    //No existe un Usuario que Inició Sesión
                    return RedirectToAction("LogOut", "Usuario");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible cargar la página. \n" + e.Message;
                return RedirectToAction("Index", "Medico");
            }

        }

    }
}