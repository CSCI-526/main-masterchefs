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
    private HashSet<int> usedIds = new HashSet<int>();

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
}
