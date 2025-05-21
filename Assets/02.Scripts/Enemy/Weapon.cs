using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;
    public BoxCollider meleeArea;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerCtrl playerhealth = collision.gameObject.GetComponent<PlayerCtrl>();
        if (playerhealth != null)
        {
            //playerhealth.OnHit(damage);
        }
    }
}
