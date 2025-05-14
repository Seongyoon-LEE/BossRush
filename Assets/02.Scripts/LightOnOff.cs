using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1.콜라이더에 부딪혔을 때 라이트가 켜지도록 구현
// 2.콜라이더 밖에 나가면 라이트가 꺼지도록 구현

public class LightOnOff : MonoBehaviour
{
    public Light _light;
    private AudioSource source;
    public AudioClip _lighteOnSound;
    public AudioClip _lighteOffSound;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) // isTrigger체크 되고 콜라이더 안에 들어왔을 때
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _light.enabled = true;
            source.PlayOneShot(_lighteOnSound);
        }
    }
    private void OnTriggerExit(Collider other) // isTrigger체크 되고 콜라이더 밖에 나갈때
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _light.enabled = false;
            source.PlayOneShot(_lighteOffSound);
        }
    }
}
