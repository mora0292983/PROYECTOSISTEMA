﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

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
                    MostrarPDF(inconsistenciaID);
                }
            }
        }

        private void MostrarPDF(int inconsistenciaID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT DocumentoPDF FROM InconsistenciasFinal WHERE InconsistenciaID = @InconsistenciaID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@InconsistenciaID", inconsistenciaID);
                connection.Open();

                byte[] fileData = (byte[])command.ExecuteScalar();

                if (fileData != null && fileData.Length > 0)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", fileData.Length.ToString());
                    Response.AddHeader("Content-Disposition", "inline; filename=justificacion.pdf");
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

