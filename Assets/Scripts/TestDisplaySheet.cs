using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

public class TestDisplaySheet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<string> notNames = new() { "MASTER Performance Board", "EXAMPLE TRACKER", "*DO NOT USE* Perf Tracker Template", "Copy of *DO NOT USE* Perf Tracker Template", "*DO NOT USE* - LEGEND" };
        List<Sheet> yesNames = new();
        SheetsReader reader = FindObjectOfType<SheetsReader>();
        //reader.getSheetRange("EXAMPLE TRACKER!E9");
        string title;
        foreach (Sheet sheet in reader.GetAllSheets())
        {
            title = sheet.Properties.Title;
            Debug.Log(title);
            if (!notNames.Contains(title)) {
                yesNames.Add(sheet);
            }
        }
    }
}
