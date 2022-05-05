using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageEntries : MonoBehaviour
{
    [SerializeField] Button entryButton;
    [SerializeField] GameObject ManageTrackersPanel;
    [SerializeField] GameObject ManageEntriesPanel;
    [SerializeField] GameObject EntryPanel;
    public PerformanceEntry selectedPerformanceEntry;
    private ManageTrackers trackerManager;
    private List<PerformanceEntry> performanceEntries;
    private PerformanceTracker selectedEmployee;
    private GameObject DisplayArea;
    private Vector3 DisplayAreaTransform;
    void OnEnable() //if we used start this would happen before trackerManger is even ready to be called
    {
        DisplayArea = GetComponentInChildren<GridLayoutGroup>().gameObject;
        DisplayAreaTransform = DisplayArea.GetComponent<Transform>().position;
        if(DisplayArea.transform.childCount==0)
        {
            selectedEmployee = ManageTrackers.SelectedEmployee;
            performanceEntries = CreatePerformanceEntries();
            DisplayEntries();
        }
       
    }

        void DisplayEntries()
    {
        entryButton.name = "Back";
        TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI textMeshProEntry = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform, false); //false is needed here or the canvas scaler will not be used

        int count = 8; //starts at 8 because 9 is where the rows start on the performance tracker and we do + 1 down below. So in the case where there are 0 current rows we want it to be 8+1. 
        foreach (PerformanceEntry entry in performanceEntries)
        {
            entryButton.name = entry.category;
            textMeshProEntry.SetText("Category: " + entry.category + "\n" + "Subcategory: " + entry.subcategory + "\n" + "Leader Name: " + entry.leaderName + "\n" + "Date: " + entry.entryDate.Day+"/"+entry.entryDate.Month+"/"+entry.entryDate.Year
            + "\n\n" + entry.textDescription);
            newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEntryPress(entry));
            // SetEntryText(newButton, entry, entryDisplayTransform);
            newButton.transform.SetParent(DisplayArea.transform, false);
            count += 1;
        }
        textMeshPro.SetText("New Entry");
        newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        PerformanceEntry performanceEntry = new()
        {
            entryDate = DateTime.Now,
            textDescription = "",
            category = "",
            subcategory = "",
            leaderName = "",
            row = count+1
        };
        newButton.onClick.AddListener(() => OnEntryPress(performanceEntry));
        newButton.transform.SetParent(DisplayArea.transform, false);
    }

    
    void OnEntryPress(PerformanceEntry entry)
    {
        selectedPerformanceEntry = entry;
        EntryPanel.SetActive(true);
        ManageEntriesPanel.SetActive(false);
    }

    void OnBackPress() {
        ManageTrackersPanel.SetActive(true); //Temporarily here, will move to back button (that goes back to main menu)
        ManageEntriesPanel.SetActive(false);
        RemoveEntries();
    }

    void RemoveEntries() { 
        for (int i = 0; i < DisplayArea.transform.childCount; i++)
             Destroy(DisplayArea.transform.GetChild(i).gameObject);
    }

    List<PerformanceEntry> CreatePerformanceEntries() { //This is worse design than CreateEmployee from ManageTrackers. Will change to that style later.
        List<PerformanceEntry> entries = new();
        SheetsReader reader = FindObjectOfType<SheetsReader>();
        List<string> range = new();
        DateTime date = new();
        range.Add(selectedEmployee.sheetName+"!A9:F990");
        IList<IList<object>> iterate = reader.getBatchData(range)[0].Values;
        int count = 9; //represents what row we are in, starts at 9 because that is the row above that we start at (on the sheet)
        if(iterate is null)
        {
            return entries;
        }
        foreach(IList<object> row in iterate) {
            List<string> validRow = ValidateRow(row);
            try {
                date = DateTime.Parse((string)validRow[0]);
            }
            catch{
                date = DateTime.MaxValue;
            }
            PerformanceEntry entry = new() {
                entryDate = date, 
                textDescription = (string)validRow[4],
                category = (string)validRow[2],
                subcategory = (string)validRow[3],
                leaderName = (string)validRow[1],
                row = count
            };
            count += 1;
            entries.Add(entry);
        }
        return entries;
    }
    List<string> ValidateRow(IList<object> row) {
        List<string> validList = new();
        for (int i = 0; i<5; i++) {
            try {
                validList.Add((string) row[i]);
            }
            catch {
                validList.Add("No Entry");
            }
        }
        return validList;
    }
}
