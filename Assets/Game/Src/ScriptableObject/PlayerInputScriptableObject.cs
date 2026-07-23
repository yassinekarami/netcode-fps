using UnityEngine;

/// <summary>
/// ScriptableObject that stores player input settings and handles
/// character movement, camera rotation, and weapon actions.
/// </summary>
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

    /// <summary>
    /// Moves the character based on the movement input.
    /// </summary>
    /// <param name="characterController">Character controller to move.</param>
    /// <param name="moveValue">Movement input.</param>
    public void MoveCharacter(CharacterController characterController, Vector2 moveValue)
    {
        Vector3 movementVector = (characterController.transform.right * moveValue.x + characterController.transform.forward * moveValue.y) * moveSpeed;
        characterController.Move(movementVector * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the character and updates the camera and weapon holder rotation.
    /// </summary>
    /// <param name="characterTransform">Player transform.</param>
    /// <param name="weaponHolder">Weapon holder transform.</param>
    /// <param name="cameraTransform">Camera transform.</param>
    /// <param name="lookValue">Look input.</param>
    public void RotateCharacter(Transform characterTransform, Transform weaponHolder, Transform cameraTransform, Vector2 lookValue)
    {
        float mouseX = lookValue.x * lookSpeed;
        float mouseY = lookValue.y * lookSpeed;

        characterTransform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        weaponHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    /// <summary>
    /// Requests the current weapon to fire.
    /// </summary>
    /// <param name="weaponController">Weapon controller.</param>
    public void FireWeapon(WeaponController weaponController)
    {
        Debug.Log("firing");
        weaponController.RequestWeaponFire();
    }

    /// <summary>
    /// Requests the current weapon to reload.
    /// </summary>
    /// <param name="weaponController">Weapon controller.</param>
    public void ReloadWeapon(WeaponController weaponController)
    {
        Debug.Log("reloading");
        weaponController.RequestWeaponReload();
    }

    /// <summary>
    /// Requests a weapon change.
    /// </summary>
    /// <param name="weaponController">Weapon controller.</param>
    public void ChangeWeapon(WeaponController weaponController)
    {
        Debug.Log("switch weapon");
        weaponController.RequestWeaponChangeServerRpc();
    }
}