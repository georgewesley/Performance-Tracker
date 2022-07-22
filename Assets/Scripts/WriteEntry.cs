using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Unity.VisualScripting;
using Data = Google.Apis.Sheets.v4.Data;

public class WriteEntry : MonoBehaviour
{
    [SerializeField] GameObject ManageEntryPanel;
    [SerializeField] GameObject WriteEntryPanel;
    [SerializeField] GameObject[] inputFields;
    private PerformanceEntry _performanceEntry;
    private Transform _createEntryButton;
    private GameObject _lastEntryButton;
    private Transform _newEntryButton;
    private bool _destroy;
    private ManageEntries _manageEntries;

    void Start() //We put these in start not in enable because they only happen once
    {
        Button saveButton = WriteEntryPanel.transform.Find("Save Button").GetComponent<Button>();
        Button backButton = WriteEntryPanel.transform.Find("Back Button").GetComponent<Button>();
        saveButton.onClick.AddListener(() => SaveFields());
        backButton.onClick.AddListener(() => OnBackPress());
    }
    void OnEnable()
    {
        _destroy = false; //need to set this baseline
        
        _manageEntries = FindObjectOfType<ManageEntries>();
        _performanceEntry = _manageEntries.selectedPerformanceEntry;
        
        _createEntryButton = ManageEntryPanel.GetComponentInChildren<VerticalLayoutGroup>().transform;
        int childCount = _createEntryButton.childCount;
        _lastEntryButton = _manageEntries.selectedGameObject;
        _createEntryButton = _createEntryButton.GetChild(childCount - 1); //gets the button that creates the entry (always the last button)
        FillFields();
    }

    void FillFields() {
        inputFields[0].GetComponent<TMP_InputField>().text = _performanceEntry.entryDate.ToLongDateString();
        inputFields[1].GetComponent<TMP_InputField>().text = _performanceEntry.leaderName;
        inputFields[2].GetComponent<TMP_InputField>().text = _performanceEntry.category;
        inputFields[3].GetComponent<TMP_InputField>().text = _performanceEntry.subcategory;
        inputFields[4].GetComponent<TMP_InputField>().text = _performanceEntry.textDescription;
    }

    void SaveFields() {
        //Debug.Log(inputFields[4].GetComponent<TMP_InputField>().text);
        SheetsReader writer = FindObjectOfType<SheetsReader>();
        Data.ValueRange body = new Data.ValueRange();
        body.Range = GenerateSheetRange();
        body.Values = GenerateValues(); // this does not happen because values does not have a value
        //Debug.Log(body.Values[0][4]);
        writer.WriteToRange(GenerateSheetRange(), body);
        Debug.Log(!_manageEntries.isNewEntry);
        if (!_manageEntries.isNewEntry)
        {
            //if it is not a new entry, we need to destroy and replace the object because we are editing it
            Debug.Log("DESTROY");
            
            _destroy = true;
        }
        else
        { 
            _resetCreateEntriesPerformanceTracker();
        }
        _newEntryButton = _manageEntries.CreateEntryButton(_performanceEntry);
        _createEntryButton.transform.SetAsLastSibling();
    }

    IList<IList<object>> GenerateValues() {
        IList<IList<object>> returnList = new GoogleList<IList<object>>();
        IList<object> textValues = new GoogleList<object>();

        _performanceEntry.entryDate = DateTime.Parse(inputFields[0].GetComponent<TMP_InputField>().text);
        _performanceEntry.leaderName = inputFields[1].GetComponent<TMP_InputField>().text;
        _performanceEntry.category = inputFields[2].GetComponent<TMP_InputField>().text;
        _performanceEntry.subcategory = inputFields[3].GetComponent<TMP_InputField>().text;
        _performanceEntry.textDescription = inputFields[4].GetComponent<TMP_InputField>().text;

        textValues.Add(_performanceEntry.entryDate.ToLongDateString());
        textValues.Add(_performanceEntry.leaderName);
        textValues.Add(_performanceEntry.category);
        textValues.Add(_performanceEntry.subcategory);
        textValues.Add(_performanceEntry.textDescription);
        returnList.Add(textValues);
        return returnList;
    }
    string GenerateSheetRange() {
        string sheetName = ManageTrackers.SelectedEmployee.sheetName;
        string row = _performanceEntry.row.ToString();
        return sheetName+"!"+"A"+row+":"+"E"+row;
    }
    void OnBackPress() {
        if (_destroy)
        {
            int index = _lastEntryButton.transform.GetSiblingIndex();
            Destroy(_lastEntryButton);
            _newEntryButton.SetSiblingIndex(index); // we destroy the object then replace it with this new one
            //there might be a better way? Maybe instead of destroying and replacing we remove all of the triggers and add new ones?
        }
        ManageEntryPanel.SetActive(true);
        WriteEntryPanel.SetActive(false);
    }

    private void _resetCreateEntriesPerformanceTracker() //We need to reset the performance tracker with updated row information when we create a new entry
    {
        Destroy(_createEntryButton.gameObject);
        _createEntryButton = _manageEntries.CreateNewEntryButton(_performanceEntry.row + 1).transform; //+1 for new row
    }
}

class GoogleList<T> : List<T>, System.Collections.Generic.IList<T> { //I don't quite know why this is needed, weird API stuff
}

