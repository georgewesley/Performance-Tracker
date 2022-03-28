using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageTrackers : MonoBehaviour
{
    [SerializeField] Employees employees;
    [SerializeField] Button button;

    [SerializeField] Button entryButton;
    private List<PerformanceTracker> performanceTrackers;
    void Start()
    {
        //Save();
        Load();
        PerformanceEntry e = new PerformanceEntry() {
            entryDate = DateTime.UtcNow,
            textDescription = "Wes did a bad job",
            category = "Bad Job",
            subcategory = "real bad job",
            leaderName = "t"
        };
        List<PerformanceEntry> l = new List<PerformanceEntry>();
        l.Add(e);
        l.Add(e);
        l.Add(e);
        performanceTrackers = employees.GetPerformanceTrackers();
        PerformanceTracker p = new PerformanceTracker() {
            firstName = "k",
            lastName = "Lyon",
            hireDate = DateTime.UtcNow,
            performanceEntries = l
        };
        performanceTrackers.Add(p);
        Debug.Log(employees.printStuff());
        DisplayEmployees();    


    }
    void DisplayEmployees(){
        GameObject PerformanceTrackersDisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        Vector3 transform = PerformanceTrackersDisplayArea.GetComponent<Transform>().position;
        foreach(PerformanceTracker tracker in performanceTrackers) {
            button.name = tracker.firstName;
            TextMeshProUGUI textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.SetText(tracker.firstName);
            Button newButton = Instantiate<Button>(button, new Vector3(transform.x, transform.y, transform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEmployeePress(tracker));
            newButton.transform.SetParent(PerformanceTrackersDisplayArea.transform);
        }
    }

    async void DisplayEntries(PerformanceTracker performanceTracker)
    {
        GameObject EntryDisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        Vector3 entryDisplayTransform = EntryDisplayArea.GetComponent<Transform>().position;
        foreach (PerformanceEntry entry in performanceTracker.performanceEntries)
        {
            entryButton.name = entry.category;
            TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.SetText("Category: " + entry.category + "\n" + "Subcategory: " + entry.subcategory + "\n" + "Leader Name: " + entry.leaderName + "\n" + "Date: " + entry.entryDate.Day+"/"+entry.entryDate.Month+"/"+entry.entryDate.Year);
            Button newButton = Instantiate<Button>(entryButton, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnEntryPress(entry));
            // SetEntryText(newButton, entry, entryDisplayTransform);
            newButton.transform.SetParent(EntryDisplayArea.transform);
        }
    }
    void OnEmployeePress(PerformanceTracker performanceTracker) {
        DisplayEntries(performanceTracker);
    }
    void OnEntryPress(PerformanceEntry entry)
    {
        Save();
        Debug.Log(entry.subcategory);
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

    // void SetEntryText(Button newButton, PerformanceEntry entry, Vector3 entryDisplayTransform)
    // {
    //     TextMeshProUGUI newText = Instantiate<TextMeshProUGUI>(entryButtonText, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
    //     newText.text = "Category: " + entry.category;
    //     newText.transform.SetParent(newButton.transform);

    //     TextMeshProUGUI newText1 = Instantiate<TextMeshProUGUI>(entryButtonText, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
    //     newText1.text = "Subcategory: " + entry.subcategory;
    //     newText1.transform.SetParent(newButton.transform);

    //     TextMeshProUGUI newText2 = Instantiate<TextMeshProUGUI>(entryButtonText, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
    //     newText2.text = "Leader Name: " + entry.leaderName;
    //     newText2.transform.SetParent(newButton.transform);
    // }

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
