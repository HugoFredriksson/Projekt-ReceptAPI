using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using Projekt_Recept;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;


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
                command.CommandText = "SELECT t1.`Id`, t1.`UserId`, t1.`UserName`, `Title`, `Description`,`Ingredients`, `ImageUrl`, `TimeStamp`, `Content` FROM `recipe` t1 LEFT JOIN `user` t2 ON t1.UserId = t2.id";
                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    Recipe recipe = new Recipe();
                    recipe.Id = data.GetInt32("Id");
                    recipe.UserId = data.GetInt32("UserId");
                    recipe.UserName = data.GetString("Username");
                    recipe.Title = data.GetString("Title");
                    recipe.Description = data.GetString("Description");
                    recipe.Ingredients = data.GetString("Ingredients");
                    recipe.ImageUrl = data.GetString("ImageUrl");
                    recipe.TimeStamp = data.GetString("TimeStamp");
                    recipe.Content = data.GetString("Content");
                    recipes.Add(recipe);
                }
                data.Close();

                foreach (Recipe recipe in recipes)
                {
                    recipe.comments = GetComments(recipe.Id);
                    recipe.categories = GetCategories(recipe.Id);
                    //recipe.LikeCount = GetLikes(recipe);
                }

            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(200, recipes);
        }

        private string UserName(int id)
        {
            User user = new User();
            try
            {

                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT UserName FROM user WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = command.ExecuteReader();

                reader.Read();

                user.UserName = reader.GetString("UserName");
                reader.Close();

                return user.UserName;

            }
            catch (Exception exception)
            {
                Console.WriteLine("Fel inträffade, kunde inte hämta användarnamn" + exception.Message);
                return user.UserName;
            }
        }

        private List<Comment> GetComments(int RecipeId)
        {
            List<Comment> comments = new List<Comment>();
           
            try
            {
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT * FROM comments WHERE RecipeId = @RecipeId";
                command.Parameters.AddWithValue("@RecipeId", RecipeId);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Comment comment = new Comment();
                    comment.CommentId = reader.GetUInt16("CommentId");
                    comment.UserId = reader.GetUInt16("UserId");
                    comment.RecipeId = reader.GetUInt16("RecipeId");
                    comment.TimeStamp = reader.GetString("TimeStamp");
                    comment.Content = reader.GetString("Content");
                    comments.Add(comment);
                }
                reader.Close();

                foreach(Comment comment in comments)
                {
                    comment.UserName = UserName(comment.UserId);
                }

                return comments;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"Gick inte att hämta kommentar! fel:{exception.Message}");
                return comments;
            }
        }

        

        private List<Category> GetCategories(int RecipeId)
        {
            List<Category> categories = new List<Category>(); 
            try
            {
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT * FROM category WHERE RecipeId = @RecipeId";
                command.Parameters.AddWithValue("@RecipeId", RecipeId);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Category category = new Category();
                    category.Id = reader.GetInt32("Id");
                    category.RecipeId = reader.GetInt16("RecipeId");
                    category.Categories = reader.GetString("Category");

                    categories.Add(category);
                }
                reader.Close();

                
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Gick inte att hämta kategorier! fel{exception.Message}");
            } return categories;
        }

        [HttpGet("{Id}")]
        public ActionResult<Recipe> GetRecipeFromId(int Id)
        {
            List<Recipe> recipe = new List<Recipe>();
            try
            {
                Connection.Open();
                MySqlCommand query = Connection.CreateCommand();
                query.Prepare();
                query.CommandText = "SELECT t1.`Id` AS RecipeId, t1.`UserId`, t1.`UserName`, t1.`Title`, GROUP_CONCAT(DISTINCT t3.`category` SEPARATOR ', ') AS Categories, t1.`Ingredients`, t1.`Description`, t1.`ImageUrl`, t1.`TimeStamp`, t1.`Content`, COALESCE(t4.LikeCount, 0) AS LikeCount FROM `recipe` t1 LEFT JOIN `category` t3 ON t1.`Id` = t3.`RecipeId` LEFT JOIN (SELECT `RecipeId`, COUNT(`RecipeId`) AS LikeCount FROM `Likes` GROUP BY  `RecipeId`) t4 ON t1.`Id` = t4.`RecipeId` WHERE t1.`Id` = @Id GROUP BY t1.`Id`, t1.`UserId`, t1.`Username`, t1.`Title`, t1.`Ingredients`, t1.`Description`, t1.`ImageUrl`, t1.`TimeStamp`, t1.`Content` ORDER BY t1.`TimeStamp` DESC;";
                query.Parameters.AddWithValue("@Id", Id);
                MySqlDataReader data = query.ExecuteReader();

                while (data.Read())
                {
                    Recipe recipes = new Recipe
                    {
                        Id = data.GetInt32("UserId"),
                        UserName = data.GetString("UserName"),
                        Title = data.GetString("Title"),
                        Description = data.GetString("Description"),
                        ImageUrl = data.GetString("ImageUrl"),
                        TimeStamp = data.GetString("TimeStamp"),
                        Content = data.GetString("Content")
                        
                    };
                    recipe.Add(recipes);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok(recipe);
        }

        [HttpDelete("DeleteRecipe")]
        public ActionResult DeleteRecipe(Recipe recipe)
        {
            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "DELETE FROM recipe WHERE Id = @Id ";
                command.Parameters.AddWithValue("Id", recipe.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(200, $"Lyckades ta bort Recept, ReceptId = {recipe.Id} ");
        }

        [HttpPut("UpdateRecipe")] 
        public ActionResult UpdateRecipe(Recipe recipe)
        {
            try
            {
                Connection.Open();

                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "UPDATE `recipe` SET `UserId`= @UserId,`UserName`= @UserName,`Title`= @Title,`Description`= @Description,`ImageUrl`= @ImageUrl, `TimeStamp`=(SELECT CURRENT_TIMESTAMP),`Content`= @Content WHERE Id = @Id";

                command.Parameters.AddWithValue("UserId", recipe.UserId);
                command.Parameters.AddWithValue("UserName", recipe.UserName);
                command.Parameters.AddWithValue("Title", recipe.Title);
                command.Parameters.AddWithValue("Description", recipe.Description);
                command.Parameters.AddWithValue("ImageUrl", recipe.ImageUrl);
                command.Parameters.AddWithValue("Content", recipe.Content);
                command.Parameters.AddWithValue("Id", recipe.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            return StatusCode(200, recipe);
        }
        [HttpPost("CreateRecipe")] // FUNKAR TYP
        public ActionResult CreateRecipe(PostRecipe recipe)
        {
            //string authorization = Request.Headers["Authorization"];
            //User user = (User)UserController.sessionId[authorization];

            const string DIRECTORY = "C:\\Users\\Elev\\source\\repos\\Recept-Projekt\\src\\recipeImage\\";
            recipe.imageUrl = recipe.imageUrl.Split(',')[1];
            byte[] data = Convert.FromBase64String(recipe.imageUrl);
            string randombase64 = generateRandomBase64String();
            string path = DIRECTORY + randombase64 + ".png";

            try
            {
                System.IO.File.WriteAllBytes(path, data);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            try
            {
                Connection.Open();
                string userHeader = Request.Headers["Authorization"];
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();

                command.CommandText = "INSERT INTO `recipe` (`Title`, `Description`, `Ingredients`, `imageUrl`, `TimeStamp`, `Content`) VALUES (@Title, @Description, @Ingredients, @imageUrl, (SELECT CURRENT_TIMESTAMP), @Content)";
                command.Parameters.AddWithValue("@Title", recipe.title);
                command.Parameters.AddWithValue("@Description", recipe.description);
                command.Parameters.AddWithValue("@Ingredients", recipe.ingredients);
                command.Parameters.AddWithValue("@imageUrl", recipe.imageUrl);
                command.Parameters.AddWithValue("@Content", recipe.content);

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
            return StatusCode(201, "Lyckades Skapa Recept");
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

//SELECT t1.`Id` AS RecipeId, t1.`UserId`,t1.`Username`,t1.`Title`,.`Description`,t1.`ImageUrl`,.`TimeStamp`,t1.`Content`,t3.`Category` FROM `recipe` t1 LEFT JOIN `user` t2 ON t1.`UserId` = t2.`Id` LEFT JOIN `category` t3 ON t1.`Id` = t3.`RecipeId` ORDER BY t1.`TimeStamp` DESC;

// SELECT t1.`Id` AS RecipeId, t1.`UserId`,t1.`Username`,t1.`Title`,t1.`Description`,t1.`ImageUrl`,t1.`TimeStamp`,t1.`Content`,t3.`CommentId`, t3.`Content` FROM `recipe` t1 LEFT JOIN `user` t2 ON t1.`UserId` = t2.`Id` LEFT JOIN `comment` t3 ON t1.`Id` = t3.`RecipeId` ORDER BY t1.`TimeStamp` DESC;

// HÄMTAR RECEPT BEROENDE PÅ ID ;-3 SELECT * FROM recipe t1 LEFT JOIN comment t2 ON t1.Id = t2.RecipeId WHERE t1.Id = @Id