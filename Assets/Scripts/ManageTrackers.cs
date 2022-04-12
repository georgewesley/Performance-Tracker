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
        //Save();
        MainMenuPanel.SetActive(true); //Temporarily here, will move to back button (that goes back to main menu)
        PerformancePanel.SetActive(false);
    }

    void EntryView() {
        ManageEntriesPanel.SetActive(true);
        ManageTrackersPanel.SetActive(false);
    }

    void LoadFromSheets()
    {
        SheetsReader reader = FindObjectOfType<SheetsReader>();
        //reader.getSheetRange("EXAMPLE TRACKER!E9");
        string[] fullName;
        string firstName;
        string lastName;
        DateTime hireDate = DateTime.Now;
        List<PerformanceEntry> performanceEntries = new();
        foreach (string name in reader.GetNames())
        {
            try
            {
                Debug.Log(name);
                fullName = name.Split(" ");
                firstName = fullName[0];
                lastName = fullName[1];
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