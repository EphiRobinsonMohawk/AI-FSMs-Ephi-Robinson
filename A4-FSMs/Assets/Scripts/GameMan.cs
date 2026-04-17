using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GameMan : MonoBehaviour
{
    public static PlayerController player;
    public static Image eaten;
    public ImageRef reference;


    private void Awake()
    {
        eaten = reference.eaten;
    }
}


