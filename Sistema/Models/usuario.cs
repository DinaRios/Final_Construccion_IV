using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class usuario
    {
        public int identificacion { get; set; }
        public string correo { get; set; }
        public string clave { get; set; }


        public string ConfirmarClave { get; set; }


    }
}