using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TeamMemberManager : MonoBehaviour
{
    private List<TeamMemberData> _teamMemberData = new();
    [SerializeField] private float spaceBetweenNewTeamMembers;
    [SerializeField] private GameObject employeePrefab;
    [FormerlySerializedAs("viewPort")] [SerializeField] private GameObject teamMemberViewPort;
    [SerializeField] private GameObject dropArea;
    [SerializeField] private GameObject dropAreaPanel;
    private XmlDocument _document = new XmlDocument();
    private List<string> _categories = new();
    private string[] _ignore;
    private bool _removedTeamMembers = false;
    
    void Start()
    {
        _ignore = new[] {"", "Total Day", " "};
        _document.Load(@"Assets/report.xml");
        GenerateNewTeamMembers();
        GenerateDropAreas();
    }

    private void Update()
    {
        if (_removedTeamMembers) return;
        RemoveTeamMembersFromGrid(); //this is super weird but if I put this in start after GenerateNewTeamMembers it does not work correctly
        _removedTeamMembers = true;
    }

    private void GenerateNewTeamMembers() //ideally this should be the only method that I need to edit if I change how data is read
    {
        foreach (XmlNode node in _document.DocumentElement.ChildNodes)
        {
            //Debug.Log(node.InnerText.Trim(););
            foreach (XmlElement element in node)
            {
                //Debug.Log("element: " + element.InnerText.Trim());
                if (!element.InnerText.Trim().StartsWith("Total Day"))
                {
                    break;
                }

                foreach (XmlNode test in element)
                {
                    string inner = test.InnerText.Trim();

                    if (inner.Length >= 12)
                        //6:00am6:00pm is 12 characters, this is the minimum length if we have a time
                        //(assuming name and position are blank, which should not be possible)
                        //Also want to make sure end contains am or pm and that the digit right before that is an int
                    {
                        if ((inner.Substring(inner.Length - 2, 2) == "pm" ||
                             inner.Substring(inner.Length - 2, 2) == "am") &&
                            int.TryParse(inner[^3].ToString(), out int number))
                        {
                            //Debug.Log(inner);
                            var teamMember = ParseShift(inner);
                            _teamMemberData.Add(teamMember);
                            GameObject teamMemberObject = Instantiate<GameObject>(employeePrefab, teamMemberViewPort.transform, true);
                            teamMemberObject.GetComponentInChildren<TextMeshProUGUI>().SetText
                                (teamMember.firstName[0].ToString() + teamMember.lastName[0]); //initials
                            teamMemberObject.transform.SetParent(teamMemberViewPort.transform);
                           
                        }
                        else
                        {
                            //anything that is not a time may be a category (like FOH, Leadership, etc)
                            if (Array.IndexOf(_ignore, inner) == -1)
                            {
                                _categories.Add(inner);
                                Debug.Log("Note a time: " + inner);
                            }
                        }
                    }
                    else
                    {
                        if (Array.IndexOf(_ignore, inner) == -1)
                        {
                            _categories.Add(inner);
                            Debug.Log("Not a time: " + inner);
                        }

                    }
                    //ParseShift(test.InnerText.Trim());
                    //Starting from back find first word with capital letter, if there is a space preceding, find next word
                    //continue till no space preceding. That is the shift name. Everything after that point is the name
                }
            }
        }

        foreach (var data in _teamMemberData)
        {
            Debug.Log("Employee Data: " + data.PrintData());
            
        }
    }

    private void RemoveTeamMembersFromGrid()
    {
        teamMemberViewPort.GetComponent<GridLayoutGroup>().enabled = false; //will need to enable if we go back
    }

    private void GenerateDropAreas() //move to its own class, only for testing here
    {
        Instantiate(dropArea, dropAreaPanel.transform);
        Instantiate(dropArea, dropAreaPanel.transform);
        Instantiate(dropArea, dropAreaPanel.transform);
    }
    private static TeamMemberData ParseShift(string shiftText) //This will also need to change if we change how we are getting data
        //returns a shift with [0] being FirstName or preferred name if one exists, [1] being LastName, [2] being start hour, [3] being start minute, [4] being end hour, [5] being end minute
    {
        string[] parsedShift = new string[7];
        bool timeEndPart = true; //determines if we are editing the end times
        bool timeStartPart = true;
        bool minuteSelected = true;
        int timeAdjustment = 12;
        int currentIndex = shiftText.Length - 2;
        while (currentIndex >= 2) //there is almost certainly a way to do this with regex but it would be pretty
                                  //complicated, I will redo it later. I already use regex for separating names below
        {
            var currentShiftText = shiftText.Substring(currentIndex, 2);
            Debug.Log("shift text: " + currentShiftText);
            if (currentShiftText == "pm")
            {
                timeAdjustment = 12;
            }
            else if (currentShiftText == "am")
            {
                timeAdjustment = 0;
            }
            else if (minuteSelected)
            {
                Debug.Log("Minutes? " + currentShiftText);
                minuteSelected = false;
                if (timeEndPart)
                {
                    parsedShift[5] = currentShiftText; //don't need adjustment for minutes, only for hour
                }
                else //this means we are at start minute
                {
                    parsedShift[3] = currentShiftText;
                }
            }
            else if (timeEndPart||timeStartPart) //maybe change to else
            {
                var potentialTime = shiftText.Substring(currentIndex - 1, 2);
                //Debug.Log("Potential Time: " + potentialTime);
                string time;
                if (int.TryParse(potentialTime, out int num))
                {
                    currentIndex -= 1; //need to move our selection because we are ignoring the :
                    num += timeAdjustment;
                    time = num.ToString();
                    //Debug.Log("We did the parse " + time);
                }
                else //this means the end hour is single digit 
                {
                    //Debug.Log("Potential Time: " + potentialTime[1]);
                    //Debug.Log("Time Adjustment: " + timeAdjustment);
                    int adjusted = int.Parse(potentialTime[1].ToString()) + timeAdjustment;
                    time = adjusted.ToString(); //if this happens, the first index will be a letter not a number
                    //Debug.Log("We did not parse " + time);
                }
                //Debug.Log(potentialTime);
                if (timeEndPart)
                {
                    //Debug.Log("We set minutes to true: " + time);
                    parsedShift[4] = time;
                    timeEndPart = false;
                    minuteSelected = true;
                }
                else
                {
                    //Debug.Log("We set timesStartPart to false " + time);
                    parsedShift[2] = time;
                    var nameAndShift = shiftText[..currentIndex];
                    var separatedNameAndShift = SeparateNamesAndShift(nameAndShift);
                    char[] trimChars = {' ', '-'};
                    parsedShift[0] = separatedNameAndShift[0].Trim(trimChars);
                    parsedShift[1] = separatedNameAndShift[1].Trim(trimChars);
                    parsedShift[6] = separatedNameAndShift[2].Trim(trimChars);
                    //Debug.Log("Should be first/preferred name: " + separatedNameAndShift[0]);
                    //Debug.Log("Should be last name: " + separatedNameAndShift[1]);
                    //Debug.Log("Should be shift: " + separatedNameAndShift[2]);
                    //Debug.Log(nameAndShift);
                    break;
                }
            }
            currentIndex -= 2; 
        }
        return CreateTeamMember(parsedShift[0], parsedShift[1], CreateNewShift(
            int.Parse(parsedShift[2]), int.Parse(parsedShift[3]), 
            int.Parse(parsedShift[4]), int.Parse(parsedShift[5])), parsedShift[6]);
    }

    private static string[] SeparateNamesAndShift(string mixedValues)
    {
        var combinedSeperatedValues = new string[3]; 
        var seperatedValues = 
            Regex.Matches(mixedValues, @"([\-\sA-Z]+[a-z]+)").Select(m => m.Value);
        //[A-Z]+ selects one or more capital letters followed by [a-z\s]+ which is one or more lowercase letters or spaces
        var seperatedValuesList = seperatedValues as List<string> ?? seperatedValues.ToList();
        //Debug.Log("Testing Enumerable: " + enumerable[^1]);
        //Debug.Log("Testing Enumerable: " + enumerable[^1][0]);
        //Debug.Log("Something so I can search: " + string.Join(" ", enumerable));
        combinedSeperatedValues[0] = seperatedValuesList[0]; //adds first name, will rewrite with preferred name later if it exists
        seperatedValuesList.RemoveAt(0); //remove firstname
        seperatedValuesList.Reverse(); //we need the shifts to show up first
        var selectedShift = true;
        var selectedPotentialPreferredName = true;
        var shiftType = "";
        var lastName = "";
        foreach (var value in seperatedValuesList)
        {
            if (selectedShift)
            {
                shiftType = value + shiftType; //adding so that the value is always at the start since we reversed list
                if (!value[0].ToString().Equals(" ")) //opposite of this means that we still have more of the shift to
                                                      //get (there are multiple words in it, such as "Shift Leader"
                                                      //or "BOH General"
                {
                    selectedShift = false;
                }
            }
            else //should only happen after we have confirmed we got all of the shiftType
            {
                var newValue = value;
                if (selectedPotentialPreferredName) //only true for the first word after selecting shiftType
                {
                    try
                    {
                        if(value[^4..].Equals("null"))
                        {
                            newValue = value[..^4];
                            //Debug.Log("Old Value: " + value + ". This is our new value: " + newValue);
                        }
                    }
                    catch
                    {
                        Debug.Log("Length was too long while checking for null in lastnames");
                    }
                    selectedPotentialPreferredName = false;
                    //Debug.Log(value);
                    //Debug.Log("Should be equal to line above if it is only last name: " + seperatedValuesList[^1]);
                    if (newValue[0].ToString() != " " && newValue[0].ToString() != "-" && 
                        !value.Equals(seperatedValuesList[^1])) //the reason we use value here instead of newValue is
                        //because in the seperatedValuesList we never changed the null. If we used newValue it would
                        //always register as false because we deleted null on one but not hte other
                        
                        //because we reversed list want to check if we have a preferred name. We know it is if we are
                        //not in the last element and there is not a space as the first value. This is a pattern in
                        //the data, last names will have a space at the beginning if there is more than one of them
                        //and if there is no space we can check if there are more elements, if there are, then we
                        //know that we have the preferred name. Also want to make sure no - because that is a hyphenated 
                        //last name
                    {
                        combinedSeperatedValues[0] = newValue;
                        continue; //breaks current iteration and does not execute next lines
                    }
                    //if above statement is not true we want to add it to last names, hence we do not run continue;
                }
                lastName = newValue + lastName;
            }
        }
        combinedSeperatedValues[1] = lastName;
        combinedSeperatedValues[2] = shiftType;
        return combinedSeperatedValues;
    }
    private static TeamMemberData CreateTeamMember(string firstName, string lastName, ShiftData shiftData, string shiftType)
    {
        return new TeamMemberData()
        {
            shift = shiftData,
            firstName = firstName,
            lastName = lastName,
            shiftType = shiftType
        };
    }

    private static ShiftData CreateNewShift(int startHour, int startMinute, int endHour, int endMinute)
    {
        return new ShiftData()
        {
            start = new int[] {startHour, startMinute},
            end = new int[] {endHour, endMinute}
        };
    }
}
