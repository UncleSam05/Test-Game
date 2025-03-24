using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction repulsionAction;
    private InputAction mineAction;

    public Vector2 MoveInput { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool RepulsionPressed { get; private set; }
    // Returns true when the left mouse button is held.
    public bool IsMinePressed { get { return mineAction.ReadValue<float>() > 0.5f; } }

    private void Awake()
    {
        // Movement (WASD and Arrow keys).
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

        // Interact (E key).
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        // Repulsion (R key).
        repulsionAction = new InputAction("Repulsion", binding: "<Keyboard>/r");
        // Mine (Left Mouse Button).
        mineAction = new InputAction("Mine", binding: "<Mouse>/leftButton");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
        repulsionAction.Enable();
        mineAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
        repulsionAction.Disable();
        mineAction.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        InteractPressed = interactAction.triggered;
        RepulsionPressed = repulsionAction.triggered;
        // Mine input is read on demand via IsMinePressed.
    }
}