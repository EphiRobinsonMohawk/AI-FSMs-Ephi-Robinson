using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreatDancersOfTheWorld : MonoBehaviour
{
    public Image UNITE;
    public float UniteTimer;
    public float UniteTimerMax;

    private void Update()
    {
        if (UniteTimer > UniteTimerMax)
        {
            UNITE.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UniteTimer += Time.deltaTime;
        }
    }
}
