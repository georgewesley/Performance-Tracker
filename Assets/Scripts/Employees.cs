using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Employees
{
    [SerializeField] PerformanceTracker[] performanceTrackers;

    public PerformanceTracker[] GetPerformanceTrackers() {
        return performanceTrackers;
    }
}
