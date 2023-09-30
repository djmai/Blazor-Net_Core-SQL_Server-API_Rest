using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;


namespace BlazorServer.Servicios
{
    public class MiServicioAuthenticationStateProvider: AuthenticationStateProvider
    {
        private ISessionStorageService almacenarSesionServicio;

        public MiServicioAuthenticationStateProvider(ISessionStorageService SessionStorageService)
        {
            almacenarSesionServicio = SessionStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var email = await almacenarSesionServicio.GetItemAsync<string>("email");
            ClaimsIdentity identity;
            if(email != null)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email)
                }, "apiauth_type");
            }
            else
            {
                identity = new ClaimsIdentity();
            }
            var usuario = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(usuario));
        }

        public void UsuarioAutenticado(string email)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email)
            }, "apiauth_type");

            var usuario = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(usuario)));
        }
             
    }
}
