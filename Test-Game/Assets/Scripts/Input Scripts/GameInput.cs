using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    // Input actions for movement, boost, brake, cursor, and interact.
    private InputAction moveAction;
    private InputAction boostAction;
    private InputAction brakeAction;
    private InputAction cursorAction;
    private InputAction interactAction; // if needed for other mechanics

    // Exposed properties for other scripts.
    public Vector2 MoveInput { get; private set; }
    public bool BoostPressed { get; private set; }
    public bool BrakePressed { get; private set; }
    public Vector2 CursorPosition { get; private set; }
    public bool InteractPressed { get; private set; }

    // Buffer field for boost trigger.
    private bool boostBuffered = false;

    private void Awake()
    {
        // Set up the Move action using composite binding for WASD and Arrow keys.
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

        // Instead of directly setting BoostPressed from boostAction.triggered,
        // we buffer the trigger so that it remains true for at least one FixedUpdate.
        if (boostAction.triggered)
            boostBuffered = true;
        BoostPressed = boostBuffered;

        BrakePressed = brakeAction.ReadValue<float>() > 0.5f;
        CursorPosition = cursorAction.ReadValue<Vector2>();

        InteractPressed = interactAction.triggered;
    }

    /// <summary>
    /// Call this method from FixedUpdate (after processing boost) to clear the buffered boost.
    /// </summary>
    public void ConsumeBoost()
    {
        boostBuffered = false;
    }
}
