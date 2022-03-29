using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        //Save();
        Load();
        PerformanceEntry e = new PerformanceEntry() {
            entryDate = DateTime.UtcNow,
            textDescription = "Rogan did a bad job",
            category = "Bad Job",
            subcategory = "real bad job",
            leaderName = "Wes'"
        };
        List<PerformanceEntry> l = new List<PerformanceEntry>();
        l.Add(e);
        l.Add(e);
        l.Add(e);
        performanceTrackers = employees.GetPerformanceTrackers();
        PerformanceTracker p = new PerformanceTracker() {
            firstName = "Rogan",
            lastName = "Lyon",
            hireDate = DateTime.UtcNow,
            performanceEntries = l
        };
        performanceTrackers.Add(p);
        Debug.Log(employees.printStuff());
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
