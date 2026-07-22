using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerInputScriptableObject", menuName = "Scriptable Objects/PlayerInputScriptableObject")]
public class PlayerInputScriptableObject : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Look")]
    public float lookSpeed = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float pitch = 0f;

    public void MoveCharacter(CharacterController characterController, Vector2 moveValue)
    {
        // Direction relative à l'orientation du perso (pas espace monde)
        Vector3 movementVector = (characterController.transform.right * moveValue.x + characterController.transform.forward * moveValue.y) * moveSpeed;
        characterController.Move(movementVector * Time.deltaTime);
    }

    public void RotateCharacter(Transform characterTransform, Transform cameraTransform, Vector2 lookValue)
    {
        float mouseX = lookValue.x * lookSpeed;
        float mouseY = lookValue.y * lookSpeed;

        // Rotation du personnage (Player) sur l'axe Y
        characterTransform.Rotate(Vector3.up * mouseX);

        // Rotation de la caméra sur l'axe X (pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void FireWeapon(WeaponController weaponController)
    {
        Debug.Log("firing");
        weaponController.RequestWeaponFire();
    }

    public void ReloadWeapon(WeaponController weaponController)
    {
        Debug.Log("reloading");
        weaponController.RequestWeaponReload();
    }

    public void ChangeWeapon(WeaponController weaponController)
    {
        Debug.Log("switch weapon");
        weaponController.SelectNextWeaponIndex();
    }
}
