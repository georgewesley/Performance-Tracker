using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Data = Google.Apis.Sheets.v4.Data;

public class WriteEntry : MonoBehaviour
{
    [SerializeField] GameObject ManageEntryPanel;
    [SerializeField] GameObject WriteEntryPanel;
    [SerializeField] GameObject[] inputFields;
    private PerformanceEntry performanceEntry;
    void OnEnable()
    {
        performanceEntry = FindObjectOfType<ManageEntries>().selectedPerformanceEntry;
        WriteEntryPanel.transform.Find("Save Button").GetComponent<Button>().onClick.AddListener(() => SaveFields());
        WriteEntryPanel.transform.Find("Back Button").GetComponent<Button>().onClick.AddListener(() => OnBackPress());
        FillFields();
    }

    void FillFields() {
        inputFields[0].GetComponent<TMP_InputField>().text = performanceEntry.entryDate.ToLongDateString();
        inputFields[1].GetComponent<TMP_InputField>().text = performanceEntry.leaderName;
        inputFields[2].GetComponent<TMP_InputField>().text = performanceEntry.category;
        inputFields[3].GetComponent<TMP_InputField>().text = performanceEntry.subcategory;
        inputFields[4].GetComponent<TMP_InputField>().text = performanceEntry.textDescription;
    }

    void SaveFields() {
        Debug.Log(inputFields[4].GetComponent<TMP_InputField>().text);
        SheetsReader writer = FindObjectOfType<SheetsReader>();
        Data.ValueRange body = new Data.ValueRange();
        body.Range = GenerateSheetRange();
        body.Values = GenerateValues(); // this does not happen because values does not have a value
        Debug.Log(body.Values[0][4]);
        writer.WriteToRange(GenerateSheetRange(), body);
    }

    IList<IList<object>> GenerateValues() {
        IList<IList<object>> returnList = new GoogleList<IList<object>>();
        IList<object> textValues = new GoogleList<object>();

        performanceEntry.entryDate = DateTime.Parse(inputFields[0].GetComponent<TMP_InputField>().text);
        performanceEntry.leaderName = inputFields[1].GetComponent<TMP_InputField>().text;
        performanceEntry.category = inputFields[2].GetComponent<TMP_InputField>().text;
        performanceEntry.subcategory = inputFields[3].GetComponent<TMP_InputField>().text;
        performanceEntry.textDescription = inputFields[4].GetComponent<TMP_InputField>().text;

        textValues.Add(performanceEntry.entryDate.ToLongDateString());
        textValues.Add(performanceEntry.leaderName);
        textValues.Add(performanceEntry.category);
        textValues.Add(performanceEntry.subcategory);
        textValues.Add(performanceEntry.textDescription);
        returnList.Add(textValues);
        return returnList;
    }

    string GenerateSheetRange() {
        string sheetName = ManageTrackers.SelectedEmployee.sheetName;
        string row = performanceEntry.row.ToString();
        return sheetName+"!"+"A"+row+":"+"E"+row;
    }
    void OnBackPress() {
        ManageEntryPanel.SetActive(true);
        WriteEntryPanel.SetActive(false);
    }
}

class GoogleList<T> : List<T>, System.Collections.Generic.IList<T> {
}

