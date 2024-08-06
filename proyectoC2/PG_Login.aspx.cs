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
        protected void btnVerificar_Click(object sender, EventArgs e)
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
                    lblMensaje.CssClass = "mensaje-error";
                }
                else if (usuario == "karina" && contraseña == "123")
                {
                    Response.Redirect("pg_inicio.aspx");
                }
                else if (usuario == "jazmin" && contraseña == "1234")
                {
                    Response.Redirect("PG_InicioJe.aspx");
                }
                else
                {
                    lblMensaje.Text = "Usuario o contraseña incorrectos. Por favor, intenta de nuevo.";
                    lblMensaje.CssClass = "mensaje-error";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Se ha producido un error: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
    }
}