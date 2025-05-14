using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainStop : MonoBehaviour
{
    public GameObject rainPrefab; // 비 프리팹
    public GameObject rainObj; // 비 오브젝트
    [SerializeField] private readonly string player = "Player";
    
    void Start()
    {
        rainObj = Instantiate(rainPrefab); // 비 프리팹을 인스턴스화 하여 
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(player))
        {
            Destroy(rainObj); // 비 오브젝트 삭제
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag(player))
        {
            rainObj = Instantiate (rainPrefab); // 비 프리팹을 인스턴스화하여 rainObj에 저장
        }
    }

}
