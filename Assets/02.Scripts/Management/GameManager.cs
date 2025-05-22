using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [Header("GameOver")]
    public bool isGameOver = false;

    [Header("PlayerHP")]
    public int playhp;
    public int playMaxHp = 100;
    public Image playerHpBar;

    //[Header("Boss HP")]
    //public int bosshp;
    //public int bossMaxHp = 300;
    //public Image BossHpBar;

    public void PlayerHit(int damage)
    {
        playhp -= damage;
        playhp = Mathf.Clamp(playhp, 0, 100);
        playerHpBar.fillAmount = (float)playhp / (float)playMaxHp;
    }
    //public void BossHit(int damage)
    //{
    //    bosshp -= damage;
    //    bosshp = Mathf.Clamp(bosshp, 0, 300);
    //}
}
