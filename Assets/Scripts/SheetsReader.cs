using System;
using System.Collections.Generic;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;


using UnityEngine;

class SheetsReader : MonoBehaviour
{
    static private string spreadsheetId = "1oB6tyA60kQ_IhZeLthKwdRtkdM-rby1AfCIuBst2UsQ";

    static private string serviceAccountID = "unity-performance-tracker@unity-performance-tracker.iam.gserviceaccount.com";
    static private SheetsService service;

    static SheetsReader()
    {
        //  Loading private key from resources as a TextAsset
        string key = "This is where one would put their key, will not upload it to github for obvious reasons";
        Debug.Log(key);

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
        Debug.Log("Tried to do sheets stuff in static");
    }

    public IList<IList<object>> getSheetRange(string sheetNameAndRange)
    {
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);
        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            return values;
        }
        else
        {
            Debug.Log("No data found.");
            return null;
        }
    }

    public IList<Sheet> GetAllSheets()
    {
        return service.Spreadsheets.Get(spreadsheetId).Execute().Sheets;
    }
}