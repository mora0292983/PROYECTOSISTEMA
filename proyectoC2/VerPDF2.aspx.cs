using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace proyectoC2
{
    public partial class VerPDF2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int actividadID;
                if (int.TryParse(Request.QueryString["ID"], out actividadID))
                {
                    MostrarPDF(actividadID);
                }
            }
        }

        private void MostrarPDF(int actividadID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT DocumentoPDF FROM ActividadesFinal WHERE ActividadID = @ActividadID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ActividadID", actividadID);
                connection.Open();

                byte[] fileData = (byte[])command.ExecuteScalar();

                if (fileData != null && fileData.Length > 0)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", fileData.Length.ToString());
                    Response.AddHeader("Content-Disposition", "inline; filename=actividad.pdf");
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
