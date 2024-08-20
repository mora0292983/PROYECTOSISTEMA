using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace proyectoC2
{
    public partial class VerPDF3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int horaExtraID;
                if (int.TryParse(Request.QueryString["HoraExtraID"], out horaExtraID))
                {
                    MostrarPDF(horaExtraID);
                }
            }
        }

        private void MostrarPDF(int horaExtraID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT DocumentoPDF FROM HorasExtraFinal WHERE HoraExtraID = @HoraExtraID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@HoraExtraID", horaExtraID);
                connection.Open();

                byte[] fileData = (byte[])command.ExecuteScalar();

                if (fileData != null && fileData.Length > 0)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", fileData.Length.ToString());
                    Response.AddHeader("Content-Disposition", "inline; filename=solicitud.pdf");
                    Response.BinaryWrite(fileData);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    Response.Write("No se encontró el archivo PDF.");
                }
            }
        }
    }
}