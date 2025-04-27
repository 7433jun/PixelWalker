using UnityEngine;
using Project.Enums;
using CSVData;
using System.Collections.Generic;

public class KHTTest : MonoBehaviour
{
    void Start()
    {
        //PlayerBase playerBase = UtilityLibrary.GetPlayerCharacterInGame();
        //
        //playerBase.PlayerSpec.AddGold(1000);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            GameManager.UI.ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameManager.UI.ToggleStore(101);
            //GameManager.UI.ToggleShopCanvas(0);
            //Buff.OnUI(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameManager.UI.ToggleStore(0);
            //Buff.OnUI(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameManager.Dialogue.StartDialogue(930101);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GameManager.Dialogue.StartDialogue(990101);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            
        }
    }
}