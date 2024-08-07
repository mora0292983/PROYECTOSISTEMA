using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace proyectoC2
{
    public partial class VerPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int inconsistenciaID;
                if (int.TryParse(Request.QueryString["ID"], out inconsistenciaID))
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                    string query = "SELECT DocumentoPDF FROM InconsistenciasFinal WHERE InconsistenciaID = @InconsistenciaID";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InconsistenciaID", inconsistenciaID);

                        connection.Open();
                        byte[] fileBytes = command.ExecuteScalar() as byte[];
                        connection.Close();

                        if (fileBytes != null)
                        {
                            string base64String = Convert.ToBase64String(fileBytes, 0, fileBytes.Length);
                            string imgSrc = string.Format("data:application/pdf;base64,{0}", base64String);
                            pdfViewer.Text = $"<embed src='{imgSrc}' type='application/pdf' width='100%' height='100%' />";
                        }
                        else
                        {
                            pdfViewer.Text = "Archivo no encontrado.";
                        }
                    }
                }
            }
        }
    }
}