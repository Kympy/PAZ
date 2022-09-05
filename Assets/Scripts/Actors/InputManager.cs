using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    internal enum InputType { Keyboard, Mobile } // Input Type
    [SerializeField] private InputType _Input = InputType.Keyboard;

    [HideInInspector] private float vertical = 0f; // Up and Down
    [HideInInspector] private float horizontal = 0f; // Left and Right
    [HideInInspector] private bool jump = false; // Jump
    [HideInInspector] private bool sprint = false; // Run
    [HideInInspector] private bool shiftUp = false; // Stop Run

    private bool leftClick = false;
    private bool leftClicking = false;
    private bool rightClicking = false;
    private bool rightClickUp = false;

    private bool EKey = false;
    private bool reload = false;
    private bool tabKey = false;

    private bool Slot_1 = false;
    private bool Slot_2 = false;
    private bool Slot_3 = false;

    private bool hasVerticalInput = false;
    private bool hasHorizontalInput = false;

    public float Vertical { get { return vertical; } } // Get V, H variables
    public float Horizontal { get { return horizontal; } }
    public bool Jump { get { return jump; } }
    public bool Sprint { get { return sprint; } }
    public bool ShiftUp { get { return shiftUp; } }

    public bool LeftClick { get { return leftClick; } }

    public bool LeftClicking { get { return leftClicking; } }
    public bool RightClicking { get { return rightClicking; } }
    public bool RightClickUp { get { return rightClickUp; } }
    public bool E { get { return EKey; } }
    public bool Reload { get { return reload; } }
    public bool TabKey { get { return tabKey; } }

    public bool Slot1 { get { return Slot_1; } }
    public bool Slot2 { get { return Slot_2; } }
    public bool Slot3 { get { return Slot_3; } }
    public bool HasVerticalInput { get { return hasVerticalInput; } }
    public bool HasHorizontalInput { get { return hasHorizontalInput; } }

    
    public override void Awake()
    {
        
    }
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
        jump = Input.GetKeyDown(KeyCode.Space);
        sprint = Input.GetKey(KeyCode.LeftShift);
        shiftUp = Input.GetKeyUp(KeyCode.LeftShift);

        Slot_1 = Input.GetKeyDown(KeyCode.Alpha1);
        Slot_2 = Input.GetKeyDown(KeyCode.Alpha2);
        Slot_3 = Input.GetKeyDown(KeyCode.Alpha3);

        EKey = Input.GetKeyDown(KeyCode.E);
        reload = Input.GetKeyDown(KeyCode.R);
        tabKey = Input.GetKey(KeyCode.Tab);

        leftClick = Input.GetMouseButtonDown(0);
        leftClicking = Input.GetMouseButton(0);
        rightClicking = Input.GetMouseButton(1);
        rightClickUp = Input.GetMouseButtonUp(1);

        hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
    }
}
