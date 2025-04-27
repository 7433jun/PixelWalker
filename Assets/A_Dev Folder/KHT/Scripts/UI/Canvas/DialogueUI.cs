using CSVData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour, Iui
{
    [Header("Dialogue")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image portraitImage;
    [SerializeField] TextMeshProUGUI lineText;
    [SerializeField] Image arrowImage;

    [Header("Choice")]
    [SerializeField] GameObject choicePanel;
    [SerializeField] TextMeshProUGUI choice1Text;
    [SerializeField] TextMeshProUGUI choice2Text;
    [SerializeField] Image choiceImage1;
    [SerializeField] Image choiceImage2;

    private float nameFontSize;
    private float lineFontSize;
    private float choice1FontSize;
    private float choice2FontSize;

    private Canvas dialogueCanvas;
    private bool ischoosing;

    void Start()
    {
        GameManager.UI.SetIui(this);

        nameFontSize = nameText.fontSize;
        lineFontSize = lineText.fontSize;
        choice1FontSize = choice1Text.fontSize;
        choice2FontSize = choice2Text.fontSize;

        dialogueCanvas = GetComponent<Canvas>();

        UpdateLanguage();
    }

    public void DrawDialogue()
    {
        ShowDialogue();

        DialogueSentenceData dialogueSentenceData = GameManager.Dialogue.GetCurrentDialogueSentenceData();

        nameText.text = GameManager.Data.GetContentsString(dialogueSentenceData.NameStringIndex);
        portraitImage.sprite = Resources.Load<Sprite>(dialogueSentenceData.PortraitResourceName);
        lineText.text = GameManager.Data.GetDialogueString(dialogueSentenceData.DialogueStringIndex);

        arrowImage.enabled = true;
    }

    public void CloseDialogue()
    {
        ischoosing = false;
        dialogueCanvas.enabled = false;
    }

    private void ShowDialogue()
    {
        dialogueCanvas.enabled = true;
    }

    public void DialogueButton()
    {
        // if draw complete

        if (ischoosing) return;

        GameManager.Dialogue.NextDialogue();
    }

    public void DrawChoice()
    {
        ischoosing = true;

        choicePanel.SetActive(true);

        DialogueData dialogueData = GameManager.Dialogue.GetCurrentDialogueData();

        choice1Text.text = GameManager.Data.GetDialogueString(dialogueData.Choice01StringIndex);
        choice2Text.text = GameManager.Data.GetDialogueString(dialogueData.Choice02StringIndex);
    }

    public void CloseChoice()
    {
        ischoosing = false;

        choicePanel.SetActive(false);
    }

    public void ChoiceButton(int index)
    {
        CloseChoice();

        GameManager.Dialogue.Choice(index);
    }

    public void ChoiceEnter(int index)
    {
        choiceImage1.enabled = false;
        choiceImage2.enabled = false;

        if (index == 1)
        {
            choiceImage1.enabled = true;
        }
        else if (index == 2)
        {
            choiceImage2.enabled = true;
        }
    }

    public void UpdateLanguage()
    {
        nameText.font = GameManager.Data.GetFont();
        nameText.fontSize = GameManager.Data.GetFontSize(nameFontSize);

        lineText.font = GameManager.Data.GetFont();
        lineText.fontSize = GameManager.Data.GetFontSize(lineFontSize);

        choice1Text.font = GameManager.Data.GetFont();
        choice1Text.fontSize = GameManager.Data.GetFontSize(choice1FontSize);

        choice2Text.font = GameManager.Data.GetFont();
        choice2Text.fontSize = GameManager.Data.GetFontSize(choice2FontSize);
    }
}
