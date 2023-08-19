using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    PlayerInput playerInput;
    CharacterController playerController;

    private Vector3 currentMovement;
    private bool isPlayerMoving;
    private int walking;
    private int running;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<CharacterController>();
        SetUpAnimationFlags();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput.PlayerControls.Movement.started += OnMovement;
        playerInput.PlayerControls.Movement.canceled += OnMovement;
        playerInput.PlayerControls.Movement.performed += OnMovement;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerController.Move(currentMovement * Time.deltaTime);
        AnimatePlayer();
    }

    private void OnEnable()
    {
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }

    void SetUpAnimationFlags()
    {
        animator = GetComponent<Animator>();
        walking = Animator.StringToHash("walking");
        running = Animator.StringToHash("running");
    }

    void AnimatePlayer()
    {
        bool playerWalking = animator.GetBool(walking);
        bool playerRunning = animator.GetBool(running);

        if (isPlayerMoving && !playerWalking) 
        {
            animator.SetBool(walking, true);
        }

        if (!isPlayerMoving && playerWalking) 
        {
            animator.SetBool(walking, false);
        }

        if (isPlayerMoving && !playerRunning)
        {
            animator.SetBool(running, true);
        }

        if (!isPlayerMoving && playerRunning)
        {
            animator.SetBool(running, false);
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        currentMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
        isPlayerMoving = currentMovement.x != 0 || currentMovement.z != 0;
    }

}
