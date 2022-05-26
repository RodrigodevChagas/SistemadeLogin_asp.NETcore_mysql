 using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace SistemadeLogin.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
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
                
                return Json(new { Msg = "Usuario logado com sucesso." });
            }
            return Json(new { Msg = "Usuario nao encontrado, verifique suas credenciais." });
        }
    }
}
