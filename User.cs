using Projekt_Recept.Controllers;

namespace Projekt_Recept
{
    public class User
    {
        public int id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public int Role { get; set; }

    }
}
