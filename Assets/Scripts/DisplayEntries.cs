using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayEntries : MonoBehaviour
{
    ArrayList entries;

    [SerializeField] PerformanceTracker tracker;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI text;
    Vector3 entryDisplayTransform;
    void Start()
    {
        entries = tracker.performanceEntries;
        Display();    


    }
    void Display(){
        GameObject EntryDisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        entryDisplayTransform = EntryDisplayArea.GetComponent<Transform>().position;
        foreach(PerformanceEntry entry in entries) {
            Button newButton = Instantiate<Button>(button, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
            newButton.onClick.AddListener(() => OnPress(entry));
            SetButtonText(newButton, entry);
            newButton.transform.SetParent(EntryDisplayArea.transform);
        }

    }
    void OnPress(PerformanceEntry performanceEntry) {
        Debug.Log("Pressed " + performanceEntry.textDescription);
    }

    void SetButtonText(Button newButton, PerformanceEntry entry) {
        TextMeshProUGUI newText = Instantiate<TextMeshProUGUI>(text, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
        newText.text = "Category: " + entry.category;
        newText.transform.SetParent(newButton.transform);

        TextMeshProUGUI newText1 = Instantiate<TextMeshProUGUI>(text, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
        newText1.text = "Subcategory: " + entry.subcategory;
        newText1.transform.SetParent(newButton.transform);

        TextMeshProUGUI newText2 = Instantiate<TextMeshProUGUI>(text, new Vector3(entryDisplayTransform.x, entryDisplayTransform.y, entryDisplayTransform.z), Quaternion.identity);
        newText2.text = "Leader Name: " + entry.leaderName;
        newText2.transform.SetParent(newButton.transform);
    }
}
