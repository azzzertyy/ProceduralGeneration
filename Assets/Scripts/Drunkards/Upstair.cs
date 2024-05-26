using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upstair : MonoBehaviour 
{
    [SerializeField] private GameObject playerPrefab;
    void Awake()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
}