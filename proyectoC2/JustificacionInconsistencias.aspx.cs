using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace proyectoC2
{
    public partial class JustificacionInconsistencias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Recuperar valores de la URL
                string idInconsistenciaValor = Request.QueryString["IDInconsistencia"];
                string empleadoIDValor = Request.QueryString["EmpleadoID"];
                string fechaValor = Request.QueryString["Fecha"];
                string tipoInconsistenciaValor = Request.QueryString["TipoInconsistencia"];

                // Asignar valores a los TextBox
                if (!string.IsNullOrEmpty(idInconsistenciaValor))
                {
                    idInconsistencia.Text = idInconsistenciaValor;
                }
                if (!string.IsNullOrEmpty(empleadoIDValor))
                {
                    Empleado.Text = empleadoIDValor;
                }
                if (!string.IsNullOrEmpty(fechaValor))
                {
                    fecha.Text = fechaValor;
                }
                if (!string.IsNullOrEmpty(tipoInconsistenciaValor))
                {
                    tipoInconsistencia.Text = tipoInconsistenciaValor;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Verificar si se ha seleccionado un archivo
            if (pdfUpload.HasFile && pdfUpload.PostedFile.ContentLength > 0)
            {
                try
                {
                    // Leer el archivo seleccionado en un array de bytes
                    byte[] fileBytes;
                    using (Stream fileStream = pdfUpload.PostedFile.InputStream)
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        fileBytes = binaryReader.ReadBytes((int)fileStream.Length);
                    }

                    // Verificar que el ID de inconsistencia es un valor válido
                    int inconsistenciaID;
                    if (int.TryParse(idInconsistencia.Text, out inconsistenciaID))
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                        string query = "UPDATE InconsistenciasFinal SET DocumentoPDF = @DocumentoPDF WHERE InconsistenciaID = @InconsistenciaID";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@DocumentoPDF", fileBytes);
                            command.Parameters.AddWithValue("@InconsistenciaID", inconsistenciaID);

                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();

                            // Mostrar un mensaje de éxito si se actualizó al menos una fila
                            if (rowsAffected > 0)
                            {
                                lblMensaje.Text = "Justificación enviada con éxito";
                                lblMensaje.CssClass = "mensaje-exito";
                            }
                            else
                            {
                                lblMensaje.Text = "Error al enviar la justificación";
                                lblMensaje.CssClass = "mensaje-error";
                            }
                        }
                    }
                    else
                    {
                        // Mensaje de error si el ID de inconsistencia no es válido
                        lblMensaje.Text = "ID de inconsistencia no válido";
                        lblMensaje.CssClass = "mensaje-error";
                    }
                }
                catch (Exception ex)
                {
                    // Mensaje de error en caso de excepción
                    lblMensaje.Text = "Error al enviar la justificación: " + ex.Message;
                    lblMensaje.CssClass = "mensaje-error";
                }
            }
            else
            {
                // Mensaje de error si no se seleccionó un archivo
                lblMensaje.Text = "Debe seleccionar un archivo para enviar la justificación";
                lblMensaje.CssClass = "mensaje-error";
            }
        }

    }
}