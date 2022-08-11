using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PotentialDropArea : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnPress());
    }
    
    private void OnPress()
    {
        //Now am removing in DragController, keeping this here just incase
        /*gameObject.transform.SetAsFirstSibling(); 
        for (int i = 1; i < gameObject.transform.parent.childCount; i++) //delete all the other buttons 
        {
            GameObject potentialDropArea = gameObject.transform.parent.GetChild(i).gameObject;
            if (potentialDropArea.CompareTag("PotentialDropArea"))
            {
                Destroy(potentialDropArea);
            }
        }*/
        gameObject.tag = "DropArea";
        //change tag to DropArea (the script not being disabled should be fine because we only ever check for tag, not if it has the component DropArea)
        gameObject.GetComponent<Image>().color = Color.white;
        gameObject.GetComponent<Button>().enabled = false;
        gameObject.GetComponent<PotentialDropArea>().enabled = false;
    }
}
