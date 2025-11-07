//using UnityEngine;
//using System;
//using System.Collections.Generic;

//[Serializable]
//public class DialogueData
//{
//    public int id;
//    public int dishId;
//    public string dishName;
//    public string sentenceTemplate;
//    public string color;
//    public string image;
//}

//[Serializable]
//public class DialogueList
//{
//    public DialogueData[] dialogues;
//}

//public class DialogueManager : MonoBehaviour
//{
//    private DialogueList dialogueList;
//    private static HashSet<int> usedIds = new HashSet<int>();

//    //Load dialogue data when start
//    void Awake()
//    {
//        LoadDialogues();

//    }

//    // Load dialogues file and deserialize it from JSON string into a dialogue list
//    void LoadDialogues()
//    {
//        TextAsset jsonFile = Resources.Load<TextAsset>("dialogues");
//        dialogueList = JsonUtility.FromJson<DialogueList>(jsonFile.text);
//    }

//    // Get a random dialogue
//    public DialogueData GetRandomDialogue()
//    {
//        if (usedIds.Count == dialogueList.dialogues.Length)
//        {
//            usedIds.Clear(); // Clear if all dialogues have been used
//        }

//        //Get a random dialogue that hasn't been used
//        int randomIndex = UnityEngine.Random.Range(0, dialogueList.dialogues.Length);
//        while (usedIds.Contains(dialogueList.dialogues[randomIndex].id))
//        {
//            randomIndex = UnityEngine.Random.Range(0, dialogueList.dialogues.Length);
//        }

//        DialogueData randomDialogue = dialogueList.dialogues[randomIndex];
//        usedIds.Add(randomDialogue.id);

//        return randomDialogue;
//    }

//    // Go through the dialogues one by one until there are no more left
//    public DialogueData GetNextDialogue()
//    {
//        if (usedIds.Count == dialogueList.dialogues.Length)
//        {
//            // Call the Game Manager for end game
//            ResetUsedIds();
//            GameManager.Instance.EndGame();
//        }

//        for (int i = 0; i < dialogueList.dialogues.Length; i++)
//        {
//            if (!usedIds.Contains(dialogueList.dialogues[i].id))
//            {
//                usedIds.Add(dialogueList.dialogues[i].id);
//                Debug.Log($"[DialogueManager] Returning dialogue ID: {dialogueList.dialogues[i].id}");
//                return dialogueList.dialogues[i];
//            }
//        }
//        Debug.LogWarning("No more dialogues left. End the game!");
//        return null;
//    }

//    // reset used ids 
//    public void ResetUsedIds()
//    {
//        usedIds.Clear();
//    }
//}


using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class DialogueData
{
    public int id;
    public int dishId;
    public string dishName;
    public string sentenceTemplate;
    public string color;
    public string image;
}

[Serializable]
public class DialogueList
{
    public DialogueData[] dialogues;
}

public class DialogueManager : MonoBehaviour
{
    private DialogueList dialogueList;
    private static HashSet<int> usedIds = new HashSet<int>();

    //Load dialogue data when start
    void Awake()
    {
        LoadDialogues();
    }

    // Load dialogues file and deserialize it from JSON string into a dialogue list
    void LoadDialogues()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("dialogues");
        dialogueList = JsonUtility.FromJson<DialogueList>(jsonFile.text);
    }

    // Get a random dialogue
    public DialogueData GetRandomDialogue()
    {
        if (usedIds.Count == dialogueList.dialogues.Length)
        {
            usedIds.Clear(); // Clear if all dialogues have been used
        }

        //Get a random dialogue that hasn't been used
        int randomIndex = UnityEngine.Random.Range(0, dialogueList.dialogues.Length);
        while (usedIds.Contains(dialogueList.dialogues[randomIndex].id))
        {
            randomIndex = UnityEngine.Random.Range(0, dialogueList.dialogues.Length);
        }

        DialogueData randomDialogue = dialogueList.dialogues[randomIndex];
        usedIds.Add(randomDialogue.id);
        return randomDialogue;
    }

    // Go through the dialogues one by one until there are no more left
    public DialogueData GetNextDialogue()
    {
        if (usedIds.Count == dialogueList.dialogues.Length)
        {
            // Call the Game Manager for end game
            ResetUsedIds();
            GameManager.Instance.EndGame();
        }

        for (int i = 0; i < dialogueList.dialogues.Length; i++)
        {
            if (!usedIds.Contains(dialogueList.dialogues[i].id))
            {
                usedIds.Add(dialogueList.dialogues[i].id);
                Debug.Log($"[DialogueManager] Returning dialogue ID: {dialogueList.dialogues[i].id}");
                return dialogueList.dialogues[i];
            }
        }

        Debug.LogWarning("No more dialogues left. End the game!");
        return null;
    }

    public DialogueData GetDialogueForDish(int dishId)
    {
        // Find and return the dialogue that matches the given dish ID
        for (int i = 0; i < dialogueList.dialogues.Length; i++)
        {
            if (dialogueList.dialogues[i].dishId == dishId)
            {
                Debug.Log($"[DialogueManager] Found dialogue for dish ID: {dishId}");
                return dialogueList.dialogues[i];
            }
        }

        // If no matching dialogue found, return the first one as fallback
        Debug.LogWarning($"[DialogueManager] No dialogue found for dish ID {dishId}, using first dialogue as fallback.");
        return dialogueList.dialogues[0];
    }

    // reset used ids 
    public void ResetUsedIds()
    {
        usedIds.Clear();
    }
}