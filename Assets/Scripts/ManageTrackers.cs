using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Apis.Sheets.v4.Data;
public class ManageTrackers : MonoBehaviour
{
    public Employees employees;
    [SerializeField] Button button;
    [SerializeField] Button entryButton;
    [SerializeField] GameObject ManageTrackersPanel;
    [SerializeField] GameObject ManageEntriesPanel;
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject PerformancePanel;
    public List<PerformanceTracker> performanceTrackers;

    public GameObject DisplayArea;
    public Vector3 DisplayAreaTransform;

    public PerformanceTracker SelectedEmployee;
    void Start()
    {
        Save();
        Load();
        LoadFromSheets();
        /*PerformanceEntry e = new PerformanceEntry() {
            entryDate = DateTime.UtcNow,
            textDescription = "Test did a bad bad bad job testing testing testing testing testing testing testing",
            category = "Bad Job",
            subcategory = "testing123",
            leaderName = "Wes'"
        };
        List<PerformanceEntry> l = new List<PerformanceEntry>();
        l.Add(e);
        l.Add(e);
        l.Add(e);
        performanceTrackers = employees.GetPerformanceTrackers();
        PerformanceTracker p = new PerformanceTracker() {
            firstName = "TEST",
            lastName = "Lyon",
            hireDate = DateTime.UtcNow,
            performanceEntries = l
        };
        performanceTrackers.Add(p);
        Debug.Log(employees.printStuff());*/

        DisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        DisplayAreaTransform = DisplayArea.GetComponent<Transform>().position;
        DisplayEmployees();    


    }
    void DisplayEmployees(){
        entryButton.name = "Back";
        TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform);
        foreach(PerformanceTracker tracker in performanceTrackers) {
            button.name = tracker.firstName;
            textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.SetText(tracker.firstName);
            newButton = Instantiate<Button>(button, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEmployeePress(tracker));
            newButton.transform.SetParent(DisplayArea.transform);
        }
    }
    void OnEmployeePress(PerformanceTracker performanceTracker) {
        SelectedEmployee = performanceTracker;
        EntryView();
    }
    void OnBackPress() {
        Save();
        MainMenuPanel.SetActive(true); //Temporarily here, will move to back button (that goes back to main menu)
        PerformancePanel.SetActive(false);
    }

    void Save() {
        string save = JsonUtility.ToJson(employees);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/EmployeeData.json", save);
    }
    void Load() {
        try
        {
            string load = System.IO.File.ReadAllText(Application.persistentDataPath + "/EmployeeData.json");
            employees = JsonUtility.FromJson<Employees>(load);
        }
        catch
        {
            //create JSON file
            Debug.Log("We failed");
            Save();
        }
    }

    void EntryView() {
        ManageEntriesPanel.SetActive(true);
        ManageTrackersPanel.SetActive(false);
    }

    void LoadFromSheets()
    {
        List<string> notNames = new() { "MASTER Performance Board", "EXAMPLE TRACKER", "*DO NOT USE* Perf Tracker Template", "Copy of *DO NOT USE* Perf Tracker Template", "*DO NOT USE* - LEGEND" };
        SheetsReader reader = FindObjectOfType<SheetsReader>();
        //reader.getSheetRange("EXAMPLE TRACKER!E9");
        string title;
        string[] fullName;
        string firstName;
        string lastName;
        DateTime hireDate = DateTime.Now;
        List<PerformanceEntry> performanceEntries = new();
        foreach (Sheet sheet in reader.GetAllSheets())
        {
            title = sheet.Properties.Title;
            Debug.Log(title);
            if (!notNames.Contains(title))
            {
                try
                {
                    Debug.Log(title);
                    fullName = ((string)reader.getSheetRange(title + "!B3")[0][0]).Split(" ");
                    firstName = fullName[0];
                    lastName = fullName[1];
                    CreateEmployee(firstName, lastName, hireDate, performanceEntries);
                }
                catch
                {
                    
                }
            }
        }
    }

    void CreateEmployee(string firstName, string lastName, DateTime hireDate, List<PerformanceEntry> performanceEntries)
    {
        PerformanceTracker employee = new PerformanceTracker()
        {
            firstName = firstName, 
            lastName = lastName,
            hireDate = hireDate,
            performanceEntries = performanceEntries
        };
        performanceTrackers.Add(employee);
        Save();
    }
}
