<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="PG_Expediente.aspx.cs" Inherits="proyectoC2.PG_Expediente" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <style>
        .main-content3 {
            display: flex;
            justify-content: center;
            align-items: flex-start;
            height: auto;
            padding: 20px;
            gap: 20px;
        }

        .content-box-container {
            display: flex;
            flex-direction: row;
            align-items: flex-start;
            gap: 20px;
            width: 100%;
            max-width: 1200px;
        }

        .content-box {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            width: 60%;
            padding: 20px;
            box-sizing: border-box;
            position: relative;
        }

        .content-box h2 {
            color: #7154FC;
            font-weight: bold;
            margin-top: 0;
            text-align: center;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            font-weight: bold;
            margin-bottom: 5px;
        }

        .asp-control {
            width: 100%;
            padding: 10px;
            border-radius: 5px;
            border: 1px solid #ccc;
            box-sizing: border-box;
        }

        .link-button {
            display: inline-block;
            background-color: #7154FC;
            color: white;
            padding: 10px 15px;
            border-radius: 5px;
            text-align: center;
            text-decoration: none;
            font-size: 16px;
            cursor: pointer;
            margin-bottom: 10px;
            border: none;
            width: 100%;
            box-sizing: border-box;
        }

        .link-button-eliminar {
            display: inline-block;
            background-color: #FF4C4C;
            color: white;
            padding: 10px 15px;
            border-radius: 5px;
            text-align: center;
            text-decoration: none;
            font-size: 16px;
            cursor: pointer;
            margin-bottom: 10px;
            border: none;
            width: 100%;
            box-sizing: border-box;
        }

        .link-button-editar {
            display: inline-block;
            background-color: #4C9BFC;
            color: white;
            padding: 10px 15px;
            border-radius: 5px;
            text-align: center;
            text-decoration: none;
            font-size: 16px;
            cursor: pointer;
            margin-bottom: 10px;
            border: none;
            width: 100%;
            box-sizing: border-box;
        }

        .link-button:hover {
            background-color: #5a005a;
        }

        .link-button-eliminar:hover {
            background-color: #cc0000;
        }

        .link-button-editar:hover {
            background-color: #357ae8;
        }

        .link-buttons-container {
            display: flex;
            flex-direction: column;
            gap: 10px;
            position: absolute;
            top: 260px;
            right: 90px;
        }

        .image-box {
            width: 45%;
            max-width: 150px;
        }

        .image-box img {
            width: 100%;
            height: auto;
            border-radius: 8px;
            object-fit: cover;
        }

        .gridview-style {
            width: 100%;
            border-collapse: collapse;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            font-family: Arial, sans-serif;
        }

        .gridview-style th {
            background-color: #7154FC;
            color: white;
            padding: 10px;
            text-align: left;
            border-bottom: 2px solid #ddd;
        }

        .gridview-style td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }

        .gridview-style tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .gridview-style tr:hover {
            background-color: #f1f1f1;
        }

        .gridview-style th, .gridview-style td {
            border: 1px solid #ddd;
        }

        /* Estilos para los modales */
        .modal {
            display: none;
            position: fixed;
            z-index: 1;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.4);
        }

        .modal-content {
            background-color: #fefefe;
            margin: 15% auto;
            padding: 20px;
            border: 1px solid #888;
            width: 80%;
            max-width: 500px;
            border-radius: 8px;
        }

        .close {
            color: #aaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

        .close:hover,
        .close:focus {
            color: black;
            text-decoration: none;
            cursor: pointer;
        }
    </style>

        <div class="main-content3">
            <div class="image-box">
                <img src="img/imgM.png" alt="Imagen de Ejemplo">
            </div>
            <div class="content-box-container">
                <div class="content-box">
                    <h2>Registro de Días Festivos</h2>
                    <asp:GridView ID="gridDepartamentos" runat="server" AutoGenerateColumns="False" CssClass="gridview-style">
                        <Columns>
                            <asp:BoundField DataField="DepartamentoID" HeaderText="ID Departamento" ItemStyle-HorizontalAlign="Center" Visible="False" />
                            <asp:BoundField DataField="NombreDepartamento" HeaderText="Nombre Departamento" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                </div>
                <!-- Botones fuera del cuadro blanco -->
                <div class="link-buttons-container">
                    <a href="#modalNuevo" class="link-button" onclick="openModal('modalNuevo')">Nuevo</a>
                    <a href="#modalEliminar" class="link-button-eliminar" onclick="openModal('modalEliminar')">Eliminar</a>
                    <a href="#modalEditar" class="link-button-editar" onclick="openModal('modalEditar')">Editar</a>
                </div>
            </div>
        </div>
        
        <!-- Modal Nuevo -->
        <div id="modalNuevo" class="modal">
            <div class="modal-content">
                <span class="close" onclick="closeModal('modalNuevo')">&times;</span>
                <h2>Nuevo Departamento</h2>
                <!-- Contenido del formulario para nuevo departamento -->
                <div class="form-group">
                    <label for="nombreDepartamento">Nombre del Departamento:</label>
                    <input type="text" id="nombreDepartamento" class="asp-control" />
                </div>
                <div class="form-group">
                    <label for="descripcion">Descripción:</label>
                    <textarea id="descripcion" class="asp-control"></textarea>
                </div>
                <button class="btn" onclick="saveNewDepartment()">Guardar</button>
            </div>
        </div>

        <!-- Modal Eliminar -->
        <div id="modalEliminar" class="modal">
            <div class="modal-content">
                <span class="close" onclick="closeModal('modalEliminar')">&times;</span>
                <h2>Eliminar Departamento</h2>
                <!-- Contenido del formulario para eliminar departamento -->
                <div class="form-group">
                    <label for="nombreDepartamentoEliminar">Nombre del Departamento:</label>
                    <input type="text" id="nombreDepartamentoEliminar" class="asp-control" />
                </div>
                <button class="btn" onclick="deleteDepartment()">Eliminar</button>
            </div>
        </div>

        <!-- Modal Editar -->
        <div id="modalEditar" class="modal">
            <div class="modal-content">
                <span class="close" onclick="closeModal('modalEditar')">&times;</span>
                <h2>Editar Departamento</h2>
                <!-- Contenido del formulario para editar departamento -->
                <div class="form-group">
                    <label for="nombreDepartamentoActual">Nombre Actual del Departamento:</label>
                    <input type="text" id="nombreDepartamentoActual" class="asp-control" />
                </div>
                <div class="form-group">
                    <label for="nuevoNombreDepartamento">Nuevo Nombre del Departamento:</label>
                    <input type="text" id="nuevoNombreDepartamento" class="asp-control" />
                </div>
                <div class="form-group">
                    <label for="nuevaDescripcion">Nueva Descripción:</label>
                    <textarea id="nuevaDescripcion" class="asp-control"></textarea>
                </div>
                <button class="btn" onclick="editDepartment()">Guardar Cambios</button>
            </div>
        </div>

        <script>
            function openModal(modalId) {
                document.getElementById(modalId).style.display = "block";
            }

            function closeModal(modalId) {
                document.getElementById(modalId).style.display = "none";
            }

            window.onclick = function (event) {
                if (event.target.classList.contains('modal')) {
                    closeModal(event.target.id);
                }
            }

            function saveNewDepartment() {
                // Implementa la lógica para guardar el nuevo departamento
                closeModal('modalNuevo');
            }

            function deleteDepartment() {
                // Implementa la lógica para eliminar el departamento
                closeModal('modalEliminar');
            }

            function editDepartment() {
                // Implementa la lógica para editar el departamento
                closeModal('modalEditar');
            }
        </script>
    
</asp:Content>