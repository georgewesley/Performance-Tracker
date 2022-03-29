using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayEntry : MonoBehaviour
{
    [SerializeField] Button entryButton;
    [SerializeField] GameObject DisplayEntryPanel;
    [SerializeField] GameObject ManageEntriesPanel;
    [SerializeField] GameObject EntryPanel;
    private ManageTrackers trackerManager;
    private List<PerformanceEntry> performanceEntries;
    private GameObject DisplayArea;
    private Vector3 DisplayAreaTransform;
    void OnEnable() //if we used start this would happen before trackerManger is even ready to be called
    {
        trackerManager = FindObjectOfType<ManageTrackers>();
        performanceEntries = trackerManager.SelectedEmployee.performanceEntries;
        DisplayArea = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        DisplayAreaTransform = DisplayArea.GetComponent<Transform>().position;
        Display();
    }

        void Display()
    {
        entryButton.name = "Back";
        TextMeshProUGUI textMeshPro = entryButton.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.SetText("Go Back");
        Button newButton = Instantiate<Button>(entryButton, new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        newButton.onClick.AddListener(() => OnBackPress());
        newButton.transform.SetParent(DisplayArea.transform);
        
        textMeshPro.SetText(FindObjectOfType<ManageEntries>().performanceEntry.textDescription);
    }
    void OnBackPress() {
        ManageEntriesPanel.SetActive(true);
        DisplayEntryPanel.SetActive(false);
        Destroy(DisplayArea.transform.GetChild(0).gameObject);
    }
}
