using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    // File path to the CSV
    private static string filePath;

    public static void SetFilePath(string cursorType)
    {
        filePath = Path.Combine(Application.dataPath, $"data_{cursorType}.csv");
    }

    // Method to write data to a CSV file
    public static void WriteToCSV(List<string[]> data)
    {
        using (StreamWriter sw = new(filePath))
        {
            foreach (string[] line in data)
            {
                sw.WriteLine(string.Join(",", line));
            }
        }
    }

    public static void AppendToCSV(string[] data)
    {
        // Use StreamWriter with append set to true
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine(string.Join(",", data));
        }

        Debug.Log($"Data successfully appended to {filePath}");
    }
}
