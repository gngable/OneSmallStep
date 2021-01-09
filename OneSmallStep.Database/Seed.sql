insert into IngredientCategories (Name, Rank)
values ('Tools', 0), ('Ingredients', 1);

------------------------------------------------------

insert into Recipes (Title, Description)
values ('Boil Water', 'How to boil water');

insert into Ingredients (RecipeId, IngredientCategoryId, Name, Quantity, Unit)
values (1, 1, 'Pot', 1, 'Quart')
	, (1, 1, 'Measuring Cup', 0, null)
	, (1, 2, 'Water from sink', 0, null);
	
insert into Steps (RecipeId, Rank, Text, NextButtonText)
values (1, 0, 'Use the measuring cup to measure 2 cups of water from the sink', null)
	, (1, 10, 'Pour the water into the pot', null)
	, (1, 20, 'Put the pot on the stove', null)
	, (1, 30, 'Turn the burner under the pot on as high as it will go', 'Stove is on')
	, (1, 40, 'Wait for the water to boil', 'Water is boiling')
	, (1, 50, 'Turn off the burner under the pot', 'The stove is off')
	, (1, 60, 'All done! Celebrate!', null);

------------------------------------------------------

insert into Recipes (Title, Description)
values ('Ramen Noodles', 'How to cook a package of ramen noodle soup');

insert into Ingredients (RecipeId, IngredientCategoryId, Name, Quantity, Unit)
values (2, 1, 'Pot', 1, 'Quart')
	, (2, 1, 'Measuring Cup', 0, null)
	, (2, 2, 'Ramen Noodle', 1, 'Package');
	
insert into Steps (RecipeId, Rank, Text, NextButtonText, TimerSeconds)
values (2, 0, 'Use the measuring cup to measure 2 cups of water from the sink', null, null)
	, (2, 10, 'Pour the water into the pot', null, null)
	, (2, 20, 'Put the pot on the stove', null, null)
	, (2, 30, 'Turn the burner under the pot on as high as it will go', 'Stove is on', null)
	, (2, 40, 'Open the package of Ramen and take out the seasoning packet', null, null)
	, (2, 60, 'Wait for the water to boil', 'Water is boiling', null)
	, (2, 70, 'Carefully put the noodles into the water', null, null)
	, (2, 80, 'Turn the burner down so that the water is simmering', null, null)
	, (2, 90, 'Let the noodles cook for 3 minutes', null, 180)
	, (2, 100, 'Turn off the burner under the pot', 'The stove is off', null)
	, (2, 110, 'Open the seasoning packet and dump it in the pot', null, null)
	, (2, 120, 'Stir the soup', null, null)
	, (2, 130, 'All done! Bon appétit!', null, null);