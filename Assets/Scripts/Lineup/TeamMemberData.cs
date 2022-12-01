using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMemberData
{
    public ShiftData shift;
    public string firstName;
    public string lastName;
    public string shiftType;
    //public TrainingData trainingData //this will be for the training data which will be used to determine experience

    public string PrintData()
    {
        return firstName + " " + lastName + " " + shift.PrintData() + " " + shiftType;
    }
}
