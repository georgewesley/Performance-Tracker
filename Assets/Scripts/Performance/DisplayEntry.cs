using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayEntry : MonoBehaviour
{
    [SerializeField] Button entryButton;
    [SerializeField] GameObject ManageEntriesPanel;
    [SerializeField] GameObject EntryPanel;
    private GameObject DisplayArea;
    private Vector3 DisplayAreaTransform;
    void OnEnable() //if we used start this would happen before trackerManger is even ready to be called
    {
        DisplayArea = GetComponentInChildren<GridLayoutGroup>().gameObject;
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

        GameObject description = new GameObject("Description");
        description.transform.SetPositionAndRotation(new Vector3(DisplayAreaTransform.x, DisplayAreaTransform.y, DisplayAreaTransform.z), Quaternion.identity);
        description.transform.SetParent(DisplayArea.transform);
        TextMeshProUGUI descriptionText = description.AddComponent<TextMeshProUGUI>();
        descriptionText.SetText(FindObjectOfType<ManageEntries>().selectedPerformanceEntry.textDescription);
        descriptionText.fontSize = 72;
        descriptionText.rectTransform.sizeDelta = new Vector2(1500, 0);
    }
    void OnBackPress() {
        ManageEntriesPanel.SetActive(true);
        EntryPanel.SetActive(false);
        for (int i = 0; i < DisplayArea.transform.childCount; i++) { 
            Destroy(DisplayArea.transform.GetChild(i).gameObject);
        }
    }
}
