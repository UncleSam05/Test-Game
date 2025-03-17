using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    // Input actions for movement, boost, brake, and cursor.
    private InputAction moveAction;
    private InputAction boostAction;
    private InputAction brakeAction;
    private InputAction cursorAction;
    private InputAction interactAction;

    // Exposed properties for other scripts.
    public Vector2 MoveInput { get; private set; }
    public bool BoostPressed { get; private set; }
    public bool BrakePressed { get; private set; }
    public Vector2 CursorPosition { get; private set; }
    public bool InteractPressed { get; private set; }

    private void Awake()
    {
        // Set up the Move action using a composite binding for WASD and Arrow keys.
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        boostAction = new InputAction("Boost", binding: "<Keyboard>/space");
        brakeAction = new InputAction("Brake", binding: "<Keyboard>/q");
        cursorAction = new InputAction("Cursor", binding: "<Mouse>/position");
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        boostAction.Enable();
        brakeAction.Enable();
        cursorAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        boostAction.Disable();
        brakeAction.Disable();
        cursorAction.Disable();
        interactAction.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        // For boost, we check if the action was triggered this frame.
        BoostPressed = boostAction.triggered;
        // For brake, a simple threshold on the button's value.
        BrakePressed = brakeAction.ReadValue<float>() > 0.5f;
        CursorPosition = cursorAction.ReadValue<Vector2>();
        InteractPressed = interactAction.triggered;
    }
}
