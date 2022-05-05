using System;
using System.Collections.Generic;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Data = Google.Apis.Sheets.v4.Data;


using UnityEngine;

class SheetsReader : MonoBehaviour
{
    static private string spreadsheetId = "1oB6tyA60kQ_IhZeLthKwdRtkdM-rby1AfCIuBst2UsQ";

    static private string serviceAccountID = "unity-performance-tracker@unity-performance-tracker.iam.gserviceaccount.com";
    static private SheetsService service;

    static SheetsReader()
    {
        //  Loading private key from resources as a TextAsset
        string key = "INSERT KEY HERE";

        // Creating a  ServiceAccountCredential.Initializer
        // ref: https://googleapis.dev/dotnet/Google.Apis.Auth/latest/api/Google.Apis.Auth.OAuth2.ServiceAccountCredential.Initializer.html
        ServiceAccountCredential.Initializer initializer = new ServiceAccountCredential.Initializer(serviceAccountID);

        // Getting ServiceAccountCredential from the private key
        // ref: https://googleapis.dev/dotnet/Google.Apis.Auth/latest/api/Google.Apis.Auth.OAuth2.ServiceAccountCredential.html
        ServiceAccountCredential credential = new ServiceAccountCredential(
            initializer.FromPrivateKey(key)
        );

        service = new SheetsService(
            new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            }
        );
    }

    public IList<Data.ValueRange> getBatchData(List<string> sheetNameAndRange)
    {
        SpreadsheetsResource.ValuesResource.BatchGetRequest request = service.Spreadsheets.Values.BatchGet(spreadsheetId);
        request.Ranges = sheetNameAndRange;
        Data.BatchGetValuesResponse response = request.Execute();
        if (response != null && response.ValueRanges.Count > 0)
        {
            return response.ValueRanges; //[0].Values of this returns the values of first range
        }
        else
        {
            Debug.Log("No data found.");
            return null;
        }
    }

    public List<List<string>> GetNames() {
        List<List<string>> nameList = new();
        List<string> names = new();
        List<string> inactiveNames = new();

        List<string> range = new();
        range.Add("MASTER Performance Board!A3:A999");
        range.Add("MASTER Performance Board!I3:I999");
        int i = 0;
        IList<Data.ValueRange> ranges = getBatchData(range);
        foreach (IList<object> row in ranges[0].Values) {
            //Debug.Log((string)row[0]);
            //Debug.Log(ranges[1].Values[i][0]);
            //Debug.Log((string)ranges[1].Values[i][0] == "Active "); //there is space in active for whatever reason
            //add index out of range exception for when status is left blank below
            try {
                if ((string) ranges[1].Values[i][0] == "Active "){//HR Status
                    names.Add((string)row[0]);
                } 
                else {
                    inactiveNames.Add((string)row[0]);
                }
            catch { //if we have an index out of range error it means that it is blank, so we should include it bc it is not inactive
                names.Add((string)row[0]);
            }
            i++;
        }
        names.Sort();
        nameList.Add(names);
        nameList.Add(inactiveNames);
        return nameList;
    }
    //should probably change this to batchupdate instead, it would take in a list of requests instead of a string name/range
    public void WriteToRange(string sheetNameAndRange, Data.ValueRange writeText, bool UserEntered = false) { //User entered being true will result in the values being parsed, things like formulas will be applied if this is the case
        int ValueInput = 1;
        if(UserEntered) {
            ValueInput = 2;
        }
        SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(writeText, spreadsheetId, sheetNameAndRange);
        SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum) ValueInput;
        request.ValueInputOption = valueInputOption;
        Data.UpdateValuesResponse response = request.Execute();
        //service.Spreadsheets.Values.Update(writeText, spreadsheetId, sheetNameAndRange).Execute();
    }

    public string AddTabToGoogleSheet(String tabName) { //returns sheet id of new tab
        var duplicateSheetRequest = new Data.DuplicateSheetRequest();
        duplicateSheetRequest.SourceSheetId = 318664475;
        duplicateSheetRequest.NewSheetName = tabName;
        Data.BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new Data.BatchUpdateSpreadsheetRequest();
        batchUpdateSpreadsheetRequest.Requests = new List<Data.Request>();
        batchUpdateSpreadsheetRequest.Requests.Add(new Data.Request
        {
            DuplicateSheet = duplicateSheetRequest
        });
        var batchUpdateRequest =
            service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
        return batchUpdateRequest.Execute().Replies[0].DuplicateSheet.Properties.SheetId.ToString();
    }

    public string GetSpreadSheetId() {
        return spreadsheetId;
    }
}