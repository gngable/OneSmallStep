insert into IngredientCategories (Name, Rank)
values ('Tools', 1), ('Ingredients', 0)

insert into Recipes (Title, Description)
values ('Boil Water', 'How to boil water')

insert into Ingredients (RecipeId, IngredientCategoryId, Name, Quantity, Unit)
values (1, 1, 'Pot', 1, 'Quart')
	, (1, 1, 'Measuring Cup', 1, 'Quart')
	, (1, 2, 'Water', 2, 'Cups')
	
insert into Steps (RecipeId, Rank, Text)
values (1, 100, 'Use the measuring cup to measure 2 cups of water from the sink')
	, (1, 90, 'Pour the water into the pot')
	, (1, 80, 'Put the pot on the stove')
	, (1, 70, 'Turn the burner under the pot on as high as it will go')
	, (1, 60, 'Wait for the water to boil')
	, (1, 50, 'Turn off the burner under the pot')
	, (1, 40, 'All done! Celebrate!')