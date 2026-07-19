using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private PlayerInputScriptableObject playerInputScriptableObject;
    [SerializeField]
    private WeaponController weaponController;
    [SerializeField]
    private Transform aimPoint;
    [SerializeField]
    private Transform cameraTransform;


    [Header("InputAction")]
    private InputAction moveAction;
    private InputAction lookArroundAction;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction changeWeaponAction;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

   

    private string inputToEnable = "keyboard";
    /// <summary>
    /// vector2 holding the value being read from the move input
    /// </summary>
    public Vector2 moveInput { get; private set; }
    /// <summary>
    /// vector2 holding the value being read from the lookArround input
    /// </summary>
    public Vector2 lookArroundInput { get; private set; }

    /// <summary>
    /// the size of the buffer
    /// </summary>
    private readonly static int BUFFER_SIZE = 1024;
    /// <summary>
    /// an array container the input
    /// </summary>
    public PlayerInputSerialization[] positionBuffer = new PlayerInputSerialization[BUFFER_SIZE];
    // ticks starts at 1 , the array starts at 0 -> we have to deduct 1 to keep the ticks and the array coherent
    // modulo 1024 is to avoid index out of bound exception 
    private int TickToIndex(int tick) => ((tick - 1) < 0 ? 0 : ((tick - 1) % 1024));

    /// <summary>
    /// subscribe to action events .
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        InitActionMap();
        NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
    }

    /// <summary>
    ///  unsubscribe from the input action events to avoid memory leaks and unintended behavior.
    /// </summary>
    public override void OnNetworkDespawn()
    {
        moveAction.performed -= ctx => ActionMovePerformed(ctx.ReadValue<Vector2>());
        lookArroundAction.performed -= ctx => ActionLookArroundPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed -= ctx => ActionFirePerformed();
        reloadAction.performed -= ctx => ActionReloadingPerformed();
        changeWeaponAction.performed -= ctx => ActionChangeWeaponPerformed();

        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
    }

    private void OnNetworkTick()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.IsInProgress() ? moveAction.ReadValue<Vector2>() : new Vector2(0, 0);
            ActionMovePerformed(moveInput);
        }

        if (lookArroundAction != null && lookArroundAction.IsInProgress())
        {
            lookArroundInput = lookArroundAction.ReadValue<Vector2>();
            ActionLookArroundPerformed(lookArroundInput);
        }
    }

    /// <summary>
    /// Initializes the input action map by finding the relevant actions and subscribing to their performed events.
    /// </summary>
    private void InitActionMap()
    {
        moveAction = InputSystem.actions.FindActionMap(inputToEnable).FindAction("move");
        lookArroundAction = InputSystem.actions.FindActionMap(inputToEnable).FindAction("look");
        fireAction = InputSystem.actions.FindActionMap(inputToEnable).FindAction("fire");
        reloadAction = InputSystem.actions.FindActionMap(inputToEnable).FindAction("reload");
        changeWeaponAction = InputSystem.actions.FindActionMap(inputToEnable).FindAction("changeWeapon");

        Debug.Log("moveAction " + moveAction);
        Debug.Log("lookArroundAction " + lookArroundAction);
        Debug.Log("fireAction " + fireAction);
        Debug.Log("reloadAction " + reloadAction);
        Debug.Log("changeWeaponAction " + changeWeaponAction);
        Debug.Log("inputToEnable " + inputToEnable);
        moveAction.performed += ctx => ActionMovePerformed(ctx.ReadValue<Vector2>());
        lookArroundAction.performed += ctx => ActionLookArroundPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed += ctx => ActionFirePerformed();
        reloadAction.performed += ctx => ActionReloadingPerformed();
        changeWeaponAction.performed += ctx => ActionChangeWeaponPerformed();
    }

    /// <summary>
    /// handle action movement
    /// </summary>
    /// <param name="moveInput"></param>
    private Vector3 ActionMovePerformed(Vector2 moveInput)
    {
        return playerInputScriptableObject.MoveCharacter(characterController, moveInput);
    }

    /// <summary>
    /// handle look arround movement
    /// </summary>
    /// <param name="lookInput"></param>
    private void ActionLookArroundPerformed(Vector2 lookInput)
    {
      //  playerInputScriptableObject.LookAround(cameraTransform, lookInput);
    }

    /// <summary>
    /// handle fire action
    /// </summary>
    private void ActionFirePerformed()
    {
        playerInputScriptableObject.FireWeapon(weaponController);
    }

    /// <summary>
    /// handle reload action
    /// </summary>
    private void ActionReloadingPerformed()
    {
        playerInputScriptableObject.ReloadWeapon();
    }

    /// <summary>
    /// handle change weapon action
    /// </summary>
    private void ActionChangeWeaponPerformed()
    {
        playerInputScriptableObject.ChangeWeapon(weaponController);
    }
}
