using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CreateNewEmployee : MonoBehaviour
{
    [SerializeField] GameObject ManageTrackers;
    [SerializeField] GameObject CreateNewEmployeePanel;
    [SerializeField] GameObject[] inputFields;
    private SheetsReader reader;
    private TMP_InputField name;
    private TMP_InputField hireDate;
    private void OnEnable()
    {
        reader = FindObjectOfType<SheetsReader>();
        name = inputFields[0].GetComponent<TMP_InputField>();
        name.text = "";
        hireDate = inputFields[1].GetComponent<TMP_InputField>();
        hireDate.text = "";
        SetButtonPresses();
    }
    private void SetButtonPresses()
    {
        CreateNewEmployeePanel.transform.Find("Generate Employee").GetComponent<Button>().onClick.AddListener(() => GenerateEmployee());
        CreateNewEmployeePanel.transform.Find("Back Button").GetComponent<Button>().onClick.AddListener(() => OnBackPress());
    }
    private void GenerateEmployee()
    {
        reader.AddTabToGoogleSheet(name.text);
    }
    private void OnBackPress()
    {
        CreateNewEmployeePanel.SetActive(false);
        ManageTrackers.SetActive(true);
    }
}
