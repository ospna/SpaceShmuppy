using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            SceneManager.LoadScene("_Scene_0");
        }
    }
}
