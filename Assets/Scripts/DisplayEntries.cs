using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEntries : MonoBehaviour
{
    PerformanceEntry[] entries;

    [SerializeField] PerformanceTracker tracker;
    [SerializeField] Button button;
    void Start()
    {
        entries = tracker.performanceEntries;
        Display();    


    }
    void Display(){
        GameObject EntryDisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        Vector3 transform = EntryDisplayArea.GetComponent<Transform>().position;
        foreach(PerformanceEntry entry in entries) {
            //do stuff with entries
        }

    }
    void OnPress(PerformanceTracker performanceTracker) {
        Debug.Log("Pressed " + performanceTracker.firstName);
    }
}
