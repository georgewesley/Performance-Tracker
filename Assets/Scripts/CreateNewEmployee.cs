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
    private TMP_InputField employeeName;
    private TMP_InputField employeeEmail;
    private TMP_InputField employeePhone;
    private TMP_InputField employeeInfo;
    private TMP_InputField employeeType;
    private TMP_InputField employeeHouse;
    private TMP_InputField employeeTeam;
    private TMP_InputField employeeRole;
    private TMP_InputField employeeStatus;
    private TMP_InputField hireDate;
    private void OnEnable()
    {
        writer = FindObjectOfType<SheetsReader>();
        employeeName = inputFields[0].GetComponent<TMP_InputField>();
        employeeName.text = "";
        employeeEmail = inputFields[1].GetComponent<TMP_InputField>();
        employeeEmail.text = "";
        employeePhone = inputFields[2].GetComponent<TMP_InputField>();
        employeePhone.text = "";
        employeeInfo = inputFields[3].GetComponent<TMP_InputField>();
        employeeInfo.text = "";
        employeeType = inputFields[4].GetComponent<TMP_InputField>();
        employeeType.text = "";
        employeeHouse = inputFields[5].GetComponent<TMP_InputField>();
        employeeHouse.text = "";
        employeeTeam = inputFields[6].GetComponent<TMP_InputField>();
        employeeTeam.text = "";
        employeeRole = inputFields[7].GetComponent<TMP_InputField>();
        employeeRole.text = "";
        employeeStatus = inputFields[8].GetComponent<TMP_InputField>();
        employeeStatus.text = "";
        hireDate = inputFields[9].GetComponent<TMP_InputField>();
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
        string sheetID = writer.AddTabToGoogleSheet(employeeName.text);
        string range = employeeName.text + "!B3:B5";

        IList<IList<object>> finalList = new GoogleList<IList<object>>();
        IList<object> nameValue = new GoogleList<object>();
        IList<object> roleValue = new GoogleList<object>();
        IList<object> hireDateValue = new GoogleList<object>();
        nameValue.Add(employeeName.text);
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
        IList<object> columnValues = new GoogleList<object>(); //Probably better to just have a single list with each index as one of these values
        columnValues.Add(GenerateHyperLink(sheetID, employeeName.text));
        columnValues.Add(employeeEmail.text); //email indx 1
        columnValues.Add(employeePhone.text); //phone index 2 
        columnValues.Add(employeeInfo.text); //additionalInfo index 3
        columnValues.Add(employeeType.text); //type index 4, this is part time or full time, better name exists
        columnValues.Add(employeeHouse.text); //house, boh or foh index 5
        columnValues.Add(employeeTeam.text); //team inxex 6
        columnValues.Add(employeeRole.text); //role inex 7
        columnValues.Add("Active "); //status index 8
        finalList.Add(columnValues);

        List<List<string>> nameList = writer.GetNames();
        int numNames = nameList[0].Count+nameList[1].Count+3; //Adds the active names, the inactive names, and 3 to get the newest row. 3 because first two rows are not used for names, 1 more because we want the next row
        range += numNames.ToString();
        range += ":I"; 
        range += numNames.ToString();

        Data.ValueRange body = new Data.ValueRange();
        body.Range = range;
        body.Values = finalList;

        writer.WriteToRange(range, body, true);
        ManageTrackers.GetComponent<ManageTrackers>().AddNewEmployeeAfterLoad(employeeName.text);
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
