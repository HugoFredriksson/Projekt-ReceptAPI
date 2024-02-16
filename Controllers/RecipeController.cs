using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;


namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept; convert zero datetime=True");

        [HttpGet("ViewAllRecipes")]
        public ActionResult<List<Recipe>> ViewAllRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();

            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT t1.`Id`, t1.`UserId`, t1.`Username`, t1.`Title`, t1.`Description`, t1.`ImageUrl`, t1.`TimeStamp`, t1.`Content` FROM `recipe` t1 LEFT JOIN  `user` t2 ON t1.Id = t2.Id;;";
                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    Recipe recipe = new Recipe();
                    recipe.Id = data.GetInt32("Id");
                    recipe.UserId = data.GetInt32("UserId");
                    recipe.UserName = data.GetString("UserName");
                    recipe.Title = data.GetString("Title");
                    recipe.Description = data.GetString("Description");
                    recipe.ImageUrl = data.GetString("ImageUrl");
                    recipe.TimeStamp = data.GetString("TimeStamp");
                    recipe.Content = data.GetString("Content");

                    recipes.Add(recipe);

                }
            }
            catch (Exception ex)
            {
                Connection.Close();
                return StatusCode(500, ex.Message);
            }
            Connection.Close();
            return StatusCode(200, recipes);
        }

        [HttpPost("CreateRecipe")] // FUNKAR TYP
        public ActionResult CreateRecipe(Recipe recipe)
        {
            string authorization = Request.Headers["Authorization"];
            User user = (User)UserController.sessionId[authorization];

            const string DIRECTORY = "C:\\Users\\Elev\\Desktop\\bildertemporär\\";
            recipe.ImageUrl = recipe.ImageUrl.Split(',')[1];
            byte[] data = Convert.FromBase64String(recipe.ImageUrl);
            string randombase64 = generateRandomBase64String();
            string path = randombase64 + ".png";

            try
            {
                System.IO.File.WriteAllBytes(path, data);
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }


            try
            {
                Connection.Open();
                string userHeader = Request.Headers["Authorization"];
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();

                command.CommandText = "INSERT INTO `recipe` (`Id`, `UserId`, `UserName`, `Title`, `Description`, `ImageUrl`, `TimeStamp`, `Content`) VALUES (@Id, @UserId, @UserName, @Title, @Description, @ImageUrl, (SELECT CURRENT_TIMESTAMP), @Content)";
                command.Parameters.AddWithValue("@Id", recipe.Id);
                command.Parameters.AddWithValue("@UserId", recipe.UserId);
                command.Parameters.AddWithValue("@UserName", recipe.UserName);
                command.Parameters.AddWithValue("@Title", recipe.Title);
                command.Parameters.AddWithValue("@Description", recipe.Description);
                command.Parameters.AddWithValue("@imagePath", path);
                command.Parameters.AddWithValue("@Content", recipe.Content);

                int rows = command.ExecuteNonQuery();

                if (rows == 0)
                {
                    System.IO.File.Delete(path);
                    Connection.Close();
                    return StatusCode(500, "Image rows was zero");
                }

            }
            catch (Exception exception)
            {
                System.IO.File.Delete(path);
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(201, "lyckades skapa Blog");
        }
        static string generateRandomBase64String()
        {

            byte[] randomBytes = new byte[5];
            new Random().NextBytes(randomBytes);

            // Convert the random bytes to Base64
            string randomBase64 = Convert.ToBase64String(randomBytes);

            // Ensure the string has the desired length


            return randomBase64.Substring(0, 5);
        }
    }
}
