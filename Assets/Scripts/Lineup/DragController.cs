using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class DragController : MonoBehaviour //need a feature to store templates of where objects are
{
    private bool _dragActive = false;
    private Vector2 _screenPosition;
    private GameObject _lastDragged;
    private GameObject _lastDropArea; //used only for swapping 
    private PointerEventData _pointerEventData;
    private GraphicRaycaster _raycaster;
    private bool _edit;
    private bool _potentialsDisplayed;
    private List<GameObject> _potentialDropAreaList;
    [SerializeField] private GameObject potentialDropArea;
    
    void Awake()
    {
        _raycaster = gameObject.GetComponent<GraphicRaycaster>();
        _potentialDropAreaList = new List<GameObject>();
        _potentialsDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).tapCount == 1)
            {
                _screenPosition = Input.GetTouch(0).position;
                _edit = false;
            }
            else
            {
                _edit = true;
                //maybe do a separate edit function 
            }
        }
        else
        {
            _dragActive = false; //if we do not detect any touches we should stop all drags
            return;
        }
        
        if (_dragActive && !_edit)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Drop();
            }
            else
            {
                Drag();
            }
        }
        else
        {
            List<RaycastResult> results = new List<RaycastResult>(); 
            PointerEventData ped = new PointerEventData(null);
            //Set required parameters, in this case, mouse position when we clicked 
            ped.position = _screenPosition;
            _raycaster.Raycast(ped,results);
            GameObject draggable = results[0].gameObject; //We only want the object that is on the top of the others
            Debug.Log(draggable.tag);
            Debug.Log(draggable.gameObject.name);
            if (draggable != null && draggable.CompareTag("draggable") || draggable.CompareTag("DropArea"))  //we check this to make sure we are actually supposed to be able to drag the object we have
            {
                DestroyPotentials(null);
                if (!_edit) //&& !draggable.transform.parent.CompareTag("DropArea"))
                {
                    _lastDragged = draggable;
                    NewDrag();
                }
                else
                {
                    if (draggable.GetComponent<DropArea>() != null)
                    {
                        if (!_potentialsDisplayed)
                        {
                            _potentialsDisplayed = true;
                            _potentialDropAreaList.Add(Instantiate(potentialDropArea, _lastDragged.transform, false)); 
                            _potentialDropAreaList[0].transform.localPosition = new Vector2(-170, 0);
                            _potentialDropAreaList.Add(Instantiate(potentialDropArea, _lastDragged.transform, false)); 
                            _potentialDropAreaList[1].transform.localPosition = new Vector2(170, 0);
                            _potentialDropAreaList.Add(Instantiate(potentialDropArea, _lastDragged.transform, false)); 
                            _potentialDropAreaList[2].transform.localPosition = new Vector2(0, -170);
                            _potentialDropAreaList.Add(Instantiate(potentialDropArea, _lastDragged.transform, false)); 
                            _potentialDropAreaList[3].transform.localPosition = new Vector2(0, 170);
                        }
                        //should pop up four PotentialDropArea buttons that we will then press to create an actual DropArea
                    }
                    else
                    {
                        //should pop up an option to remove the team member, maybe a button that is shaped like a red x that appears in corner, double tap to cancel 
                    }
                }
            }
            else if (_edit)
            {
                DestroyPotentials(null);
                //here we did not click on anything so we want to give option of creating new team member or position. Clicking else where will close this window
                //have prefab that spawns where mouse location is, it will destroy itself if it detects and presses anywhere besides on its dropdown menu
            }
            else if (draggable.CompareTag("PotentialDropArea"))
            {
                DestroyPotentials(draggable.gameObject);
            }
        }
    }

    void NewDrag()
    {
        Debug.Log("New Drag");
        _dragActive = true;
        _lastDragged.transform.SetAsLastSibling();// We do this so it is always on top.
        if(_lastDragged.GetComponent<LineUpTeamMember>() != null)
        {
            LineUpTeamMember lineUpTeamMember = _lastDragged.GetComponent<LineUpTeamMember>();
            lineUpTeamMember.originalPosition = lineUpTeamMember.GetCurrentPosition();
            _lastDropArea = lineUpTeamMember.RemoveLastDropArea();
        }
        else //this is if _lastDragged is a DropArea, we want to make sure that all of the teamMembers stay on top of the position 
        {
            foreach (GameObject teamMember in _lastDragged.GetComponent<DropArea>().GetTeamMembers())
            {
                teamMember.transform.SetAsLastSibling();
            }
        }
    }
    void Drag()
    {
        Debug.Log("Drag");
        Vector3 tempLocalPosition = _lastDragged.transform.localPosition; //without this using _lastDragged.transform.localPosition below will cause bug
        if (_lastDragged.CompareTag("DropArea"))
        {
            foreach (GameObject teamMember in _lastDragged.GetComponent<DropArea>().GetTeamMembers()) //going to need to do recursion to move every person
            {
                teamMember.gameObject.transform.position = Input.mousePosition;
            }
            if (_lastDragged.transform.parent.CompareTag("DropArea")) //move parent instead, child will move with
            {
                Debug.Log("Parent should be: " + _lastDragged.name);
                _lastDragged.transform.parent.position =
                    Input.mousePosition - tempLocalPosition; 
            }
            else
            {
                _lastDragged.gameObject.transform.position = Input.mousePosition;
            }
        }
        else
        {
            _lastDragged.gameObject.transform.position = Input.mousePosition;
        }
    }
    void Drop()
    {
        Debug.Log("Drop");
        _dragActive = false;
        List<RaycastResult> results = new List<RaycastResult>(); 
        PointerEventData ped = new PointerEventData(null);
        ped.position = _screenPosition;
        _raycaster.Raycast(ped,results);
        GameObject potentialDropArea = results[^3].gameObject; //should always be the third last object, last is LineUpPanel, second last is Viewport
        Debug.Log(potentialDropArea.name);
        if (potentialDropArea.CompareTag("DropArea")) //possibly move everything below this into it's own function, also we must make sure we are not looking at the same object we are currently dragging 
        {
            Vector2 _newPosition = _screenPosition;
            if(!_lastDragged.gameObject.CompareTag("DropArea")) //This means that we are currently holding a non-DropArea
            {
                DropArea dropArea = potentialDropArea.GetComponent<DropArea>();
                if (dropArea.GetNumPositions() <= dropArea.GetTeamMembers().Count) //If there is someone in there, we need to swap them
                {
                    GameObject teamMember = dropArea.GetTeamMembers()[0];
                    teamMember.transform.SetAsLastSibling(); //need to make sure it appears on the top!
                    
                    LineUpTeamMember lineUpTeamMember = teamMember.GetComponent<LineUpTeamMember>();
                    lineUpTeamMember.RemoveLastDropArea(); //this auto-removes them from the dropArea as well
                    teamMember.transform.position = _lastDragged.GetComponent<LineUpTeamMember>().originalPosition;
                    if (_lastDropArea != null) //this means we are swapping two objects
                    {
                        lineUpTeamMember.SetLastDropArea(_lastDropArea);
                    }
                }
                _newPosition = potentialDropArea.transform.position;
                _lastDragged.GetComponent<LineUpTeamMember>().SetLastDropArea(potentialDropArea);
            }
            else //this means that we are trying to drop a DropArea, need to check if what is below is acceptable
            {
                //We know the _lastDragged and potentialDropArea both have the DropArea tag, we want to see if they are the same. If they are, we are fine placing it. If they are not, we need to shunt which is what this if statement is for
                if (!ReferenceEquals(_lastDragged, potentialDropArea))
                {
                    DropArea localDropArea = _lastDragged.GetComponent<DropArea>();
                    _newPosition = localDropArea.originalPosition;
                    foreach (GameObject teamMember in localDropArea.GetTeamMembers())
                    {
                        teamMember.transform.position = localDropArea.originalPosition;
                    }
                }
            }

            _lastDropArea = null; //reset because we are no longer dragging anything, lastDropArea is only relevant to the current _lastDrag object
            _lastDragged.transform.position = _newPosition;
        }
        
    }
    private void DestroyPotentials(GameObject protectedPotential)
    {
        if (_potentialsDisplayed)
        {
            if (protectedPotential != null)
            {
                foreach (GameObject dropArea in _potentialDropAreaList)
                {
                    if (dropArea != protectedPotential)
                    {
                        Destroy(dropArea);
                    }
                }
            }
            else
            {
                foreach (GameObject dropArea in _potentialDropAreaList)
                {
                    Destroy(dropArea);
                }
            }
            _potentialsDisplayed = false;
            _potentialDropAreaList = new List<GameObject>();
        }
    }
}
