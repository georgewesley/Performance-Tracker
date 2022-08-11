using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    private int _numPositions;
    private List<GameObject> _currentTeamMembers;
    public Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = gameObject.transform.position;
        _currentTeamMembers = new List<GameObject>();
        _numPositions = 1; //will always start with one position, may need to change this if saving and loading data
        SetPositionName("Test123");
    }

    public void SetNumPositions(int numPositions)
    {
        ChangeNumberOfAreas(numPositions - _numPositions);
        _numPositions = numPositions;
    }

    public int GetNumPositions()
    {
        return _numPositions;
    }

    public void AddTeamMember(GameObject teamMember) //this should never be called from anywhere except LineUpTeamMember
    {
        _currentTeamMembers.Add(teamMember);
    }

    public void RemoveTeamMember(GameObject teamMember) //this should never be called from anywhere except LineUpTeamMember
    {
        _currentTeamMembers.Remove(teamMember);
    }

    public void SetPositionName(string name)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(name);
    }

    public List<GameObject> GetTeamMembers()
    {
        return _currentTeamMembers;
    }

    private void ChangeNumberOfAreas(int numberToChange) //sometimes this will be negative or positive which is what tells us to add or remove
    {
        
    }
}
