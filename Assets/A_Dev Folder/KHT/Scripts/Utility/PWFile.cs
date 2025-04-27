using CSVData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class PWFile
{
    public static T LoadJson<T>(string filePath, string fileName)
    {
        string jsonText = File.ReadAllText($"{filePath}/{fileName}.json");
        return JsonConvert.DeserializeObject<T>(jsonText);
    }

    public static void SaveJson<T>(T jsonData, string filePath, string fileName)
    {
        string jsonText = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
        File.WriteAllText($"{filePath}/{fileName}.json", jsonText);
    }

    public static Dictionary<int, T> LoadCSVDataTable<T>(string filePath, string fileName) where T : IData, new()
    {
        Dictionary<int, T> dataTable = new Dictionary<int, T>();
    
        string[] lines = File.ReadAllLines($"{filePath}/{fileName}.csv");
        if (lines.Length < 2) return dataTable;
    
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
    
            if (int.TryParse(values[0], out int indexData) && CSVParser.TryParse<T>(values, out T parsedData))
            {
                dataTable.Add(indexData, parsedData);
            }
            else
            {
                Debug.LogError($"Failed to parse data at {filePath}/{fileName}.csv line : {i}");
            }
        }
    
        return dataTable;
    }

    public static Dictionary<int, List<string>> LoadCSVStringTable(string filePath, string fileName)
    {
        Dictionary<int, List<string>> stringTable = new Dictionary<int, List<string>>();

        using (StreamReader streamReader = new StreamReader($"{filePath}/{fileName}.csv"))
        {
            string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] values = Regex.Split(line, pattern);

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim().Trim('"');
                }

                List<string> stringData = new List<string>();

                if (int.TryParse(values[0], out int indexData) && values[1] != string.Empty)
                {
                    for (int j = 1; j < values.Length; j++)
                    {
                        stringData.Add(values[j]);
                    }
                    stringTable.Add(indexData, stringData);
                }
                else
                {
                    //Debug.LogError($"Failed to parse data at {filePath}/{fileName}.csv line : {i}");
                }
            }
        }

        return stringTable;
    }
}
