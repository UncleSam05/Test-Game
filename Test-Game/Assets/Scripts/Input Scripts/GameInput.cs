using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction repulsionAction;
    private InputAction mineAction;
    private InputAction toggleDrillAction; // new action for toggling the drill

    public Vector2 MoveInput { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool RepulsionPressed { get; private set; }
    // Returns true when the left mouse button is held.
    public bool IsMinePressed { get { return mineAction.ReadValue<float>() > 0.5f; } }
    // Returns true when the toggle drill action is triggered (X key pressed).
    public bool ToggleDrillTriggered { get { return toggleDrillAction.triggered; } }

    private void Awake()
    {
        // Movement binding.
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

        // Interact binding.
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        // Repulsion binding.
        repulsionAction = new InputAction("Repulsion", binding: "<Keyboard>/r");
        // Mine binding (left mouse).
        mineAction = new InputAction("Mine", binding: "<Mouse>/leftButton");
        // Toggle Drill binding (X key).
        toggleDrillAction = new InputAction("ToggleDrill", binding: "<Keyboard>/x");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
        repulsionAction.Enable();
        mineAction.Enable();
        toggleDrillAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
        repulsionAction.Disable();
        mineAction.Disable();
        toggleDrillAction.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        InteractPressed = interactAction.triggered;
        RepulsionPressed = repulsionAction.triggered;
    }
}
