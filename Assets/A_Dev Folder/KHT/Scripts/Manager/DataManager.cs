using CSVData;
using Project.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataManager
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
                instance = new DataManager();
            return instance;
        }
    }

    private Dictionary<int, List<string>> contentsStringTable;
    private Dictionary<int, List<string>> dialogueStringTable;

    public Dictionary<int, CharacterData> CharacterDataTable { get; private set; }
    public Dictionary<int, StatusData> StatusDataTable { get; private set; }
    public Dictionary<int, ItemData> ItemDataTable { get; private set; }
    public Dictionary<int, DialogueData> DialogueDataTable { get; private set; }
    public Dictionary<int, DialogueSentenceData> DialogueSentenceDataTable { get; private set; }
    public Dictionary<int, AchievementData> AchievementDataTable { get; private set; }
    public Dictionary<int, StoreData> StoreDataTable { get; private set; }

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> systemText;
    private List<TMP_FontAsset> fontList;
    private TMP_FontAsset defaultFont;

    private DataManager()
    {
        LoadFile();

        //foreach (var kv in StoreDataTable)
        //{
        //    foreach (var i in kv.Value.ItemList)
        //    {
        //        Debug.Log(i);
        //    }
        //}
    }

    private void LoadFile()
    {
        contentsStringTable = PWFile.LoadCSVStringTable("Assets/A_Dev Folder/PHK/CSV/String", "Contents_String");
        dialogueStringTable = PWFile.LoadCSVStringTable("Assets/A_Dev Folder/PHK/CSV/String", "Dialogue_String");

        CharacterDataTable = PWFile.LoadCSVDataTable<CharacterData>("Assets/A_Dev Folder/PHK/CSV/Character", "Character");
        //statusDataTable = PWFile.LoadCSVDataTable<StatusData>("Assets/A_Dev Folder/PHK/CSV/Character", "Status");
        ItemDataTable = PWFile.LoadCSVDataTable<ItemData>("Assets/A_Dev Folder/PHK/CSV/Item", "Item");
        DialogueDataTable = PWFile.LoadCSVDataTable<DialogueData>("Assets/A_Dev Folder/PHK/CSV/Dialogue", "Dialogue");
        DialogueSentenceDataTable = PWFile.LoadCSVDataTable<DialogueSentenceData>("Assets/A_Dev Folder/PHK/CSV/Dialogue", "Dialogue_Sentence");
        //achievementDataTable = PWFile.LoadCSVDataTable<AchievementData>("Assets/A_Dev Folder/PHK/CSV/Achievement", "Achievement");
        StoreDataTable = PWFile.LoadCSVDataTable<StoreData>("Assets/A_Dev Folder/PHK/CSV/Store", "Store");

        systemText = PWFile.LoadJson<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>("Assets/A_Dev Folder/KHT/Json", "System");

        FontInfo fontInfo = Resources.Load<FontInfo>("KHT/ScriptableObjects/Font Info");
        fontList = fontInfo.TMP_FontList;
        defaultFont = fontInfo.DefaultTMP_Font;
    }

    public string GetContentsString(int index, string replacement = "{value}")
    {
        string placeholder = "{value}";
        string contents = string.Empty;

        if (contentsStringTable.ContainsKey(index))
        {
            contents = contentsStringTable[index][(int)SettingManager.SettingData.languageValue];

            if (contents.Contains(placeholder))
            {
                contents = contents.Replace(placeholder, replacement);
            }
        }
        else
        {
            Debug.LogError($"ContentsStringTable is not contain {index}");
        }

        return contents;
    }

    public string GetDialogueString(int index)
    {
        string dialogue = string.Empty;

        if (dialogueStringTable.ContainsKey(index))
        {
            dialogue = dialogueStringTable[index][(int)SettingManager.SettingData.languageValue];
        }
        else
        {
            Debug.LogError($"DialogueStringTable is not contain {index}");
        }

        return dialogue;
    }

    public string GetSystemText(string category, string key)
    {
        if (systemText.TryGetValue(category, out var categoryData) &&
            categoryData.TryGetValue(key, out var localizationData) &&
            localizationData.TryGetValue(SettingManager.SettingData.languageValue.ToString(), out var value))
        {
            return value;
        }

        return $"Wrong {category}/{key}";
    }

    public TMP_FontAsset GetFont()
    {
        if ((int)SettingManager.SettingData.languageValue < fontList.Count)
            return fontList[(int)SettingManager.SettingData.languageValue];
        else
            return defaultFont;
    }

    public float GetFontSize(float baseSize)
    {
        switch(SettingManager.SettingData.languageValue)
        {
            case Language.EN: baseSize *= 1.0f; break;
            case Language.KO: baseSize *= 0.9f; break;
        }

        return baseSize;
    }
}
