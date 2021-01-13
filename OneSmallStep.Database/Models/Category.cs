using System;
using System.Collections.Generic;
using System.Text;

namespace OneSmallStep.Database.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
    }
}
