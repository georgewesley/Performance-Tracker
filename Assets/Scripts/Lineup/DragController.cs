using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;
using Object = System.Object;

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
    [SerializeField] private GameObject dropAreaPrefab;
    [SerializeField] private GameObject teamMemberViewport;
    
    private void Awake()
    {
        _raycaster = gameObject.GetComponent<GraphicRaycaster>();
        _potentialDropAreaList = new List<GameObject>();
        _potentialsDisplayed = false;
    }

    // Update is called once per frame
    private void Update()
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
                            _potentialDropAreaList.Add(Instantiate(dropAreaPrefab, _lastDragged.transform, false)); 
                            _potentialDropAreaList[0].transform.localPosition = new Vector2(-170, 0);
                            _potentialDropAreaList.Add(Instantiate(dropAreaPrefab, _lastDragged.transform, false)); 
                            _potentialDropAreaList[1].transform.localPosition = new Vector2(170, 0);
                            _potentialDropAreaList.Add(Instantiate(dropAreaPrefab, _lastDragged.transform, false)); 
                            _potentialDropAreaList[2].transform.localPosition = new Vector2(0, -170);
                            _potentialDropAreaList.Add(Instantiate(dropAreaPrefab, _lastDragged.transform, false)); 
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

    private void NewDrag()
    {
        Debug.Log("New Drag");
        _dragActive = true;
        if(_lastDragged.GetComponent<LineUpTeamMember>() != null)
        {
            _lastDragged.transform.SetParent(teamMemberViewport.transform); //whenever we pick up a team member we want them to be in the team member list
            _lastDragged.transform.SetAsLastSibling(); // We do this so it is always on top.
            LineUpTeamMember lineUpTeamMember = _lastDragged.GetComponent<LineUpTeamMember>();
            lineUpTeamMember.originalPosition = lineUpTeamMember.GetCurrentPosition();
            _lastDropArea = lineUpTeamMember.RemoveLastDropArea();
        }
        else //this is if _lastDragged is a DropArea, we need to put the original parent on top and record the position of the original parent just encase we make an invalid move
        {
            DropArea tempDropArea = _lastDragged.GetComponent<DropArea>();
            tempDropArea.originalDropArea.transform.SetAsLastSibling();
            DropArea tempOriginalDropArea = tempDropArea.originalDropArea.GetComponent<DropArea>();
            tempOriginalDropArea.originalPosition = tempOriginalDropArea.transform.position;
        }
    }
    private void Drag()
    {
        Debug.Log("Drag");
        if (_lastDragged.CompareTag("DropArea"))
        {
            DropArea tempDropArea = _lastDragged.GetComponent<DropArea>();
            Vector3 tempLocalPosition = tempDropArea.originalDropArea.transform.position - _lastDragged.transform.position; //should be 0 when it is original, should give offset if not
            tempDropArea.originalDropArea.transform.position = Input.mousePosition + tempLocalPosition;
        }
        else
        {
            _lastDragged.gameObject.transform.position = Input.mousePosition;
        }
    }
    private void Drop()
    {
        Debug.Log("Drop");
        _dragActive = false;
        List<RaycastResult> results = new List<RaycastResult>(); 
        PointerEventData ped = new PointerEventData(null);
        ped.position = _screenPosition;
        _raycaster.Raycast(ped,results);
        GameObject potentialDropArea = results[^3].gameObject; //should always be the third last object, last is LineUpPanel, second last is Viewport
        Debug.Log(potentialDropArea.name);
        if (potentialDropArea.CompareTag("DropArea")) 
        {
            DropInDropArea(potentialDropArea);
        }
        //if it is not a drop area, turning _dragActive to false is all we really have to do
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

    private void DropInDropArea(GameObject potentialDropArea)
    { 
        if(!_lastDragged.gameObject.CompareTag("DropArea")) //This means that we are currently holding a non-DropArea
        {
            DropArea dropArea = potentialDropArea.GetComponent<DropArea>();
            if (dropArea.GetTeamMember() != null) //If there is someone in there, we need to swap them
            {
                GameObject teamMember = dropArea.GetTeamMember(); //gets team member that is currently in dropArea (the one we are going to replace with our _lastDragged)
                LineUpTeamMember lineUpTeamMember = teamMember.GetComponent<LineUpTeamMember>();
                lineUpTeamMember.RemoveLastDropArea(); //this auto-removes them from the dropArea as well
                teamMember.transform.position = _lastDragged.GetComponent<LineUpTeamMember>().originalPosition;
                if (_lastDropArea != null) //this means we are swapping between two dropAreas
                {
                    lineUpTeamMember.SetLastDropArea(_lastDropArea);
                }
                else
                {
                    teamMember.transform.SetParent(teamMemberViewport.transform); //if we are swapping between a team member that has no dropArea and one that does, we need to move the one formally in dropArea back to the team member list
                }
            }
            _lastDragged.transform.position = potentialDropArea.transform.position;
            _lastDragged.GetComponent<LineUpTeamMember>().SetLastDropArea(potentialDropArea);
        }
        else //this means that we are trying to drop a DropArea, need to check if what is below is acceptable
        {
            //We know the _lastDragged and potentialDropArea both have the DropArea tag, we want to see if they are the same. If they are, we are fine placing it. If they are not, we need to shunt which is what this if statement is for
            if (!ReferenceEquals(_lastDragged, potentialDropArea))
            {
                DropArea localDropArea = _lastDragged.GetComponent<DropArea>();
                localDropArea.originalDropArea.transform.position =
                    localDropArea.originalDropArea.GetComponent<DropArea>().originalPosition;
            }
        }
        _lastDropArea = null; //reset because we are no longer dragging anything, lastDropArea is only relevant to the current _lastDrag object    
    }
}
