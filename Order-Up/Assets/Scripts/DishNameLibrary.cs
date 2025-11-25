using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DishEntry
{
    public int id;
    public string name;
}

[System.Serializable]
public class DishDataWrapper
{
    public List<DishEntry> Dishnames;
}


public static class DishNameLibrary
{
    private static Dictionary<int, string> _dishMap;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("dishNames");

        if (jsonFile == null)
        {
            Debug.LogError("DishNameLibrary: Could not find 'dishNames.json' in a Resources folder");
            return;
        }
        
        DishDataWrapper data = JsonUtility.FromJson<DishDataWrapper>(jsonFile.text);
        
        if (data != null && data.Dishnames != null)
        {
            _dishMap = data.Dishnames.ToDictionary(x => x.id, x => x.name);
            Debug.Log($"DishNameLibrary loaded {_dishMap.Count} dish names.");
        }
    }
    
    public static string GetName(int id)
    {
        if (_dishMap != null && _dishMap.TryGetValue(id, out string name))
        {
            return name;
        }
        return "Unknown Dish";
    }
    
    public static string GetName(string idString)
    {
        if (int.TryParse(idString, out int idInt))
        {
            return GetName(idInt);
        }
        
        Debug.LogWarning($"DishNameLibrary: Could not parse ID '{idString}' to an integer.");
        return "Invalid ID";
    }
}