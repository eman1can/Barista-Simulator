using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Created by Ethan Wolfe
 * Controls the GUI popup aspect of interaction
 */
public class InteractionController : MonoBehaviour {
    [Header("Display Options")]
    [SerializeField] private GameObject namePanel;
    [SerializeField] private Text nameDisplay;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject infoPrefab;
    [SerializeField] private int slotCount = 3;
    
    private string name;
    private InfoPanel[] panels;
    public void HideName() {
        name = null;
        namePanel.SetActive(false);
    }

    public void SetName(string newName) {
        name = newName;
        nameDisplay.text = newName;
        namePanel.SetActive(true);
    }

    public string GetName() {
        return name;
    }

    private void Awake() {
        panels = new InfoPanel[slotCount];
        for (var i = 0; i < slotCount; i++) {
            GameObject infoObject = Instantiate(infoPrefab);
            infoObject.transform.SetParent(infoPanel.transform);
            infoObject.SetActive(false);
            panels[i] = infoObject.GetComponent<InfoPanel>();
        }
    }

    public void SetInfo(Sprite[] images, string[] strings, int count) {
        infoPanel.SetActive(true);
        for (var i = 0; i < count; i++) {
            panels[i].SetImage(images[i]);
            panels[i].SetText(strings[i]);
            panels[i].gameObject.SetActive(true);
        }
    }

    public void UpdateStrings(string[] strings, int count) {
        for (var i = 0; i < count; i++)
            panels[i].SetText(strings[i]);
    }

    public void HideInfo() {
        for (var i = 0; i < slotCount; i++)
            panels[i].gameObject.SetActive(false);
        infoPanel.SetActive(false);
    }

    private void OnDestroy() {
        panels = null;
    }
}