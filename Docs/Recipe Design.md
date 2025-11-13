# Alpha

Related scripts:  
CombinationSystem.cs, Recipe.cs, GameData.cs, Ingredient.cs, IngredientData.cs 

## Ingredients

3 states: raw, cooked, overcooked

| Num | Ingredients | Related Recipes |
| :---- | :---- | :---- |
| 1 | Potato | French Fries, Cheesy Fries |
| 2 | Cheese | Cheesy Fries, Cheese Pizza, Mac & Cheese, Spaghetti with Tomato Sauce  |
| 3 | Dough | Cheese Pizza |
| 4 | Tomato | Cheese Pizza, Chicken Sandwich, Spaghetti with Tomato Sauce, Chicken Salad without cheese |
| 5 | Bread | Chicken Sandwich, Omelette Sandwich |
| 6 | lettuce | Chicken Sandwich, Chicken Salad without cheese |
| 7 | Pasta | Mac & Cheese, Spaghetti with Tomato Sauce, Chicken Noodle Soup |
| 8 | Cooked Egg | Chicken Salad without cheese, Omelette Sandwich |
| 9 | Chicken | Chicken Salad without cheese,  Chicken Noodle Soup, Chicken Sandwich |
|  |  |  |

## Cookwares

| Cookwares | Ingredients | Time(cooked, overcooked) |
| :---- | :---- | :---- |
| Fryer | Chicken, Potato | 3s,6s |
| Pan | Bread, Cheese | 4s,8s |
| Pot | Noodles, Rice | 5s,10s |
| Oven | Dough | 5s,10s |
| Rice Cooker | Rice | 5s |
| Cutter/Board | Potato, Lettuce, Tomato | 3s |

## Recipes

| Recipes | Ingredients | Ingredient state | Cookwares | Instructions |
| :---- | :---- | :---- | :---- | :---- |
| **French Fries** | Potato  | Potato (cooked) | Fryer | Potato\>fryer |
| **Cheesy Fries** | Potato \+ Cheese | Potato(cooked) \+ Cheese(raw) | Fryer | Potato\>fryer \+Cheese |
| **Cheese Pizza** | Dough \+ Cheese \+ Tomato | Dough(cooked) \+ Cheese(cooked) \+ Tomato(cooked) | Oven | Dough \+Cheese \+Tomato \>Oven |
| **Mac & Cheese** | Pasta \+ Cheese  | Pasta(cooked) \+ Cheese(cooked)  | Pan | Pasta \+Cheese \>Pan  |
| **Chicken Sandwich** | Bread \+ Chicken \+ Lettuce \+ Tomato | Bread(raw) \+ Chicken(cooked) \+ Lettuce(raw) \+Tomato(raw) | Fryer | Chicken\>Fryer \+Bread \+Lettuce \+Tomato |
| **Omelette Sandwich** | Bread \+ Egg | Bread(cooked) \+ Egg(cooked) | / | Bread  \+Egg |
| **Spaghetti with Tomato Sauce** | Pasta \+ Tomato \+ Cheese | Pasta(cooked) \+ Tomato(cooked) \+ Cheese(cooked) | Pot, pan | Noodles\>Pot Cheese \+Tomato\>Pan |
| **Chicken Salad without cheese** | Lettuce \+ Cooked Egg \+ Tomato \+ Chicken  | Lettuce(raw) \+ Cooked Egg(cooked) \+ Tomato(raw) \+ Chicken(cooked) | Pot | Chicken\>Pot \+Tomato \+Cooked Egg \+Lettuce |
| **Chicken Noodle Soup** | Pasta \+ Chicken | Pasta(cooked) \+ Chicken(cooked) | Pot | Chicken Pasta \>Pot |
|  |  |  |  |  |

