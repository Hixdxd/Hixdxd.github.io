using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float ZHP = 100f;

    private void Update()
    {
        
    }
    public void TakeHit(float damage) // 피격처리
    {
        ZHP -= damage; 
        Debug.Log("Enemy HP: " + ZHP);
        if (ZHP <= 0)
        {
            
            Destroy(gameObject);
        }
    }
}
