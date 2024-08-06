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
        private static string verificationCode;
        private static string currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btncontinuar_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtdescripcion.Text.Trim();
                string contraseña = txtpassword.Text.Trim();

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
                {
                    lblMensaje.Text = "Por favor, complete todos los campos.";
                    lblMensaje.CssClass = "mensaje-error";
                }
                else if (usuario == "karina" && contraseña == "123")
                {
                    currentUser = usuario;
                    verificationCode = GenerateVerificationCode();
                    string userEmail = "morakarina708@gmail.com"; // Correo del usuario
                    SendVerificationCode(userEmail, verificationCode);

                    // Mostrar el modal de verificación
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowVerificationModal", "showVerificationModal();", true);
                }
                else if (usuario == "jazmin" && contraseña == "1234")
                {
                    Response.Redirect("PG_InicioJe.aspx");
                }
                else if (usuario == "pp" && contraseña == "12345")
                {
                    currentUser = usuario;
                    verificationCode = GenerateVerificationCode();
                    string userEmail = "morakarina708@gmail.com"; // Correo del usuario
                    SendVerificationCode(userEmail, verificationCode);

                    // Mostrar el modal de verificación
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowVerificationModal", "showVerificationModal();", true);
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

        protected void btnVerifyCode_Click(object sender, EventArgs e)
        {
            string enteredCode = txtVerificationCode.Text.Trim();

            if (enteredCode == verificationCode)
            {
                // Redirigir a la página correspondiente
                if (currentUser == "karina" || currentUser == "pp")
                {
                    Response.Redirect("PG_Inicio.aspx");
                }
                else
                {
                    lblVerificationMessage.Text = "Código verificado con éxito.";
                    lblVerificationMessage.CssClass = "mensaje-exito";
                }
            }
            else
            {
                lblVerificationMessage.Text = "Código incorrecto. Por favor, intenta de nuevo.";
                lblVerificationMessage.CssClass = "mensaje-error";
            }
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Genera un código de 6 dígitos
        }

        private void SendVerificationCode(string toEmail, string code)
        {
            string fromEmail = "morakarina708@gmail.com"; // Reemplaza con tu correo electrónico
            string fromPassword = "gjaabuczpwkibgng"; // Reemplaza con tu contraseña

            MailMessage mail = new MailMessage(fromEmail, toEmail);
            mail.Subject = "Código de Verificación";
            mail.Body = $"Tu código de verificación es: {code}";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Puerto para TLS
            smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                // Manejo de errores de envío de correo
                throw new Exception("Error al enviar el correo: " + ex.Message);
            }
        }
    }
}