using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Employees
{
    [SerializeField] List<PerformanceTracker> performanceTrackers = new List<PerformanceTracker>();

    public List<PerformanceTracker> GetPerformanceTrackers() {
        return performanceTrackers;
    }

    public int printStuff() {
        return performanceTrackers.Count;
    }
}
