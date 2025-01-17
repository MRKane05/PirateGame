using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//A function to trigger any command through an input on the controller
public class ButtonPressTrigger : MonoBehaviour
{
    public string ControllerButton = "Cross";
    public KeyCode KeyboardButton;
    public UnityEvent PressEvent;

    void Update()
    {
        if (Input.GetKeyDown(KeyboardButton) ||  Input.GetButtonDown(ControllerButton))
        {
            PressEvent.Invoke();
        }
    }
}
