using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;

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

    private float pitch = 0f;
    private string inputToEnable = "keyboard";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;

        InitActionMap();

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


        moveAction.performed += ctx => ActionMovePerformed(ctx.ReadValue<Vector2>());
        lookArroundAction.performed += ctx => ActionLookArroundPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed += ctx => ActionFirePerformed();
        reloadAction.performed += ctx => ActionReloadingPerformed();
        changeWeaponAction.performed += ctx => ActionChangeWeaponPerformed();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    
        if (moveAction.IsInProgress())
        {
            ActionMovePerformed(moveAction.ReadValue<Vector2>());
        }
        if (lookArroundAction.IsInProgress())
        {
            ActionLookArroundPerformed(lookArroundAction.ReadValue<Vector2>());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsOwner) return;
        moveAction.performed -= ctx => ActionMovePerformed(ctx.ReadValue<Vector2>());
        lookArroundAction.performed -= ctx => ActionLookArroundPerformed(ctx.ReadValue<Vector2>());
        fireAction.performed -= ctx => ActionFirePerformed();
        reloadAction.performed -= ctx => ActionReloadingPerformed();
        changeWeaponAction.performed -= ctx => ActionChangeWeaponPerformed();
    }

    /// <summary>
    /// handle action movement
    /// </summary>
    /// <param name="moveInput"></param>
    private void ActionMovePerformed(Vector2 moveInput)
    {
        playerInputScriptableObject.MoveCharacter(characterController, moveInput);
    }

    /// <summary>
    /// handle look arround movement
    /// </summary>
    /// <param name="lookInput"></param>
    private void ActionLookArroundPerformed(Vector2 lookInput)
    {
        playerInputScriptableObject.LookAround(cameraTransform, lookInput);
    }

    /// <summary>
    /// handle fire action
    /// </summary>
    private void ActionFirePerformed()
    {
        playerInputScriptableObject.FireWeapon();
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
        playerInputScriptableObject.ChangeWeapon();
    }
   
}
