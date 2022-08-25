using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    internal enum InputType { Keyboard, Mobile } // Input Type
    [SerializeField] private InputType _Input = InputType.Keyboard;

    [HideInInspector] private float vertical = 0f; // Up and Down
    [HideInInspector] private float horizontal = 0f; // Left and Right
    //[HideInInspector] private float jump = 0f; // Jump
    [HideInInspector] private bool jump = false; // Jump
    [HideInInspector] private bool sprint = false; // Run
    [HideInInspector] private bool brake = false; // Stop Run

    private bool hasVerticalInput = false;
    private bool hasHorizontalInput = false;

    public float Vertical { get { return vertical; } } // Get V, H variables
    public float Horizontal { get { return horizontal; } }
    public bool Jump { get { return jump; } }
    public bool Sprint { get { return sprint; } }
    public bool Brake { get { return brake; } }
    public bool HasVerticalInput { get { return hasVerticalInput; } }
    public bool HasHorizontalInput { get { return hasHorizontalInput; } }

    private void Update()
    {
        if(_Input == InputType.Keyboard) // By Input Type
        {
            KeyboardInput();
        }
        else
        {
            //
        }
    }
    private void KeyboardInput() // KeyBoard Input Update
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        //jump = Input.GetAxis("Jump");
        jump = Input.GetKeyDown(KeyCode.Space);
        sprint = Input.GetKey(KeyCode.LeftShift);
        brake = Input.GetKeyUp(KeyCode.LeftShift);

        hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
    }
}
