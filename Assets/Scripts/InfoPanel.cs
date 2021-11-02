using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Created by Ethan Wolfe
 * Controls a slot on the interaction GUI that displays an icon and text
 */
public class InfoPanel : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Text text;

    public void SetImage(Sprite sprite) {
        image.sprite = sprite;
    }

    public void SetText(string newString) {
        text.text = newString;
    }
}