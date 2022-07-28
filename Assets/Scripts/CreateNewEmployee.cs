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
    [SerializeField] GameObject[] dropDowns;
    private SheetsReader writer;
    private TMP_InputField employeeName;
    private TMP_InputField employeeEmail;
    private TMP_InputField employeePhone;
    private TMP_InputField employeeRole;
    private TMP_InputField hireDate;
    private TMP_Dropdown I9;
    private TMP_Dropdown workPermit;
    private TMP_Dropdown isMinor;
    private TMP_Dropdown employeeType;
    private TMP_Dropdown house;
    private TMP_Dropdown employeeStatus;
    private void OnEnable() //we want to reset this screen to its default state
    {
        writer = new SheetsReader();
        employeeName = inputFields[0].GetComponent<TMP_InputField>(); 
        employeeName.text = "";
        employeeEmail = inputFields[1].GetComponent<TMP_InputField>();
        employeeEmail.text = "";
        employeePhone = inputFields[2].GetComponent<TMP_InputField>();
        employeePhone.text = "";
        employeeRole = inputFields[3].GetComponent<TMP_InputField>();
        employeeRole.text = "";
        hireDate = inputFields[4].GetComponent<TMP_InputField>();
        hireDate.text = "";
        I9 = dropDowns[0].GetComponent<TMP_Dropdown>();
        I9.value = 0; //this should change us to the first option 
        workPermit = dropDowns[1].GetComponent<TMP_Dropdown>();
        workPermit.value = 0;
        isMinor = dropDowns[2].GetComponent<TMP_Dropdown>();
        isMinor.value = 0;
        employeeType = dropDowns[3].GetComponent<TMP_Dropdown>();
        employeeType.value = 0;
        house = dropDowns[4].GetComponent<TMP_Dropdown>();
        house.value = 0;
        employeeStatus = dropDowns[5].GetComponent<TMP_Dropdown>();
        employeeStatus.value = 0;
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
        roleValue.Add(employeeRole.text); //Maybe make this a drop down menu with a choice 
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
        columnValues.Add(employeeEmail.text); 
        columnValues.Add(employeePhone.text); 
        columnValues.Add(I9.options[I9.value].text); //the reason this works is because I9.value is the currently selected index and we are getting the value of that index, very smart, taken off unity forums
        columnValues.Add(workPermit.options[workPermit.value].text); 
        columnValues.Add(isMinor.options[isMinor.value].text); 
        columnValues.Add(employeeType.options[employeeType.value].text); 
        columnValues.Add(house.options[house.value].text); 
        columnValues.Add(employeeRole.text); 
        columnValues.Add(employeeStatus.options[employeeStatus.value].text); 
        //hiredate is not included here because it is only visible on the employee's file
        finalList.Add(columnValues);

        List<List<string>> nameList = writer.GetNames();
        int numNames = nameList[0].Count+nameList[1].Count+3; //Adds the active names, the inactive names, and 3 to get the newest row. 3 because first two rows are not used for names, 1 more because we want the next row
        range += numNames.ToString();
        range += ":J"; //last column available 
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
