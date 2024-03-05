﻿using Projekt_Recept.Controllers;

namespace Projekt_Recept
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string TimeStamp { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        
    }
}
