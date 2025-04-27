using CSVData;
using System;
using UnityEngine;
using Project.Enums;

public class DialogueManager
{
    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get
        {
            if (instance == null)
                instance = new DialogueManager();
            return instance;
        }
    }

    private int currentDialogueIndex;
    private int currentDialogueSentenceIndex;

    private DialogueManager()
    {

    }

    public void StartDialogue(int dialogueIndex)
    {
        currentDialogueIndex = dialogueIndex;
        currentDialogueSentenceIndex = GameManager.Data.DialogueDataTable[dialogueIndex].StartDialogueSentenceIndex;

        CallDialogue();
    }

    public void NextDialogue()
    {
        currentDialogueSentenceIndex++;
        CallDialogue();
    }

    public void Choice(int index)
    {
        if (index == 1)
        {
            StartDialogue(GetCurrentDialogueData().Choice01DialogueIndex);
        }
        else if (index == 2)
        {
            StartDialogue(GetCurrentDialogueData().Choice02DialogueIndex);
        }
    }

    private void CallDialogue()
    {
        DialogueData dialogueData = GameManager.Data.DialogueDataTable[currentDialogueIndex];

        if (currentDialogueSentenceIndex <= dialogueData.EndDialogueSentenceIndex)
        {
            DialogueAction();
            GameManager.UI.DialogueUI.DrawDialogue();
        }
        else
        {
            switch (dialogueData.NextDialogueType)
            {
                case NextDialogueType.End:
                    GameManager.UI.DialogueUI.CloseDialogue();
                    break;
                case NextDialogueType.Dialogue:
                    StartDialogue(dialogueData.NextDialogueIndex);
                    break;
                case NextDialogueType.Choice:
                    GameManager.UI.DialogueUI.DrawChoice();
                    break;
            }
        }
    }

    private void DialogueAction()
    {
        string dialogueFuncName = $"Dialogue{currentDialogueSentenceIndex}";
        Type dialogueFuncType = Type.GetType(dialogueFuncName);
        if (dialogueFuncType != null)
        {
            IDialogueFunc foundDialogueFunc = (IDialogueFunc)Activator.CreateInstance(dialogueFuncType);

            foundDialogueFunc.Action();
        }
    }

    public DialogueData GetCurrentDialogueData()
    {
        return GameManager.Data.DialogueDataTable[currentDialogueIndex];
    }

    public DialogueSentenceData GetCurrentDialogueSentenceData()
    {
        return GameManager.Data.DialogueSentenceDataTable[currentDialogueSentenceIndex];
    }
}
