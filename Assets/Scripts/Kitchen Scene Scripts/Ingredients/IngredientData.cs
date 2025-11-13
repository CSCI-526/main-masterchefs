using UnityEngine;


public enum IngredientState {Raw, Cooked, Overcooked}

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/Ingredient")]

public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;
    public IngredientData cookedResult; // What this ingredient becomes when cooked
    public IngredientData overcookedResult;//What this becomes when overcooked

}
