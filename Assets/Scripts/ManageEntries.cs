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
    public PerformanceEntry performanceEntry;
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
            trackerManager = FindObjectOfType<ManageTrackers>();
            selectedEmployee = trackerManager.SelectedEmployee;
            performanceEntries = CreatePerformanceEntries();
            DisplayEntries();
        }
       
    }

        void DisplayEntries()
    {
        entryButton.name = "Back";
        TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform);
        foreach (PerformanceEntry entry in performanceEntries)
        {
            entryButton.name = entry.category;
            textMeshPro.SetText("Category: " + entry.category + "\n" + "Subcategory: " + entry.subcategory + "\n" + "Leader Name: " + entry.leaderName + "\n" + "Date: " + entry.entryDate.Day+"/"+entry.entryDate.Month+"/"+entry.entryDate.Year
            + "\n\n" + entry.textDescription);
            newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEntryPress(entry));
            // SetEntryText(newButton, entry, entryDisplayTransform);
            newButton.transform.SetParent(DisplayArea.transform);
        }
    }

    
    void OnEntryPress(PerformanceEntry entry)
    {
        performanceEntry = entry;
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
        range.Add(selectedEmployee.sheetName+"!A9:F990");
        IList<IList<object>> iterate = reader.getSheetRange(range)[0].Values;
        foreach(IList<object> row in iterate) {
            PerformanceEntry entry = new() {
                entryDate = DateTime.Parse((string)row[0]), 
                textDescription = (string)row[4],
                category = (string)row[2],
                subcategory = (string)row[3],
                leaderName = (string)row[1]
            };
            entries.Add(entry);
        }
        return entries;
    }
}
