using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;


namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

        public static Hashtable sessionId = new Hashtable();

        [HttpPost("CreateUser")]
        public ActionResult createAccount(User user)
        {
            string checkUniqueUser = CheckIfUniqueUserDataExists(user);
            if (checkUniqueUser != String.Empty)
            {
                return BadRequest(checkUniqueUser);
            }

            try
            {
                Connection.Open();
                string authorization = Request.Headers["Authorization"];

                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                command.CommandText = "INSERT INTO `user` (`UserName`, `Email`, `Password`, `Role`) VALUES (@UserName, @Email, @PassWord, 2)";
                command.Parameters.AddWithValue("@Username", user.UserName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Guid guid = Guid.NewGuid();
                    string key = guid.ToString();
                    user.id = (int)command.LastInsertedId;
                    sessionId.Add(key, user);
                    Connection.Close();
                    return StatusCode(201, key);

                }
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            return StatusCode(400);
        }

        [HttpGet("LogIn")]
        public ActionResult LogIn() // FUNKAR TYP :-)
        {
            string auth = this.HttpContext.Request.Headers["Authorization"];
            User user = DecodeUser(new User(), auth);
            Connection.Open();
            MySqlCommand command = Connection.CreateCommand();
            command.Prepare();
            command.CommandText = "SELECT * FROM user WHERE Email = @Email";
            command.Parameters.AddWithValue("@Email", user.Email);
            MySqlDataReader data = command.ExecuteReader();
            try
            {

                string passwordHash = String.Empty;

                while (data.Read())
                {
                    passwordHash = data.GetString("Password");
                    user.id = data.GetInt32("Id");
                    user.Email = data.GetString("Email");
                    user.Role = data.GetInt32("Role");
                }

                if (passwordHash != string.Empty && BCrypt.Net.BCrypt.Verify(user.Password, passwordHash))
                {
                    Guid guid = Guid.NewGuid();
                    string key = guid.ToString();
                    Console.WriteLine(key);
                    sessionId.Add(key, user);
                    Connection.Close();
                    return Ok(key);
                }

                Connection.Close();
                return StatusCode(400);
            }
            catch (Exception exception)
            {
                Connection.Close();
                Console.WriteLine($"Login failed: {exception.Message}");
                return StatusCode(500);
            }
        }

        private string CheckIfUniqueUserDataExists(User user)
        {
            string checkUniqueUser = String.Empty;
            try
            {
                MySqlCommand query = Connection.CreateCommand();
                query.Prepare();
                query.CommandText = "SELECT * FROM user WHERE Email = @userEmail OR Username = @UserName";
                query.Parameters.AddWithValue("@Username", user.UserName);
                query.Parameters.AddWithValue("@userEmail", user.Email);
                MySqlDataReader data = query.ExecuteReader();

                if (data.Read())
                {
                    if (data.GetString("Email") == user.Email)
                    {
                        checkUniqueUser = "Email används redan.";
                    }
                    if (data.GetString("userName") == user.UserName)
                    {
                        checkUniqueUser = "Användarnamn används redan.";
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"UserController.CheckIfUniqueUserDataExists: {exception.Message}");
                Connection.Close();
            }

            return checkUniqueUser;
        }
        private User DecodeUser(User user, string auth)
        {
            if (auth != null && auth.StartsWith("Basic"))
            {
                string encodedUsernamePassword = auth.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                int seperatorIndex = usernamePassword.IndexOf(':');
                user.Email = usernamePassword.Substring(0, seperatorIndex);
                user.Password = usernamePassword.Substring(seperatorIndex + 1);
            }
            else
            {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            return user;
        }
    }
}

