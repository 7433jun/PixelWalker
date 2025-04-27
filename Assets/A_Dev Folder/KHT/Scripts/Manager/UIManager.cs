using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            return instance;
        }
    }

    // ¼öÁ¤Áß
    private List<Iui> uiList;
    private SettingMenuUI settingMenuUI;
    private HandbookUI handbookUI;
    private MapUI mapUI;
    private InventoryUI inventoryUI;
    private AchievementUI achievementUI;
    private StoreUI storeUI;
    private DialogueUI dialogueUI;

    public List<Iui> UIList => Instance.uiList;
    public SettingMenuUI SettingMenu => settingMenuUI;
    public HandbookUI HandbookUI => handbookUI;
    public MapUI MapUI => mapUI;
    public InventoryUI InventoryUI => inventoryUI;
    public AchievementUI AchievementUI => achievementUI;
    public StoreUI StoreUI => storeUI;
    public DialogueUI DialogueUI => dialogueUI;

    private UIManager()
    {
        uiList = new List<Iui>();
    }

    public void SetIui(Iui ui)
    {
        UIList.Add(ui);

        var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        var uiType = ui.GetType();

        foreach (var field in fields)
        {
            if (field.FieldType == uiType)
            {
                field.SetValue(this, ui);
                break;
            }
        }
    }

    public void UpdateLanguageUIList()
    {
        foreach (Iui ui in UIList)
        {
            ui.UpdateLanguage();
        }
    }

    public void ToggleInventory()
    {
        //bool toggleCanvas = !inventoryCanvas.enabled;
        //
        //if (toggleCanvas)
        //{
        //    UtilityLibrary.GetPlayerControllerInGame().EnableActionControl(false);
        //
        //    //DrawInventory();
        //    //DrawStats();
        //    //DrawInfo(0);
        //}
        //else
        //{
        //    UtilityLibrary.GetPlayerControllerInGame().EnableActionControl(true);
        //}
        //
        //inventoryCanvas.enabled = toggleCanvas;

        if (!handbookUI.Canvas.enabled || !inventoryUI.Canvas.enabled)
        {
            handbookUI.Canvas.enabled = true;

            mapUI.Canvas.enabled = false;
            achievementUI.Canvas.enabled = false;

            inventoryUI.Canvas.enabled = true;
        }
        else
        {
            handbookUI.Canvas.enabled = false;
        }
    }

    public void ToggleStore(int storeIndex)
    {
        Canvas storeCanvas = storeUI.GetComponent<Canvas>();
        bool toggleCanvas = !storeCanvas.enabled;

        if (toggleCanvas)
        {
            GameManager.Store.OpenStore(storeIndex);
        }
        else
        {

        }

        storeCanvas.enabled = toggleCanvas;
    }
}
