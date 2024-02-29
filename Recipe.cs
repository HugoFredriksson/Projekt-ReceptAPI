using Projekt_Recept.Controllers;

namespace Projekt_Recept
    {
        public class Recipe
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Categories { get; set; } = string.Empty;
            public string Ingredients { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty; 
            public string TimeStamp { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public int LikeCount { get; set; }
        }
    }
