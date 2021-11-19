using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using QuizClient.Models;

namespace QuizClient.Controllers
{
    public class HomeController : Controller
    {
        public string UrlServer { get; set; } = "http://localhost:8050/";
        public IActionResult Index(string mensaje, int b)
        {
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                ModelState.AddModelError("", mensaje);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                string mensaje = "Por favor ingrese un nombre";
                return RedirectToAction("Index", new { mensaje});
            }
            else
            {
                HttpClient http = new HttpClient();
                HttpRequestMessage requ = new HttpRequestMessage
                {
                    Content = new StringContent(username, Encoding.UTF8, "text/plain"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(UrlServer + "Usuario")
                };
                HttpResponseMessage httpResponse = http.SendAsync(requ).Result;
                byte[] buffer = httpResponse.Content.ReadAsByteArrayAsync().Result;
                string mensaje = Encoding.UTF8.GetString(buffer);

                if (string.IsNullOrWhiteSpace(mensaje))
                {
                    return RedirectToAction("Encuesta", new { username });
                }
                else
                {
                    return RedirectToAction("Index", new { mensaje });
                }
            }
        }

        public IActionResult Encuesta(string username)
        {
            return View(new Registro { Username = username });
        }

        [HttpPost]
        public IActionResult Encuesta(Registro registro)
        {
            if (string.IsNullOrWhiteSpace(registro.Username))
            {
                return RedirectToAction("Index");
            }
            HttpClient http = new HttpClient();
            string json = JsonConvert.SerializeObject(registro);
            HttpRequestMessage requ = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri(UrlServer + "Respuesta")
            };
            HttpResponseMessage httpResponse = http.SendAsync(requ).Result;
            Registro reg = new Registro { Username = registro.Username };
            double f;
            if (double.TryParse(reg.Tiempo, out f))
            {
                byte[] buffer = httpResponse.Content.ReadAsByteArrayAsync().Result;
                string message = Encoding.UTF8.GetString(buffer);
                string s = reg.Tiempo;
                string u = reg.Username;
                string i = message;
                return RedirectToAction("Respuesta", new { s, u, i });
            }
            else
            {
                byte[] buffer = httpResponse.Content.ReadAsByteArrayAsync().Result;
                string segundos = Encoding.UTF8.GetString(buffer);
                reg = new Registro { Inciso = registro.Inciso, Username = registro.Username, Tiempo = segundos };
                string s = reg.Tiempo;
                string u = reg.Username;
                string i = reg.Inciso;
                return RedirectToAction("Respuesta", new { s, u, i });
            }
        }

        public IActionResult Respuesta(string s, string u, string i)
        {
            return View(new Registro { Tiempo=s, Username=u, Inciso=i});
        }

        [HttpPost]
        public IActionResult Respuesta(string username)
        {
            return RedirectToAction("Encuesta", new { username });
        }
    }
}
