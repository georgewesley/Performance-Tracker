using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftData
{
    public int[] start = new int[2]; //index 0 is hour, index 2 is minute 
    public int[] end = new int[2];

    public string PrintData()
    {
        return start[0] + ":" + start[1] + " " + end[0] + ":" + end[1];
    }
}
