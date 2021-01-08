using System;
using System.Collections.Generic;
using System.Text;

namespace OneSmallStep.Database.Models
{
    public class Step
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string NextButtonText { get; set; }
        public int? TimerSeconds { get; set; }
        public int Rank { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
