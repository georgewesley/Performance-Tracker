using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data = Google.Apis.Sheets.v4.Data;
public class CreateNewEmployee : MonoBehaviour
{
    [SerializeField] GameObject ManageTrackers;
    [SerializeField] GameObject CreateNewEmployeePanel;
    [SerializeField] GameObject[] inputFields;
    private SheetsReader writer;
    private TMP_InputField name;
    private TMP_InputField hireDate;
    private void OnEnable()
    {
        writer = FindObjectOfType<SheetsReader>();
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
        string sheetID = writer.AddTabToGoogleSheet(name.text);
        string range = name.text + "!B3:B5";

        IList<IList<object>> finalList = new GoogleList<IList<object>>();
        IList<object> nameValue = new GoogleList<object>();
        IList<object> roleValue = new GoogleList<object>();
        IList<object> hireDateValue = new GoogleList<object>();
        nameValue.Add(name.text);
        roleValue.Add("Team Member"); //Maybe make this a drop down menu with a choice 
        hireDateValue.Add(hireDate.text);
        finalList.Add(nameValue);
        finalList.Add(roleValue);
        finalList.Add(hireDateValue);

        Data.ValueRange body = new Data.ValueRange();
        body.Range = range;
        body.Values = finalList;

        writer.WriteToRange(range, body);

        AddEmployeeToMainPage(sheetID);
    }
    private void OnBackPress()
    {
        CreateNewEmployeePanel.SetActive(false);
        ManageTrackers.SetActive(true);
    }

    private void AddEmployeeToMainPage(string sheetID) {
        string range = "MASTER Performance Board!A";

        IList<IList<object>> finalList = new GoogleList<IList<object>>();
        IList<object> nameValue = new GoogleList<object>(); //Can add a bunch of new lists for email and other options
        nameValue.Add(GenerateHyperLink(sheetID, name.text));
        finalList.Add(nameValue);

        List<List<string>> nameList = writer.GetNames();
        int numNames = nameList[0].Count+nameList[1].Count+3; //Adds the active names, the inactive names, and 3 to get the newest row. 3 because first two rows are not used for names, 1 more because we want the next row
        range += numNames.ToString();

        Data.ValueRange body = new Data.ValueRange();
        body.Range = range;
        body.Values = finalList;

        writer.WriteToRange(range, body, true);
        ManageTrackers.GetComponent<ManageTrackers>().AddNewEmployeeAfterLoad(name.text);
    }

    private string GenerateHyperLink(string sheetID, string label) {
        string hyperLink = "=HYPERLINk(\"https://docs.google.com/spreadsheets/d/";
        hyperLink += writer.GetSpreadSheetId();
        hyperLink += "/edit#gid=";
        hyperLink += sheetID;
        hyperLink += "\", \"" + label + "\")";
        Debug.Log(hyperLink);
        return hyperLink;
    }
}
