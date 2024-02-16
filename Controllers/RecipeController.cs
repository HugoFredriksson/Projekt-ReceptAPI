using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
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

        [HttpPost("CreateRecipe")]
        public ActionResult CreateRecipe(Recipe recipe)
        {
            string authorization = Request.Headers["Authorization"];
            User user = (User)UserController.sessionId[authorization];

            const string DIRECTORY = "C:\\Users\\Elev\\source\\repos\\MissensZooAPI\\MissensZooOchBlogg\\images\\";
            blog.image = blog.image.Split(',')[1];
            byte[] data = Convert.FromBase64String(blog.image);
            string randombase64 = generateRandomBase54String();
            string path = randombase64 + ".png";

            //Attempts to create image file
            try
            {
                System.IO.File.WriteAllBytes(path, data);
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }

            //Attempts to insert blog values into database
            try
            {
                Connection.Open();
                string userHeader = Request.Headers["Authorization"];
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();

                command.CommandText = "INSERT INTO `blogpost` (`title`, `userId`, `content`, `imagePath`, `timestamp`, `category`) VALUES (@title, @userId, @blogContent, @imagePath, (SELECT CURRENT_TIMESTAMP), @category)";
                command.Parameters.AddWithValue("@title", blog.title);
                command.Parameters.AddWithValue("@userId", blog.userId);
                command.Parameters.AddWithValue("@imagePath", path);
                command.Parameters.AddWithValue("@blogContent", blog.content);
                command.Parameters.AddWithValue("@category", blog.category);
                int rows = command.ExecuteNonQuery();

                //If rows of values are 0 deletes image and returns error message
                if (rows == 0)
                {
                    System.IO.File.Delete(path);
                    Connection.Close();
                    return StatusCode(500, "Image rows was zero");
                }

            }
            catch (Exception exception)
            {
                //If insertion into database fails deletes image and returns 500 Internal Server Error
                System.IO.File.Delete(path);
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(201, "lyckades skapa Blog");
        }
    }
}
