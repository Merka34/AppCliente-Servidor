using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientVuelos.Models;

namespace ClientVuelos.Models.ViewModels
{
    public class IndexViewModel
    {
        public int IdCode { get; set; }
        public string HttpRespuesta { get; set; }
        public IEnumerable<Registro> ListaVuelos { get; set; }
    }
}
