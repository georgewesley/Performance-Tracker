using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageTrackers : MonoBehaviour
{
    [SerializeField] Employees employees;
    [SerializeField] Button button;
    private PerformanceTracker[] performanceTrackers;
    void Start()
    {
        Load();
        performanceTrackers = employees.GetPerformanceTrackers();
        Display();    


    }
    void Display(){
        GameObject PerformanceTrackersDisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        Vector3 transform = PerformanceTrackersDisplayArea.GetComponent<Transform>().position;
        foreach(PerformanceTracker tracker in performanceTrackers) {
            button.name = tracker.firstName;
            TextMeshProUGUI textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.SetText(tracker.firstName);
            Button newButton = Instantiate<Button>(button, new Vector3(transform.x, transform.y, transform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnPress(tracker));
            newButton.transform.SetParent(PerformanceTrackersDisplayArea.transform);
        }

    }
    void OnPress(PerformanceTracker performanceTracker) {
        Debug.Log("Pressed " + performanceTracker.firstName);
    }
    void Save() {
        string save = JsonUtility.ToJson(employees);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/EmployeeData.json", save);
    }
    void Load() {
        string load = System.IO.File.ReadAllText(Application.persistentDataPath + "/EmployeeData.json");
        employees = JsonUtility.FromJson<Employees>(load);
    }
}
