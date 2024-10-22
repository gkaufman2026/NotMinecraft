using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGUI : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 90), "Terrain");
        if (GUI.Button(new Rect(20, 40, 80, 20), "Level 1"))
        {
           
        }

        if (GUI.Button(new Rect(20, 70, 80, 20), "Level 2"))
        {
            
        }
    }
}
