using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange1 : MonoBehaviour
{
    public void ChangeFirstScene()
    {
        SceneManager.LoadScene("Prototype 4");
    }

    public void ChangeSecondScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}
