using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using ModeloClasesAlumnos;

namespace BlazorServer.Pages
{
    public class AlumnosBase : ComponentBase
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }

        [Inject]
        public IServicioAlumnos? ServicioAlumnos { get; set; }

        [Inject]
        public IServicioLogin? ServicioLogin { get; set; }

        public IEnumerable<Alumno>? Alumnos { get; set; }
        public bool MostrarPopUP = false;
        public Alumno? alumno { get; set; }

        public Boolean mostrarError = false;
        public String txtError = String.Empty;

        Login l = new Login();
        UsuarioAPI u = new UsuarioAPI();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                l.Usuario = Environment.GetEnvironmentVariable("UsuarioAPI")!;
                l.Password = Environment.GetEnvironmentVariable("PassAPI")!;
                u = await ServicioLogin.SolicitudLogin(l);
                Environment.SetEnvironmentVariable("Token", u.Token);

                Alumnos = (await ServicioAlumnos.DameAlumnos()).ToList();

            }catch (Exception ex)
            {
                txtError = ex.Message;
                MostrarError();
                StateHasChanged();
            }
        }

        protected void CerrarError()
        {
            mostrarError = false;
            CerrarPop();
        }

        protected void MostrarError()
        {
            CerrarPop();
            mostrarError = true;
        }

        protected void Eliminar(Alumno alu)
        {
            alumno = alu;
            MostrarPopUP = true;
        }
        protected void CerrarPop()
        {
            MostrarPopUP = false;
            alumno = null;
        }

        protected async void DarDeBaja(int id)
        {
            await ServicioAlumnos.EliminarAlumno(id);
            CerrarPop();
            navigationManager.NavigateTo("alumnos", true);
        }
        protected async void Activar(Alumno alu)
        {
            alu.FechaBaja = null;
            await ServicioAlumnos.ModificarAlumno(alu.Id, alu);
            navigationManager.NavigateTo("alumnos", true);
        }

        /*protected override Task OnInitializedAsync()
        {
            CargarAlumnos();
            return base.OnInitializedAsync();
        }

        private void CargarAlumnos()
        {


            Precio precioBlazor = new Precio();
            precioBlazor.Id = 1;
            precioBlazor.Coste = 19.99;
            precioBlazor.FechaInicio = DateTime.Now;
            precioBlazor.FechaTermino = DateTime.Now.AddDays(3);

            Curso cursoBlazor = new Curso();
            cursoBlazor.Id = 1;
            cursoBlazor.NombreCurso = "Curso Blazor";
            cursoBlazor.ListaPrecios = new List<Precio>();
            cursoBlazor.ListaPrecios.Add(precioBlazor);

            Alumno alumno1 = new Alumno
            {
                Id = 1,
                Nombre = "Miguel Martínez",
                Email = "mail@pruebas.com",
                Foto = "https://picsum.photos/1080.webp",
                Listacursos = new List<Curso>(),
                FechaAlta = DateTime.Now,
                FechaBaja = null,
            };

            Alumno alumno2 = new Alumno
            {
                Id = 1,
                Nombre = "Manuel Martínez",
                Email = "mail2@pruebas.com",
                Foto = "https://picsum.photos/1080.webp",
                Listacursos = new List<Curso>(),
                FechaAlta = DateTime.Now,
                FechaBaja = null,
            };

            Alumno alumno3 = new Alumno
            {
                Id = 1,
                Nombre = "Juan Perez",
                Email = "mail4@pruebas.com",
                Foto = "https://picsum.photos/1080.webp",
                Listacursos = new List<Curso>(),
                FechaAlta = DateTime.Now,
                FechaBaja = null,
            };

            alumno1.Listacursos.Add(cursoBlazor);
            alumno2.Listacursos.Add(cursoBlazor);
            alumno3.Listacursos.Add(cursoBlazor);

            Alumnos = new List<Alumno> { alumno1, alumno2, alumno3 };
        }
        */
    }
}