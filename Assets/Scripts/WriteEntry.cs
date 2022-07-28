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
    [SerializeField] private GameObject[] dropDowns;
    private PerformanceEntry _performanceEntry;
    private Transform _createEntryButton;
    private GameObject _lastEntryButton;
    private Transform _newEntryButton;
    private bool _destroy;
    private ManageEntries _manageEntries;
    private SheetsReader _sheetsReader;
    private IList<IList<object>> _subcategories;
    private TMP_Dropdown _leaderDropDown;
    private TMP_Dropdown _categoryDropDown;
    private TMP_Dropdown _subcategoryDropDown;

    void Awake() //We put these in start not in enable because they only happen once
    //note that start happens after enable??? That is why we use Awake() here
    {
        Button saveButton = WriteEntryPanel.transform.Find("Save Button").GetComponent<Button>();
        Button backButton = WriteEntryPanel.transform.Find("Back Button").GetComponent<Button>();
        saveButton.onClick.AddListener(() => SaveFields());
        backButton.onClick.AddListener(() => OnBackPress());
        _sheetsReader = FindObjectOfType<SheetsReader>();
        
        _leaderDropDown = dropDowns[0].GetComponent<TMP_Dropdown>();
        _categoryDropDown = dropDowns[1].GetComponent<TMP_Dropdown>();
        _subcategoryDropDown = dropDowns[2].GetComponent<TMP_Dropdown>();
    }
    void OnEnable()
    {
        _destroy = false; //need to set this baseline

        _manageEntries = ManageEntryPanel.GetComponent<ManageEntries>();
        _performanceEntry = _manageEntries.selectedPerformanceEntry;
        
        _createEntryButton = ManageEntryPanel.GetComponentInChildren<VerticalLayoutGroup>().transform;
        int childCount = _createEntryButton.childCount;
        _lastEntryButton = _manageEntries.selectedGameObject;
        _createEntryButton = _createEntryButton.GetChild(childCount - 1); //gets the button that creates the entry (always the last button)
        FillFields();
    }

    void UpdateSubcategoryDropDown()
    {
        Debug.Log("We changed the sub");
        _subcategoryDropDown.ClearOptions();
        _subcategoryDropDown.AddOptions(GetSubCategories(_categoryDropDown.options[_categoryDropDown.value].text));
        _subcategoryDropDown.value = 0;
    }

    private void FillFields()
    {
        List<string> range = new List<string>();
        range.Add("*DO NOT USE* - LEGEND!B2:AS2"); //leaders
        range.Add("*DO NOT USE* - LEGEND!B1:AS1"); //categories, subcategories will need to update based on when category is changed
        range.Add("*DO NOT USE* - LEGEND!A4:AS40"); //Should read all of the subcategories and store them to reference locally so we do not need to do a batch update everytime we update categories
        IList<Data.ValueRange> ranges = _sheetsReader.getBatchData(range);
        IList<object> leaders = ranges[0].Values[0];
        IList<object> categories = ranges[1].Values[0];
        _subcategories = ranges[2].Values; //initialization 
        inputFields[0].GetComponent<TMP_InputField>().text = _performanceEntry.entryDate.ToLongDateString();
        inputFields[1].GetComponent<TMP_InputField>().text = _performanceEntry.textDescription;
        
        SetDropDownOptions(leaders, categories);
    }

    private void SetDropDownOptions(IList<object> leaders, IList<object> categories)
    {
        _leaderDropDown.ClearOptions();
        _categoryDropDown.ClearOptions();
        _subcategoryDropDown.ClearOptions();
        
        foreach (string name in leaders)
        {
            List<string> options = new();
            options.Add(name);
            _leaderDropDown.AddOptions(options);
        }
        foreach (string category in categories)
        {
            List<string> options = new();
            options.Add(category);
            _categoryDropDown.AddOptions(options);
        }
        List<string> subcategoryOptions = new List<string>(GetSubCategories(_performanceEntry.category));
        _subcategoryDropDown.AddOptions(subcategoryOptions);
        
        _leaderDropDown.value = leaders.IndexOf(_performanceEntry.leaderName);
        _categoryDropDown.value = categories.IndexOf(_performanceEntry.category);
        _subcategoryDropDown.GetComponent<TMP_Dropdown>().value = subcategoryOptions.IndexOf(_performanceEntry.subcategory);
        _categoryDropDown.onValueChanged.AddListener(delegate {UpdateSubcategoryDropDown();});
    }

    private List<string> GetSubCategories(string category)
    {
        int matchIndex = 0;
        List<string> matchedSubcategories = new();
        bool first = true; //used to determine if we are looking at the first string in the list of subcategories which will always be the name of the category followed by " Sub"
        for (int i = 0; i < _subcategories.Count; i++)
        {
            if (_subcategories[i][0].ToString().Split(" Sub")[0] == category) //we have to remove the " Sub" to make category match, alternatively we could add " Sub" to the end of category. This has nothing to do with what is in this code, but rather how the filter function on google sheets works. 
            {
                foreach (string matchedSubCategory in _subcategories[i]) //we have to do this so that we have a list of strings instead of a list of objects
                {
                    Debug.Log("We are doing a thing");
                    Debug.Log(matchedSubCategory);
                    if (first) //probably a better way to skip first item in list
                    {
                        first = false;
                    }
                    else
                    {
                        matchedSubcategories.Add(matchedSubCategory);
                        Debug.Log(matchedSubCategory);
                    }
                }
                break;
            }
                //return subcategories[subcategories.IndexOf(category)];
        }
        return matchedSubcategories;
    }

    void SaveFields() {
        Data.ValueRange body = new Data.ValueRange();
        body.Range = GenerateSheetRange();
        body.Values = GenerateValues(); // this does not happen because values does not have a value
        //Debug.Log(body.Values[0][4]);
        _sheetsReader.WriteToRange(GenerateSheetRange(), body);
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
        _performanceEntry.textDescription = inputFields[1].GetComponent<TMP_InputField>().text;
        var dropDown = dropDowns[0].GetComponent<TMP_Dropdown>();
        _performanceEntry.leaderName = dropDown.options[dropDown.value].text;
        dropDown = dropDowns[1].GetComponent<TMP_Dropdown>();
        _performanceEntry.category = dropDown.options[dropDown.value].text;
        dropDown = dropDowns[2].GetComponent<TMP_Dropdown>();
        _performanceEntry.subcategory = dropDown.options[dropDown.value].text;
        //add back category, subcategory, and leader but getting from dropDowns

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

