using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Apis.Sheets.v4.Data;
public class  ManageTrackers : MonoBehaviour
{
    public Employees employees;
    [SerializeField] Button button;
    [SerializeField] Button entryButton;
    [SerializeField] GameObject ManageTrackersPanel;
    [SerializeField] GameObject ManageEntriesPanel;
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject PerformancePanel;
    [SerializeField] GameObject GenerateEmployeePanel;
    public List<PerformanceTracker> performanceTrackers;
    public GameObject DisplayArea;
    public Vector3 DisplayAreaTransform;
    public static PerformanceTracker SelectedEmployee;
    void Start()
    {
        LoadFromSheets();

        DisplayArea = GetComponentInChildren<GridLayoutGroup>().gameObject;
        DisplayAreaTransform = DisplayArea.GetComponent<Transform>().position;
        DisplayEmployees();    
    }
    void DisplayEmployees(){
        entryButton.name = "Back";
        TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI trackerTextMeshPro;
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform, false);
        foreach(PerformanceTracker tracker in performanceTrackers) { //should create a method 
            button.name = tracker.firstName;
            trackerTextMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
            string buttonName = tracker.firstName;
            if (tracker.lastName != "") { //if no last name indexing will create an error; indexing empty string is an error
                buttonName = buttonName + " " + tracker.lastName[0];
            }
            trackerTextMeshPro.SetText(buttonName);
            newButton = Instantiate<Button>(button, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEmployeePress(tracker));
            newButton.transform.SetParent(DisplayArea.transform, false);
        }
        entryButton.name = "New Employee";
        textMeshPro.SetText("Create New Employee");
        newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => CreateEmployeePress());
        newButton.transform.SetParent(DisplayArea.transform, false);
    }
    void OnEmployeePress(PerformanceTracker performanceTracker) {
        SelectedEmployee = performanceTracker;
        EntryView();
    }
    void OnBackPress() {
        //Save();
        MainMenuPanel.SetActive(true); //Temporarily here, will move to back button (that goes back to main menu)
        PerformancePanel.SetActive(false);
    }

    void CreateEmployeePress()
    {
        Debug.Log("We did correct thing");
        GenerateEmployeePanel.SetActive(true);
        ManageTrackersPanel.SetActive(false);
    }

    void EntryView() {
        ManageEntriesPanel.SetActive(true);
        ManageTrackersPanel.SetActive(false);
    }
    
    void LoadFromSheets()
    {
        SheetsReader reader = FindObjectOfType<SheetsReader>();
        string[] fullName;
        string firstName;
        string lastName = "";
        DateTime hireDate = DateTime.Now;
        List<PerformanceEntry> performanceEntries = new();
        foreach (string name in reader.GetNames()[0])
        {
            try
            {
                fullName = name.Split(" ");
                firstName = fullName[0];
                if(fullName.Length-1 != 0) { 
                    lastName = fullName[fullName.Length-1]; //if they only have one name this will be the same as first name
                } 
                CreateEmployee(firstName, lastName, name.Trim(), hireDate, performanceEntries);
            }
            catch{}
        }
    }

    void CreateEmployee(string firstName, string lastName, string sheetName, DateTime hireDate, List<PerformanceEntry> performanceEntries)
    {
        PerformanceTracker employee = new PerformanceTracker()
        {
            firstName = firstName, 
            lastName = lastName,
            sheetName = sheetName,
            hireDate = hireDate,
            performanceEntries = performanceEntries
        };
        performanceTrackers.Add(employee);
        //Save();
    }

    public void AddNewEmployeeAfterLoad(string name) {
        string[] fullName = name.Split(" ");
        string firstName = fullName[0];
        string lastName = "";
        List<PerformanceEntry> performanceEntries = new();
        if(fullName.Length-1 != 0) { 
            lastName = fullName[fullName.Length-1]; //if they only have one name this will be the same as first name
        } 
        CreateEmployee(firstName, lastName, name.Trim(), DateTime.Now, performanceEntries); 
        DisplayNewEmployee();
    }

    private void DisplayNewEmployee() {
        PerformanceTracker newTracker = performanceTrackers[performanceTrackers.Count-1];
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        button.name = newTracker.firstName;
        TextMeshProUGUI trackerTextMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
        string buttonName = newTracker.firstName;
        if (newTracker.lastName != "") { //if no last name indexing will create an error; indexing empty string is an error
            buttonName = buttonName + " " + newTracker.lastName[0];
        }
        trackerTextMeshPro.SetText(buttonName);
        newButton = Instantiate<Button>(button, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnEmployeePress(newTracker));
        newButton.transform.SetParent(DisplayArea.transform, false);
    }
}

    // void Save() {
    //     string save = JsonUtility.ToJson(employees);
    //     System.IO.File.WriteAllText(Application.persistentDataPath + "/EmployeeData.json", save);
    // }
    // void Load() {
    //     try
    //     {
    //         string load = System.IO.File.ReadAllText(Application.persistentDataPath + "/EmployeeData.json");
    //         employees = JsonUtility.FromJson<Employees>(load);
    //     }
    //     catch
    //     {
    //         //create JSON file
    //         Debug.Log("We failed");
    //         Save();
    //     }
    // }