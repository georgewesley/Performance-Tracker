using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Data = Google.Apis.Sheets.v4.Data;


using UnityEngine;

class SheetsReader : MonoBehaviour
{
    static private string spreadsheetId = "1RXtmd8zTgnOkkWyBJXgkWIwqT1E9ej8DVJGgcSZwkb8";

    static private string serviceAccountID = "performance-tracker@hale-photon-354522.iam.gserviceaccount.com";
    static private SheetsService service;

    static SheetsReader()
    {
        string key = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC/WYOu560n6e9D\nSWDjJQ5iQWLPo1VduicUlpvipsD7jLw6YaXShjPiWRFZ4RgjF9vOL8aspbvknN7S\n6YVdZJuf9mGs0wHp8kNjBr1lt5DsVJ05/h2CaQW84ZN3/Y8XSBHjgyf6CQgwkC3J\nItxv1WFOU1Ol6oWPEzt47Np/qCbQHX8UTG3UAsjnxc+IaGdaKbhH3/gm+ehiWOWk\nXeVWanNtsi/zq9/SFhmCwN5JKsRAbgnfyyUkWBTa0IWgSbzdvhjuViZcMNwWvVPv\nh1+9OaEW5LlZxRzD0kpJqwXLlTxQ1/syC985ng41nXDKkmJpX/uShZEkMeJDHIXe\n28HGW3FnAgMBAAECggEAH12WTvlS/OlFJbZwLKD1C/jD2wgJr49wEIPk5vDb+V/x\ntXAu36TNTxu30w/UBmHxed7YUOc/Nr4wGEqHE0QGmTX91M0nJyi7EGRub1n5ENmI\nhuph/pp7LPbyVrI4M7dwlzv3NZkoFS3NE4dPXarl2bz28oolfR/ImTiEqHWbxx6G\nBhUjzdFKmuxCVD+56s+H+pQigP4l5NTuv27WJYUCVi8WCbDJTJLbTNVTgQfZAiPE\nryLo3qdx4g5hwbGomzUV+uaaLC1nZh3hNhUmTmgCMknpn6P3r/BwIoDuTkjUuiTW\nQ16mL7JzMOXvRPoiWuM3D6E4QQNtHZz1mF4YgWKXAQKBgQDnUhSYNMMmztDzhPhH\n2wg9uRC9oEAjUeEHo8egqrNyJ+ZloWiH99GIedK276+lRadlmVLytV9t6NhjyEGV\n8+S4pUT5q/rXzHjYNvA+Z5D+cTYvK8u4hgH0TEUVD6w7LjRtVD5wJ4fef8IP3vZu\nzv7DaqPqSB43aWkN8qD5J6voRwKBgQDTw7s8qC7AV0KEHZIJk25udyKsDuxxhidl\nCAw84LObzpRO7QdQq8+khP3HPeKXQkejie9AqZ4uCue90ZiCHWi9/HJwlfnhIM3j\nble7iuNOjR5CBDevEg794AOU7U6TkpJi/7cHNdk9Qu/OFrFJnyPhsg7mcvIQT3/7\ndzb7V8nd4QKBgQCy9W91aaxpA6voGkEy5iN0DQ8EhUvZh2j+zhiNFkMJ2BCJI9yJ\nBJKYcRcx7DEJPeAJ3BquJt+TWoa+e5kx96RFraa1OfYwqcH8FFS9Esa78r4mtE1B\njntIkxEHAD8Q8eghhQFhJ1QYMOLkAGzKwV2btY7mm7C00doyrjkXSTfX1QKBgANW\nfXgKSppbhb/hW3DtMvtow1Ik9hMgAzTzeIXpIMue6PWJhOj/nElCk2F1l0G9GLX6\nMZw6UDT3lQmH6Th70C/Wb9NYedTTmIsyLQ3WtZiCXuy5dks7JKNZyZSqXOe0krwe\nvbrOXXs6t97uuqKncIBZNyTowOoC5siG64Xwr0zhAoGBAIA1sMqfFkNBqfUtWEb6\nOXInFmRqx7i0KVob8SKZoiw8In+Xk5MkzteY3qwZ0p1WVk2wXs9SydOjdaua/vls\naJoaTyZcXs7HJA4uo7zuj/bF6iaXtO1PDK9nM8r5NZa/Pcpipl3FgyHp8l4So3ya\ngZOXWApMB2TIHkFKKUTadNjM\n-----END PRIVATE KEY-----\n";

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
        Debug.Log("No data found.");
        return null;
    }

    public List<List<string>> GetNames() { //maybe this should be in manage trackers, although I can think of many different places we may need to get the names from the main page
        List<List<string>> nameList = new();
        List<string> names = new();
        List<string> inactiveNames = new();

        List<string> range = new();
        range.Add("MASTER Performance Board!A3:A999");
        range.Add("MASTER Performance Board!J3:J999"); //this should match the range with the ACTIVE or INACTIVE
        int i = 0;
        IList<Data.ValueRange> ranges = getBatchData(range);
        foreach (IList<object> row in ranges[0].Values) {
            //Debug.Log((string)row[0]);
            //Debug.Log(ranges[1].Values[i][0]);
            //Debug.Log((string)ranges[1].Values[i][0] == "Active "); //there is space in active for whatever reason
            //add index out of range exception for when status is left blank below
            try {
                if ((string) ranges[1].Values[i][0] == "ACTIVE"){//HR Status
                    names.Add((string)row[0]);
                } 
                else {
                    inactiveNames.Add((string)row[0]);
                }
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
        duplicateSheetRequest.SourceSheetId = 362286870; //from the template
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