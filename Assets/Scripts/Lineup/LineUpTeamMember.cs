using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpTeamMember : MonoBehaviour
{
    private string _firstName;
    private string _lastName;
    [SerializeField]
    private GameObject _lastDropArea;
    public Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = gameObject.transform.position;
    }

    public void SetLastDropArea(GameObject dropArea)
    {
        if (dropArea.GetComponent<DropArea>() != null)
        {
            _lastDropArea = dropArea;
            dropArea.GetComponent<DropArea>().AddTeamMember(gameObject);
        }
        else
        {
            Debug.LogError("DropArea is null, please assign a GameObject that contains the DropArea component");
        }
    }

    public GameObject RemoveLastDropArea()
    {
        GameObject lastDropArea = _lastDropArea;
        if (_lastDropArea != null)
        {
            _lastDropArea.GetComponent<DropArea>().RemoveTeamMember(gameObject); //removes self from the last game object
            _lastDropArea = null;
        }
        return lastDropArea;
    }

    public Vector2 GetCurrentPosition()
    {
        return _lastDropArea == null ? transform.position : _lastDropArea.transform.position;
    }
}
