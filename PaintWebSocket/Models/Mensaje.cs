using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaint4.Models
{
    public struct Mensaje
    {
        private string _error;

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }

    }
}
