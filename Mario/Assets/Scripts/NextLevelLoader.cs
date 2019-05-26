using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class NextLevelLoader : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump") || CrossPlatformInputManager.GetButtonDown("Enter"))
        {
            //GameControl.control.LoadNextLevel();
        }
    }
}
