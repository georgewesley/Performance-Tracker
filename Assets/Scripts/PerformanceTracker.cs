using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PerformanceTracker
{
    public string firstName;
    public string lastName;// {get; set;}
    public DateTime hireDate; //{get; set;}
    public ArrayList performanceEntries;//{get; set;}

    public void CreatePerformanceEntry(DateTime date, string text, string category, string subcategory, string leader)
    {
        PerformanceEntry performance = new PerformanceEntry
        {
            entryDate = date,
            textDescription = text,
            category = category,
            subcategory = subcategory,
            leaderName = leader
        };
        performanceEntries.Add(performance);
    }
    
}


