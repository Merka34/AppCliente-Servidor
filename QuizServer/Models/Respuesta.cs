using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizServer.Models
{
    public class Respuesta
    {
        public string Username { get; set; }
        public string IdRespuesta { get; set; }
        public double Tiempo { get; set; }
        public bool Correcto { get; set; }
    }
}
