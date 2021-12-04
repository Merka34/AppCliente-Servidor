using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaint4.Models
{
    public class Datos
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Circulo Trazo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Circulo> ListaTrazos { get; set; }
    }
}
