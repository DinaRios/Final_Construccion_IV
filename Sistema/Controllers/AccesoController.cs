using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sistema.Models;
using System.Data.SqlClient;
using System.Data;

namespace Sistema.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = "data source = LAPTOP-R62HLFIJ; catalog = entrar;integrated security = true";
        // GET: Acceso
        public ActionResult login()
        {
            return View();
        }

        public ActionResult registrar()
        {
            return View();

        }

        [HttpPost]
        public ActionResult registrar(usuario oUsuario)
        {
            bool registrado;
            string mensaje;
            if (oUsuario.clave == oUsuario.ConfirmarClave)
            {
                oUsuario.clave = ConvertirSha256(oUsuario.clave);
            } else
            {
                ViewData["mensaje"] = "las contraseñas no son iguales... por favor verificar";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("registrar", cn);
                cmd.Parameters.AddWithValue("correo", oUsuario.correo);
                cmd.Parameters.AddWithValue("clave", oUsuario.clave);
                cmd.Parameters.Add("registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("mensaje", SqlDbType.VarChar).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["registrado"].Value);
                mensaje = cmd.Parameters["mensaje"].Value.ToString();

            }


            ViewData["mensaje"] = mensaje;
            if (registrado)
            {
                return RedirectToAction("login","acceso");  
               
            }else
            {
                return View();
            }
            
        }

       [HttpPost]
       public ActionResult login (usuario oUsuario)
        {
            oUsuario.clave = ConvertirSha256(oUsuario.clave);

            using(SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("validar", cn);
                cmd.Parameters.AddWithValue("correo", oUsuario.correo);
                cmd.Parameters.AddWithValue("clave", oUsuario.clave);
                cmd.CommandType =CommandType.StoredProcedure;

                cn.Open();

                oUsuario.identificacion = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if(oUsuario.identificacion != 0)
            {
                Session["usuario"] = oUsuario;
                return RedirectToAction("index", "home");
            }
            else
            {
                ViewData["mensaje"] = "usuario no encontrado ";
            }

        }


       public static string ConvertirSha256(string texto)
        {
            //esto hace referencia al encriptamiento de claves de usuario
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash .ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));        
            }
        }

    }

}