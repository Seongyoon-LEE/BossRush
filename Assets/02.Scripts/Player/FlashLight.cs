using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1.F키를 누르면 라이트가 켜진다.
// 2.다시 F키를 누르면 라이트가 꺼진다.
public class FlashLight : MonoBehaviour
{
    [SerializeField] private Light flashLight;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip LightOnclip;
    void Start()
    {
        flashLight = GetComponent<Light>();
        source = GetComponent<AudioSource>();
        
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            flashLight.enabled = !flashLight.enabled;
            source.PlayOneShot(LightOnclip);
        }
    }
}
