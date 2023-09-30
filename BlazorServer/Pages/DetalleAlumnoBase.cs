using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using ModeloClasesAlumnos;

namespace BlazorServer.Pages
{
    public class DetalleAlumnoBase : ComponentBase
	{
        [Inject]
        public NavigationManager navigationManager { get; set; }
        
		[Inject]
		public IServicioAlumnos? ServicioAlumnos { get; set; }

		[Parameter]
		public string? Id { get; set; }

        public bool MostrarPopUP = false;

        public Alumno? alumno { get; set; } = new Alumno();

		protected override async Task OnInitializedAsync()
		{
			alumno = await ServicioAlumnos!.DameAlumno(Convert.ToInt32(Id));
		}

        protected async void DarDeBaja(int id)
        {
            await ServicioAlumnos.EliminarAlumno(id);
            navigationManager.NavigateTo($"alumno/{id}", true);
        }

        protected async void Activar()
        {
            alumno.FechaBaja = null;
            await ServicioAlumnos.ModificarAlumno(alumno.Id, alumno);
            navigationManager.NavigateTo($"alumno/{alumno.Id}", true);
        }
    }
}
