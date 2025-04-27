using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager
{
    private static StoreManager instance;
    public static StoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = new StoreManager();
            return instance;
        }
    }

    private Dictionary<int, IGoodsFunc> goodsFuncTable;

    private int currentStoreIndex;

    private StoreManager()
    {
        goodsFuncTable = new Dictionary<int, IGoodsFunc>();

        // init goodsFunc
        foreach (var kv in GameManager.Data.StoreDataTable)
        {
            foreach (int itemIndex in kv.Value.ItemList)
            {
                string goodsFuncName = $"Goods{itemIndex}";
                Type goodsFuncType = Type.GetType(goodsFuncName);
                if (goodsFuncType != null)
                {
                    IGoodsFunc foundGoodsFunc = (IGoodsFunc)Activator.CreateInstance(goodsFuncType);

                    goodsFuncTable.Add(itemIndex, foundGoodsFunc);
                }
                else
                {
                    goodsFuncTable.Add(itemIndex, new EmptyGoods());
                }
            }
        }
    }

    public void OpenStore(int storeIndex)
    {
        currentStoreIndex = storeIndex;

        GameManager.UI.StoreUI.DrawStore(storeIndex);
    }

    public int GetItemIndex(int slotIndex)
    {
        return GameManager.Data.StoreDataTable[currentStoreIndex].ItemList[slotIndex];
    }

    public void Buy(int itemIndex)
    {
        goodsFuncTable[itemIndex].Buy();
    }

    public bool CheckBuyable(int itemIndex)
    {
        return goodsFuncTable[itemIndex].ChechBuyable();
    }
}
