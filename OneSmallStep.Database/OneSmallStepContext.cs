using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OneSmallStep.Database.Models;

namespace OneSmallStep.Database
{
    public class OneSmallStepContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Step> Steps { get; set; }

        public OneSmallStepContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public OneSmallStepContext()
        {
            _dbPath = "sktl.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_dbPath}");
    }
}
