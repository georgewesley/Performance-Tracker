using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class DragController : MonoBehaviour
{
    private bool _dragActive = false;
    private Vector2 _screenPosition;
    private GameObject _lastDragged;
    private PointerEventData _pointerEventData;
    private GraphicRaycaster _raycaster;
    
    void Awake()
    {
        _raycaster = gameObject.GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            _screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }
        
        if (_dragActive)
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
            Debug.Log("We did raycast");
            if (results.Count > 0) //this will pretty much always be true because there are many components that can be hit by raycasts
            {
                GameObject draggable = results[0].gameObject; //We only want the object that is on the top of the others
                if (draggable != null && draggable.CompareTag("draggable"))  //we check this to make sure we are actually supposed to be able to drag the object we have
                {
                    _lastDragged = draggable;
                    NewDrag();
                }
            } 
        }
    }

    void NewDrag()
    {
        _dragActive = true;
        //_lastDragged.GetComponent<Image>(); = false; //we need to be able to detect what is below it
    }
    void Drag()
    {
        _lastDragged.gameObject.transform.position = Input.mousePosition;
    }
    void Drop()
    {
        _dragActive = false;
        List<RaycastResult> results = new List<RaycastResult>(); 
        PointerEventData ped = new PointerEventData(null);
        ped.position = _screenPosition;
        _raycaster.Raycast(ped,results);
        if (results.Count > 1) //The DropArea will always be the second object hit by the raycast since the mouse is currently holding an image which is getting hit first
        {
            Debug.Log("Greater than one result");
            GameObject potentialDropArea = results[1].gameObject;
            if (potentialDropArea.CompareTag("DropArea"))
            {
                _lastDragged.gameObject.transform.position = potentialDropArea.transform.position;
            }
        }
    }
}
