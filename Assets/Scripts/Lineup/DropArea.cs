using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    private GameObject _currentTeamMember;
    public Vector2 originalPosition;
    public GameObject originalDropArea;
    //need a reference to the "ultimate parent", if this is a reference to itself we know it is the ultimate parent

    private void Awake()
    {
        originalPosition = gameObject.transform.position;
        _currentTeamMember = null;
        SetPositionName("Test123");
        originalDropArea = gameObject.transform.parent.CompareTag("DropArea") ? gameObject.transform.parent.GetComponent<DropArea>().originalDropArea : gameObject;
        //above line determines whether this is a DropArea with parents or if it is a DropArea that has no DropArea parent
    }

    public void AddTeamMember(GameObject teamMember) //this should never be called from anywhere except LineUpTeamMember
    {
        _currentTeamMember = teamMember;
        teamMember.transform.SetParent(gameObject.transform); //this operation should occur in TeamMemberManager instead or maybe DragController
    }

    public void RemoveTeamMember(GameObject teamMember) //this should never be called from anywhere except LineUpTeamMember
    {
        _currentTeamMember = null;
    }

    public void SetPositionName(string name)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(name);
    }

    public GameObject GetTeamMember()
    {
        return _currentTeamMember;
    }

    private GameObject RecursiveOriginalDropArea(GameObject dropArea) //this does not work
    {
        if (dropArea.transform.parent.CompareTag("DropArea"))
        {
            return RecursiveOriginalDropArea(gameObject.transform.parent.gameObject);
        }
        return gameObject;
    }
}
