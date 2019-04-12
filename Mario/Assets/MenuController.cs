using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public Text startText;
    public Text loadText;

    SelectedText selectedText;
    enum SelectedText
    {
        START,
        LOAD
    }
    
    void Start()
    {
        selectedText = SelectedText.START;
        startText.fontStyle = FontStyle.Bold;
    }
    
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Vertical"))
        {
            ToggleSelection();
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump") || CrossPlatformInputManager.GetButtonDown("Enter"))
        {
            if (selectedText == SelectedText.START)
            {
                GameControl.control.PrepareForTransitionToNextLevel();
            }
            else if (selectedText == SelectedText.LOAD)
            {
                GameControl.control.PrepareForLoadingSavedLevel();
            }
        }
    }

    void ToggleSelection()
    {
        switch (selectedText)
        {
            case SelectedText.START:
                startText.fontStyle = FontStyle.Normal;
                loadText.fontStyle = FontStyle.Bold;
                selectedText = SelectedText.LOAD;
                break;
            case SelectedText.LOAD:
                loadText.fontStyle = FontStyle.Normal;
                startText.fontStyle = FontStyle.Bold;
                selectedText = SelectedText.START;
                break;
        }
    }
}
