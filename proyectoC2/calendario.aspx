<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="calendario.aspx.cs" Inherits="proyectoC2.WebForm1666" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Calendario</title>
    <!-- FullCalendar CSS -->
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.7/main.min.css" rel="stylesheet" />
    <!-- Estilos personalizados -->
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            background-color: #f4f4f4;
        }

        #calendar {
            max-width: 900px;
            margin: 0 auto;
        }

        .fc-daygrid-day.fc-day-festivo {
            background-color: red !important;
            color: white !important;
        }
    </style>
</head>
<body>
    <div id="calendar"></div>

    <!-- FullCalendar JS -->
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.7/main.min.js"></script>
    <!-- Script para el calendario -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var calendarEl = document.getElementById('calendar');
            var calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                events: [
                    // Ejemplo de días festivos
                    { title: 'Día Festivo', date: '2024-08-20', classNames: ['fc-day-festivo'] },
                    { title: 'Día Festivo', date: '2024-08-25', classNames: ['fc-day-festivo'] }
                ]
            });
            calendar.render();
        });
    </script>
</body>
</html>