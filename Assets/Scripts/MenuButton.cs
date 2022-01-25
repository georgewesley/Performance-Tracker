using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    [SerializeField] GameObject mainMenuPanel;
    public void OnPress(string newPanel)
    {
        foreach(GameObject panel in panels) {
            Debug.Log(panel.name);
            if (panel.name == newPanel) {
                Debug.Log(panel.name);
                panel.SetActive(true);
                mainMenuPanel.SetActive(false);
                return;
            }
        }
    }
}
