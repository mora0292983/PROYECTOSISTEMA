using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGrease.Activities;
using System.Net.Mail;
using System.Net;

namespace proyectoC2
{
    public partial class PG_Login : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btncontinuar_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener los valores de los TextBox
                string usuario = txtdescripcion.Text.Trim();
                string contraseña = txtpassword.Text.Trim();

                // Verificar los datos
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
                {
                    // Mostrar un mensaje de error si alguno de los campos está vacío
                    lblMensaje.Text = "Por favor, complete todos los campos.";
                    lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
                }
                else if (usuario == "karina" && contraseña == "123")
                {
                    // Redirigir a otra página si los datos son correctos
                    Response.Redirect("PG_Inicio.aspx");
                }
                else if (usuario == "jazmin" && contraseña == "1234")
                {
                    // Redirigir a otra página si los datos son correctos
                    Response.Redirect("PG_InicioJe.aspx");
                }
                else
                {
                    // Mostrar un mensaje de error si los datos son incorrectos
                    lblMensaje.Text = "Usuario o contraseña incorrectos. Por favor, intenta de nuevo.";
                    lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                lblMensaje.Text = "Se ha producido un error: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
        protected void btnButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener el valor del TextBox para el código de verificación
                string codigoVerificacion = txtVerificationCode.Text.Trim();

                // Verificar el código
                if (codigoVerificacion == "658674")
                {
                    // Redirigir a la página "inicio.aspx" si el código es correcto
                    Response.Redirect("pg_panelEmpleado.aspx");
                }
                else
                {
                    // Redirigir a la página "contacto.aspx" si el código es incorrecto
                    Response.Redirect("pg_panelsupervisora.aspx");
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                lblMensaje.Text = "Se ha producido un error: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
    }
}