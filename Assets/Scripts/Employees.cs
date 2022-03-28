using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Employees
{
    [SerializeField] ArrayList performanceTrackers;

    public ArrayList GetPerformanceTrackers() {
        return performanceTrackers;
    }
}
