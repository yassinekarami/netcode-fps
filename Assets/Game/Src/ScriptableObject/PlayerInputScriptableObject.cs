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

    public Vector3 MoveCharacter(CharacterController characterController, Vector2 moveValue)
    {
        // Direction relative à l'orientation du perso (pas espace monde)
        Vector3 movementVector = (characterController.transform.right * moveValue.x + characterController.transform.forward * moveValue.y) * moveSpeed;

        characterController.Move(movementVector * Time.deltaTime);
        return characterController.transform.position;
    }

    public void LookAround(Transform cameraTransform, Vector2 lookValue)
    {
        float mouseX = lookValue.x * lookSpeed;
        float mouseY = lookValue.y * lookSpeed;
        // Rotation du personnage sur l'axe Y (gauche/droite)
        cameraTransform.parent.Rotate(Vector3.up * mouseX);
        // Rotation de la caméra sur l'axe X (haut/bas)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    public void ChangeWeapon(CharacterController characterController)
    {
        Debug.Log("switch weapon");
    }

    public void FireWeapon(WeaponController weaponController)
    {
        Debug.Log("firing");
        weaponController.RequestWeaponFire();
    }

    public void ReloadWeapon()
    {
        Debug.Log("reloading");
    }

    public void ChangeWeapon(WeaponController weaponController)
    {
        Debug.Log("switch weapon");
        weaponController.SelectNextWeapon();
    }
}
