using UnityEngine;
using Project.Enums;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace CSVData
{
    public interface IData
    {

    }

    public static class CSVParser
    {
        public static bool TryParse<T>(string[] values, out T parsedData) where T : IData, new()
        {
            parsedData = new T();
            var properties = typeof(T).GetProperties();

            if (values.Length <= properties.Length) return false;

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                string value = values[i + 1];

                if (value == "NULL")
                {
                    continue;
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(parsedData, value);
                }
                else if (property.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
                {
                    property.SetValue(parsedData, intValue);
                }
                else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                {
                    property.SetValue(parsedData, floatValue);
                }
                else if (property.PropertyType == typeof(bool) && TryParseBoolCSV(value, out bool boolValue))
                {
                    property.SetValue(parsedData, boolValue);
                }
                else if (property.PropertyType.IsEnum && int.TryParse(value, out int enumValueIndex))
                {
                    var enumValue = Enum.ToObject(property.PropertyType, enumValueIndex);
                    property.SetValue(parsedData, enumValue);
                }
                else if (property.PropertyType == typeof(List<int>))
                {
                    List<int> intListValue = new List<int>();

                    for (int j = i + 1; j < values.Length; j++)
                    {
                        if (int.TryParse(values[j], out int eIntValue))
                        {
                            intListValue.Add(eIntValue);
                        }
                    }
                    property.SetValue(parsedData, intListValue);

                    break;
                }
                else
                {
                    // Wrong DataType : Add type funciton or Check
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseBoolCSV(string value, out bool result)
        {
            if (value == "0")
            {
                result = false;
                return true;
            }
            else if (value == "1")
            {
                result = true;
                return true;
            }

            result = false;
            return false;
        }
    }

    public class CharacterData : IData
    {
        public string StringKey { get; private set; }
        public int NameStringIndex { get; private set; }
        public CharacterType CharacterType { get; private set; }
    }

    public class StatusData : IData
    {

    }

    public class ItemData : IData
    {
        public string StringKey { get; private set; }
        public int NameStringIndex { get; private set; }
        public int DescriptionStringIndex { get; private set; }
        public ItemType ItemType { get; private set; }
        public bool IsStack { get; private set; }
        public int MaxStackCount { get; private set; }
        public PriceType PriceType { get; private set; }
        public int BuyPrice { get; private set; }
        public int InventorySlotIndex { get; private set; }
        public int Cost { get; private set; }
        public int EffectValue { get; private set; }
        public string IconResourceName { get; private set; }
    }

    public class DialogueData : IData
    {
        public string StringKey { get; private set; }
        public int StartDialogueSentenceIndex { get; private set; }
        public int EndDialogueSentenceIndex { get; private set; }
        public NextDialogueType NextDialogueType { get; private set; }
        public int NextDialogueIndex { get; private set; }
        public int Choice01StringIndex { get; private set; }
        public int Choice01DialogueIndex { get; private set; }
        public int Choice02StringIndex { get; private set; }
        public int Choice02DialogueIndex { get; private set; }
    }

    public class DialogueSentenceData : IData
    {
        public int NameStringIndex { get; private set; }
        public int DialogueStringIndex { get; private set; }
        public string PortraitResourceName { get; private set; }
    }

    public class AchievementData : IData
    {
        public int NameStringIndex { get; private set; }
        public int DescriptionStringIndex { get; private set; }
        public AchievementType AchievementType { get; private set; }
        public AchievementMethodType AchievementMethodType { get; private set; }
        public float TargetPositionX { get; private set; }
        public float TargetPositionY { get; private set; }
        public int TargetItemIndex { get; private set; }
        public int TargetMonsterTarget { get; private set; }
        public int TargetCount { get; private set; }
        public PriceType RewardPriceType { get; private set; }
        public int RewardPriceCount { get; private set; }
        public int RewardItemIndex { get; private set; }
        public int RewardItemCount { get; private set; }
    }

    public class StoreData : IData
    {
        public List<int> ItemList { get; private set; }
    }
}