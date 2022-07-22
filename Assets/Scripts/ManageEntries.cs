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
    public GameObject selectedGameObject;
    public bool isNewEntry;
    public Button createNewEntryButton;
    private Vector3 DisplayAreaTransform;
    private ManageTrackers trackerManager;
    private List<PerformanceEntry> performanceEntries;
    private PerformanceTracker selectedEmployee;
    private GameObject DisplayArea;
    
    void OnEnable() //if we used start this would happen before trackerManger is even ready to be called
    {
        DisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
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
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform, false); //false is needed here or the canvas scaler will not be used

        int count = 8; //starts at 8 because 9 is where the rows start on the performance tracker and we do + 1 down below. So in the case where there are 0 current rows we want it to be 8+1. 
        foreach (PerformanceEntry entry in performanceEntries)
        {
            CreateEntryButton(entry);
            count += 1;
        }
        CreateNewEntryButton(count+1); 
    }

        public Button CreateNewEntryButton(int rowForEntry)
        {
            TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.SetText("New Entry");
            createNewEntryButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            PerformanceEntry performanceEntry = new()
            {
                entryDate = DateTime.Now,
                textDescription = "",
                category = "",
                subcategory = "",
                leaderName = "",
                row = rowForEntry
            };
            //below line must not occur at the start of this method because that is before it is declared and will result in null referance
            createNewEntryButton.onClick.RemoveAllListeners(); //this should reset the listeners so that only the current ones are used
            createNewEntryButton.onClick.AddListener(() => OnEntryPress(performanceEntry, createNewEntryButton.gameObject));
            createNewEntryButton.transform.SetParent(DisplayArea.transform, false);
            return createNewEntryButton;
        }

        public Transform CreateEntryButton(PerformanceEntry entry)
        {
            Debug.Log("I Created an Entry!! Wow wow wow");
            var textMeshProEntry = entryButton.GetComponentInChildren<TextMeshProUGUI>();
            entryButton.name = entry.category;
            textMeshProEntry.SetText("Category: " + entry.category + "\n" + "Subcategory: " + entry.subcategory + "\n" + "Leader Name: " + entry.leaderName + "\n" + "Date: " + entry.entryDate.Day+"/"+entry.entryDate.Month+"/"+entry.entryDate.Year
                                     + "\n\n" + entry.textDescription);
            var newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEntryPress(entry, newButton.gameObject, false)); //we are not saying that this is not a new button, it is, we are saying that when we press this button it will not create a new entry
            // SetEntryText(newButton, entry, entryDisplayTransform);
            newButton.transform.SetParent(DisplayArea.transform, false);
            return newButton.transform;
        }


        void OnEntryPress(PerformanceEntry entry, GameObject button, bool newEntry = true)
        {
            isNewEntry = newEntry; 
            selectedPerformanceEntry = entry;
            selectedGameObject = button;
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
