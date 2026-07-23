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
    private GameObject playerCamera;
    [SerializeField]
    private GameObject weaponHolder;


    [Header("InputAction")]
    private InputAction moveAction;
    private InputAction lookArroundAction;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction changeWeaponAction;
   

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
    /// subscribe to action events .
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerCamera.SetActive(IsOwner);
        playerCamera.tag = "MainCamera";
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
        lookArroundAction.performed -= ctx => ActionRotateCharacterPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed -= ctx => ActionFirePerformed();
        reloadAction.performed -= ctx => ActionReloadingPerformed();
        changeWeaponAction.performed -= ctx => ActionChangeWeaponPerformed();

        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
    }
    /// <summary>
    /// method called at each tick used to handle the input
    /// </summary>
    private void OnNetworkTick()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.IsInProgress() ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            ActionMovePerformed(moveInput);
        }

        if (lookArroundAction != null )
        {
            lookArroundInput = lookArroundAction.IsInProgress()? lookArroundAction.ReadValue<Vector2>() : Vector2.zero;
            ActionRotateCharacterPerformed(lookArroundInput);
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
        lookArroundAction.performed += ctx => ActionRotateCharacterPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed += ctx => ActionFirePerformed();
        reloadAction.performed += ctx => ActionReloadingPerformed();
        changeWeaponAction.performed += ctx => ActionChangeWeaponPerformed();
    }

    /// <summary>
    /// handle action movement
    /// </summary>
    /// <param name="moveInput"></param>
    private void ActionMovePerformed(Vector2 moveInput)
    {
        if (!IsOwner) { return; }
        playerInputScriptableObject.MoveCharacter(characterController, moveInput);
    }

    /// <summary>
    /// handle character rotation
    /// </summary>
    /// <param name="lookInput"></param>
    private void ActionRotateCharacterPerformed(Vector2 lookInput)
    {
        if (!IsOwner) { return; }
        playerInputScriptableObject.RotateCharacter(gameObject.transform, weaponHolder.transform, playerCamera.transform, lookInput);
    }

    /// <summary>
    /// handle fire action
    /// </summary>
    private void ActionFirePerformed()
    {
        if (!IsOwner) { return; }
        playerInputScriptableObject.FireWeapon(weaponController);
    }

    /// <summary>
    /// handle reload action
    /// </summary>
    private void ActionReloadingPerformed()
    {
        if (!IsOwner) { return; }
        playerInputScriptableObject.ReloadWeapon(weaponController);
    }

    /// <summary>
    /// handle change weapon action
    /// </summary>
    private void ActionChangeWeaponPerformed()
    {
        if (!IsOwner) { return; }
        playerInputScriptableObject.ChangeWeapon(weaponController);
    }
}
