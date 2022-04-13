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

    public IList<Data.ValueRange> getSheetRange(List<string> sheetNameAndRange)
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

    public List<string> GetNames() {
        List<string> names = new();
        List<string> range = new();
        range.Add("MASTER Performance Board!A12:A999");
        range.Add("MASTER Performance Board!I12:I999");
        int i = 0;
        IList<Data.ValueRange> ranges = getSheetRange(range);
        foreach (IList<object> row in ranges[0].Values) {
            //Debug.Log((string)row[0]);
            //Debug.Log(ranges[1].Values[i][0]);
            //Debug.Log((string)ranges[1].Values[i][0] == "Active "); //there is space in active for whatever reason
            if ((string) ranges[1].Values[i][0] == "Active "){//HR Status
                names.Add((string)row[0]);
            } 
            i++;
        }
        return names;
    }

    public void WriteToRange(string sheetNameAndRange, Data.ValueRange writeText) {
        SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(writeText, spreadsheetId, sheetNameAndRange);
        SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum) 1;
        request.ValueInputOption = valueInputOption;
        Data.UpdateValuesResponse response = request.Execute();
        //service.Spreadsheets.Values.Update(writeText, spreadsheetId, sheetNameAndRange).Execute();
    }

    public void AddTabToGoogleSheet(String tabName) {
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

        batchUpdateRequest.Execute();
    }
}