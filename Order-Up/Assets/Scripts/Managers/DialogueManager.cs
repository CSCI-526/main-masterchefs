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
        GameData.CurrentDialogId = randomDialogue.id; // Update current level
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
                Debug.Log($"Dialogue ID: {dialogueList.dialogues[i].id} selected.");
                usedIds.Add(dialogueList.dialogues[i].id);
                // GameData.CurrentLevel = dialogueList.dialogues[i].id; // Update current level
                return dialogueList.dialogues[i];
            }
        }
        // No more dialogues left. End the game!"
        return null;
    }

    // reset used ids 
    public void ResetUsedIds()
    {
        usedIds.Clear();
    }
}
