using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClientVuelos.Models;
using System.Net;
using ClientVuelos.Models.ViewModels;

namespace ClientVuelos.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index(IndexViewModel viewmodel)
        {
            IndexViewModel vm = new IndexViewModel();
            if (viewmodel == null)
            {
                vm.HttpRespuesta = "";
                vm.IdCode = 0;
            }

            try
            {
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new ArgumentException("Por favor verifique su conexión a Internet");
                }
                vm.ListaVuelos = Registros();
            }
            catch(ArgumentException ae)
            {
                vm.IdCode = 2;
                vm.HttpRespuesta = ae.Message;
            }
            catch (Exception ex)
            {
                vm.IdCode = 2;
                vm.HttpRespuesta = ex.Message;
            }
            return View(vm);
        }

        [Route("Agregar")]
        public IActionResult Agregar(string formJson)
        {
            FormularioViewModel vm;
            if (!string.IsNullOrWhiteSpace(formJson))
            {
                vm = JsonConvert.DeserializeObject<FormularioViewModel>(formJson);
            }
            else
            {
                vm = new FormularioViewModel();
            }
            if (vm.MyRegistro == null || string.IsNullOrWhiteSpace(vm.Mensaje))
            {
                vm.MyRegistro = new Registro { Destino = "", Estado = "", Vuelo = "", Hora = "" };
            }
            else if (vm.MyRegistro.Destino == null && vm.MyRegistro.Vuelo == null && vm.MyRegistro.Hora == null && vm.MyRegistro.Estado == null)
            {
                vm.MyRegistro = new Registro { Destino = "", Estado = "", Vuelo = "", Hora = "" };
            }
            return View(vm);
        }

        [Route("Editar/{vuelo}")]
        public IActionResult Editar(string vuelo, string formJson)
        {
            try
            {
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new ArgumentException("Por favor verifique su conexión a Internet");
                }
                if (string.IsNullOrWhiteSpace(formJson))
                {
                    IEnumerable<Registro> registros = Registros();
                    Registro reg = registros.FirstOrDefault(x => x.Vuelo == vuelo);
                    if (reg == null)
                    {
                        return RedirectToAction("Index");
                    }
                    FormularioViewModel vm = new FormularioViewModel();
                    vm.MyRegistro = reg;
                    return View(vm);
                }
                else
                {
                    FormularioViewModel vm = JsonConvert.DeserializeObject<FormularioViewModel>(formJson);
                    return View(vm);
                }
                
            }
            catch (ArgumentException ae)
            {
                IndexViewModel vm = new IndexViewModel();
                vm.IdCode = 2;
                vm.HttpRespuesta = ae.Message;
                return RedirectToAction("Index", new { vm });
            }
            catch (Exception ex)
            {
                IndexViewModel vm = new IndexViewModel();
                vm.IdCode = 2;
                vm.HttpRespuesta = ex.Message;
                return RedirectToAction("Index", new { vm });
            }
        }

        [Route("Eliminar/{vuelo}")]
        public IActionResult Eliminar(string vuelo)
        {
            try
            {
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new ArgumentException("Por favor verifique su conexión a Internet");
                }
                IEnumerable<Registro> registros = Registros();
                Registro reg = registros.FirstOrDefault(x => x.Vuelo == vuelo);
                if (reg == null)
                {
                    return RedirectToAction("Index");
                }
                return View(reg);
            }
            catch (ArgumentException ae)
            {
                IndexViewModel vm = new IndexViewModel();
                vm.IdCode = 2;
                vm.HttpRespuesta = ae.Message;
                return RedirectToAction("Index", new { vm });
            }
            catch (Exception ex)
            {
                IndexViewModel vm = new IndexViewModel();
                vm.ListaVuelos = Registros();
                vm.IdCode = 2;
                vm.HttpRespuesta = ex.Message;
                return RedirectToAction("Index", new { vm });
            }

        }

        [Route("")]
        [HttpPost]
        public IActionResult Index(string Hora, string Destino, string Vuelo, string Estado, string type)
        {
            HttpResponseMessage respuesta; //Campo en que se almacenara la respuesta del servidor
            IndexViewModel vm = new IndexViewModel(); //Nueva instancia de ViewModel para la vista Index
            HttpClient client = new HttpClient(); // Instancia para comunicacion al servidor via HTTP
            try
            {
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new ArgumentException("Por favor verifique su conexión a Internet");
                }
                Registro reg = new Registro { Hora = Hora, Destino = Destino, Vuelo = Vuelo, Estado = Estado };
                //Si alguno de los datos es nulo
                if (string.IsNullOrWhiteSpace(Hora) || string.IsNullOrWhiteSpace(Vuelo) || string.IsNullOrWhiteSpace(Destino) || string.IsNullOrWhiteSpace(Estado))
                {
                    FormularioViewModel fvm = new FormularioViewModel();
                    fvm.Mensaje = "No puede dejar campos vacios";
                    fvm.MyRegistro = reg;
                    string formJson = JsonConvert.SerializeObject(fvm);

                    if (type=="POST") // Si es en la vista Agregar
                    {
                        return RedirectToAction("Agregar", new { formJson });
                    }
                    else if(type=="PUT") // Si es en la vista Editar
                    {
                        return RedirectToAction("Editar", new { Vuelo, formJson });
                    }
                    else // El usuario cambio el valor
                    {
                        // 0 - Ningun mensaje
                        // 1 - Buen Mensaje
                        // 2 - Mal Mensaje
                        vm.IdCode = 2;
                        vm.HttpRespuesta = "La entrada de datos del formulario ocasiono un conflicto";
                        vm.ListaVuelos = Registros();
                        return View(vm);
                    }
                }
                //Si ninguno es nulo | Todo esta correcto
                string json = JsonConvert.SerializeObject(reg);
                switch (type)
                {
                    case "POST":
                        HttpRequestMessage requPOST = new HttpRequestMessage
                        {
                            Content = new StringContent(json, Encoding.UTF8, "application/json"),
                            Method = HttpMethod.Post,
                            RequestUri = new Uri("http://vuelos.itesrc.net/Tablero")
                        };

                        respuesta = client.SendAsync(requPOST).Result;
                        if (respuesta.IsSuccessStatusCode)
                        {
                            vm.IdCode = 1;
                            vm.HttpRespuesta = $"El vuelo {Vuelo} se ha agregado correctamente";
                        }
                        else
                        {
                            vm.IdCode = 2;
                        }
                        vm.ListaVuelos = Registros();
                        return View(vm);

                    case "DELETE":
                        HttpRequestMessage requDELETE = new HttpRequestMessage
                        {
                            Content = new StringContent(json, Encoding.Default, "application/json"),
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri("http://vuelos.itesrc.net/Tablero")
                        };
                        respuesta = client.SendAsync(requDELETE).Result;
                        if (respuesta.IsSuccessStatusCode)
                        {
                            vm.IdCode = 1;
                            vm.HttpRespuesta = $"El vuelo {Vuelo} se ha eliminado correctamente";
                        }
                        else
                        {
                            vm.IdCode = 2;
                        }
                        vm.ListaVuelos = Registros();
                        return View(vm);

                    case "PUT":
                        vm.ListaVuelos = Registros();
                        Registro regC = vm.ListaVuelos.FirstOrDefault(x => x.Vuelo == Vuelo);
                        if (regC==null)
                        {
                            vm.IdCode = 2;
                            vm.HttpRespuesta = $"El vuelo {Vuelo} no existe";
                            return View(vm);
                        }
                        HttpRequestMessage requPUT = new HttpRequestMessage
                        {
                            Content = new StringContent(json, Encoding.Default, "application/json"),
                            Method = HttpMethod.Put,
                            RequestUri = new Uri("http://vuelos.itesrc.net/Tablero")
                        };
                        respuesta = client.SendAsync(requPUT).Result;
                        if (respuesta.IsSuccessStatusCode)
                        {
                            vm.IdCode = 1;
                            vm.HttpRespuesta = $"El vuelo {Vuelo} se ha actualizado correctamente";
                        }
                        else
                        {
                            vm.IdCode = 2;
                        }
                        vm.ListaVuelos = Registros();
                        return View(vm);
                    default:
                        break;
                }
            }
            catch (ArgumentException ae)
            {
                vm.IdCode = 2;
                vm.HttpRespuesta = ae.Message;
                return RedirectToAction("Index", new { vm });
            }
            catch (Exception)
            {
                vm.IdCode = 2;
                vm.HttpRespuesta = "No se logro conectarse al servidor, revise su conexión a Internet";
                return RedirectToAction("Index", new { vm });
            }
            return View();
        }

        public IEnumerable<Registro> Registros()
        {
            WebClient web = new WebClient();
            string f = web.DownloadString("http://vuelos.itesrc.net/Tablero");
            var datos = JsonConvert.DeserializeObject<IEnumerable<Registro>>(f);
            return datos;
        }
    }
}
