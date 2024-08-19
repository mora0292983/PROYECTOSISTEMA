using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace proyectoC2
{
    public partial class evidenciaHorasExtra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtener el HoraExtraID desde la URL
                int horaExtraID = 0;
                if (Request.QueryString["HoraExtraID"] != null)
                {
                    horaExtraID = Convert.ToInt32(Request.QueryString["HoraExtraID"]);
                }

                // Cargar los datos correspondientes si se ha proporcionado un HoraExtraID
                if (horaExtraID > 0)
                {
                    CargarDatosHoraExtra(horaExtraID);
                }
            }
        }

        private void CargarDatosHoraExtra(int horaExtraID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerHorasExtraPorID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HoraExtraID", horaExtraID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idSolicitud.Text = reader["HoraExtraID"].ToString();

                                // Formatear la fecha a "dd-mm-aaaa"
                                fecha.Text = Convert.ToDateTime(reader["Fecha"]).ToString("dd-MM-yyyy");

                                // Formatear HoraInicio y HoraFinal para mostrar solo horas y minutos
                                horaInicio.Text = TimeSpan.Parse(reader["HoraInicio"].ToString()).ToString(@"hh\:mm");
                                horaFin.Text = TimeSpan.Parse(reader["HoraFinal"].ToString()).ToString(@"hh\:mm");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    lblMensaje.Text = "Error al cargar los datos: " + ex.Message;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (pdfUpload.HasFile)
            {
                string extension = System.IO.Path.GetExtension(pdfUpload.FileName).ToLower();
                if (extension == ".pdf")
                {
                    try
                    {
                        byte[] pdfBytes = pdfUpload.FileBytes;
                        string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand("sp_ActualizarEvidenciaPDF", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@HoraExtraID", idSolicitud.Text);
                                cmd.Parameters.AddWithValue("@DocumentoPDF", pdfBytes);

                                SqlParameter returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                                returnParameter.Direction = ParameterDirection.ReturnValue;

                                cmd.ExecuteNonQuery();

                                int result = (int)returnParameter.Value;
                                if (result == 1)
                                {
                                    lblMensaje.CssClass = "mensaje-exito";
                                    lblMensaje.Text = "Evidencia enviada correctamente.";
                                }
                                else if (result == 0)
                                {
                                    lblMensaje.CssClass = "mensaje-error";
                                    lblMensaje.Text = "Ya existe una evidencia registrada para esta solicitud.";
                                }
                                else
                                {
                                    lblMensaje.CssClass = "mensaje-error";
                                    lblMensaje.Text = "Error al enviar la evidencia.";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMensaje.CssClass = "mensaje-error";
                        lblMensaje.Text = "Error al enviar la evidencia: " + ex.Message;
                    }
                }
                else
                {
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Text = "Por favor, seleccione un archivo en formato PDF.";
                }
            }
            else
            {
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Text = "Debe seleccionar un archivo para enviar.";
            }
        }
    }
}
