using System;
using Unity.VisualScripting.Antlr3.Runtime;

[Serializable]
public class RecipeHint
{
    public int id;
    public string currentRecipe;
    public string[] hints;
}

[Serializable]
public class HintDatabase
{
    public RecipeHint[] recipes;
}