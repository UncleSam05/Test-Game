using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction repulsionAction;

    public Vector2 MoveInput { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool RepulsionPressed { get; private set; }

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
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
        repulsionAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
        repulsionAction.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        InteractPressed = interactAction.triggered;
        RepulsionPressed = repulsionAction.triggered;
    }
}