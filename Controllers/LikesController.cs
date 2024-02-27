using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Encoders;
using Projekt_Recept;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LikesController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

        [HttpPost("LikeRecipe")]
        public ActionResult LikeRecipe(Likes likes)
        {
            string authorization = Request.Headers["Authorization"];
            User user = (User)UserController.sessionId[authorization];

            try
            {
                Connection.Open();
                string userHeader = Request.Headers["Authorization"];
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "INSERT INTO `likes` (`RecipeId`, `UserId`) VALUES (@RecipeId, @UserId);";
                command.Parameters.AddWithValue("RecipeId", likes.RecipeId);
                command.Parameters.AddWithValue("UserId", likes.UserId);
                int rows = command.ExecuteNonQuery();

                if (rows == 0)
                {
                    Connection.Close();
                    return StatusCode(500, "Image rows was zero");
                }

            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(200, "Gillade Recept");
        }

        [HttpGet("GetLikesFromRecipe/{RecipeId}")]
        public ActionResult<List<Likes>> GetLikesFromRecipe(int RecipeId)
        {
            List<Likes> likes = new List<Likes>();

            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT * FROM `likes` WHERE RecipeId = @RecipeId;";
                command.Parameters.AddWithValue("@RecipeId", RecipeId);
                MySqlDataReader data = command.ExecuteReader();
                while (data.Read())
                {
                    Likes like = new Likes();
                    
                    like.UserId = data.GetInt32("UserId");
                    like.RecipeId = data.GetInt32("RecipeId");
                    
                    likes.Add(like);
                }
                return StatusCode(200, likes);
            }
            catch (Exception ex)
            {
                Connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("RemoveLike")]
        public ActionResult RemoveLike(Likes likes)
        {
            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "DELETE FROM likes WHERE `likes`.`RecipeId` = @RecipeId AND `likes`.`UserId` = @UserId";
                command.Parameters.AddWithValue("RecipeId", likes.RecipeId);
                command.Parameters.AddWithValue("UserId", likes.UserId);
                command.ExecuteNonQuery();

                Connection.Close();
                return StatusCode(200, $"Lyckades ta bort en gillamarkering, ReceptId = {likes.RecipeId} UserId = {likes.UserId}");
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
        }
    }
}
