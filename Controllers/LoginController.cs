using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace SistemadeLogin.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                return Json(new {Msg = "Usuario ja logado." }); 
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logar(string username, string senha) 
        {
            MySqlConnection mySqlConnection = new MySqlConnection("Server=localhost;Database=usuariosdb;Uid=root;Password=dyggolekao123");
            await mySqlConnection.OpenAsync();
            
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            mySqlCommand.CommandText = $"SELECT * FROM usuarios WHERE username = '{username}' AND senha = '{senha}'";
           
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
           
            if (await reader.ReadAsync()) 
            {
                int usuarioId = reader.GetInt32(0);
                string nome = reader.GetString(1);

                List<Claim> direitoAcesso = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                    new Claim(ClaimTypes.Name, nome)
                };

                var identity = new ClaimsIdentity(direitoAcesso, "Identity.Login");
                var  userPrincipal = new ClaimsPrincipal(new[] {identity});

                await HttpContext.SignInAsync(userPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.Now.AddHours(1)
                    });

                return Json(new { Msg = "Usuario logado com sucesso." });
            }
            return Json(new { Msg = "Usuario nao encontrado, verifique suas credenciais." });
        }
        public async Task<IActionResult> Logout() 
        {
            if (User.Identity.IsAuthenticated) {
                await HttpContext.SignOutAsync();
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
