using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MariaCtrl : MonoBehaviour
{
    public string horizontal = "Horizontal";
    public string vertical = "Vertical";
    public string fire1 = "Fire1";
    public string mouseX = "Mouse X";
    private float h = 0f, v = 0f, r = 0f; // ¸¶¿ì½º x 

    void Start()
    {
            
    }

    
    void Update()
    {
        h = Input.GetAxis(horizontal);
        v = Input.GetAxis(vertical);
        r = Input.GetAxis(mouseX);
    }
}
