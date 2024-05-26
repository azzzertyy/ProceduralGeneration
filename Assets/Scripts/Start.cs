using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour 
{
    public void StartWFC()
    {
        SceneManager.LoadScene("WFC");
    }
    public void StartDrunk()
    {
        SceneManager.LoadScene("Drunk");
    }
}