using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuizClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {/*
            HttpClient http = new HttpClient();
            List<Sombrero> lista = new List<Sombrero>();
            lista.Add(new Sombrero {Marca="Criko", Valor=90 });
            lista.Add(new Sombrero {Marca="NEmba", Valor=55 });
            lista.Add(new Sombrero {Marca="TAYF", Valor=78 });
            string json = JsonConvert.SerializeObject(lista);
            HttpRequestMessage requ = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:8050/")
            };
            HttpResponseMessage message = http.SendAsync(requ).Result;*/
            return View();
        }
    }

    public class Sombrero
    {
        public string Marca { get; set; }
        public int Valor { get; set; }
    }
}
