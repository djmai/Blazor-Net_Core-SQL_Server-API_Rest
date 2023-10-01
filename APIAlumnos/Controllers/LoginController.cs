using APIAlumnos.Datos;
using APIAlumnos.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using ModeloClasesAlumnos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIAlumnos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CursosController> _log;
        private readonly IRepositorioUsuarios _usuariosRepositorio;

		public LoginController(IConfiguration configuration, ILogger<CursosController> logger, IRepositorioUsuarios usuariosRepositorio)
        {
            _configuration = configuration;
            _log = logger;
            _usuariosRepositorio = usuariosRepositorio;
		}

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioAPI>> Login(Login usuarioLogin)
        {
            UsuarioAPI? infoUsuario = null;

            try
            {
                infoUsuario = await AutenticarUsuarioAsync(usuarioLogin.Usuario, usuarioLogin.Password);
                if (infoUsuario == null)
                    throw new Exception("Credenciales no validas");
                else
                    infoUsuario = GenerarTokenJWT(infoUsuario);
                
            }catch (Exception ex)
            {
                infoUsuario = new UsuarioAPI();
                _log.LogError("Se produjo un error al autenticarse" + ex.ToString());

                infoUsuario.error = new Error();
                infoUsuario.error.mensaje = ex.ToString();
                infoUsuario.error.mostrarUsuario = false;

            }
            return infoUsuario;
        }

        private async Task<UsuarioAPI> AutenticarUsuarioAsync(string usuario, string password)
        {
            if (_configuration["UsuarioAPI"] == usuario && _configuration["PassAPI"] == password)
            {
                return new UsuarioAPI()
                {
                    Nombre = _configuration["NombreUsuario"],
                    Apellidos = _configuration["ApellidosUsuario"],
                    Email = _configuration["EmailUsuario"],
                };
            }

            return null;
        }

        private UsuarioAPI GenerarTokenJWT(UsuarioAPI usuarioInfo)
        {
            // Cabecera
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:ClaveSecreta"]!)
                );
            var _signingCredentiasl = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _header = new JwtHeader(_signingCredentiasl);

            // Claims
            var _claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", usuarioInfo.Nombre),
                new Claim("apellidos", usuarioInfo.Apellidos),
                new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email),
            };

            // Payload
            var _payload = new JwtPayload(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: _claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(24)
                );

            // Token

            var _token = new JwtSecurityToken(
                    _header,
                    _payload
                );

            usuarioInfo.Token = new JwtSecurityTokenHandler().WriteToken( _token );

            return usuarioInfo;
        }

        [HttpPost]
        [Route("CrearUsuario")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<UsuarioLogin>> CrearUsuario(UsuarioLogin usuario)
        {
            UsuarioLogin resultado = new UsuarioLogin();
            try
            {
                if (usuario == null)
                    return BadRequest();

                resultado = await _usuariosRepositorio.AltaUsuario(usuario);
            }
            catch(SqlException ex)
            {
                resultado.error = new Error();
                _log.LogError("Se produjo un error en el controlador de login en el método CrearUsuario:" + ex.ToString());
                resultado.error.mensaje = "Error dando de alta nuevo usuario" + ex.Message;
                resultado.error.mostrarUsuario = true;
            }
            catch (Exception ex)
            {
				resultado.error = new Error();
				_log.LogError("Se produjo un error en el controlador de login en el método CrearUsuario:" + ex.ToString());
				resultado.error.mensaje = ex.ToString();
				resultado.error.mostrarUsuario = false;
			}
            return resultado;
        }

        [HttpPost]
        [Route("ValidarUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioLogin>> ValidarUsuario(UsuarioLogin usuario)
        {
            UsuarioLogin resultado = new UsuarioLogin();
            try
            {
                if (usuario == null)
                    return BadRequest();

                resultado = await _usuariosRepositorio.DameUsuario(usuario.EmailLogin);

                if (usuario.EmailLogin != resultado.EmailLogin || resultado.Password != usuario.Password)
                    throw new Exception("Credenciales no validas");
            }
            catch (SqlException ex)
            {
                resultado.error = new Error();
                _log.LogError("Se produjo un error en el controlador de login en el método ValidarUsuario:" + ex.ToString());
                resultado.error.mensaje = "Error dando de alta nuevo usuario" + ex.Message;
                resultado.error.mostrarUsuario = true;
            }
            catch (Exception ex)
            {
                resultado.error = new Error();
                _log.LogError("Se produjo un error en el controlador de login en el método ValidarUsuario:" + ex.ToString());
                resultado.error.mensaje = ex.ToString();
                resultado.error.mostrarUsuario = false;
            }
            return resultado;
        }
    }
}
