using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class RegistroActividadRealizada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Cargar los tipos de actividad desde la base de datos
                //CargarTiposDeActividad();

                // Cargar las horas en los DropDownLists de horaInicio y horaFin
                CargarHoras(ddlHoraInicio);
                CargarHoras(ddlHoraFin);
            }

        }

        private void CargarHoras(DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute <= 30; minute += 30)
                {
                    string time = $"{hour:00}:{minute:00}";
                    ddl.Items.Add(new ListItem(time, time));
                }
            }
            ddl.Items.Insert(0, new ListItem("-- Selecciona una Hora --", "0"));
        }
    }
}