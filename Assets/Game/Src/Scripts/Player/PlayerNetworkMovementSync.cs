using UnityEngine;
using Unity.Netcode;
using UnityEngine.PlayerLoop;
/// <summary>
/// synchronize the player position with the server, it have the authority on the client position avec performing input
/// </summary>
public class PlayerNetworkMovementSync : NetworkBehaviour
{
    /// <summary>
    /// characterController for movement simulation
    /// </summary>
    [SerializeField]
    private CharacterController simulateCharacterController;
    /// <summary>
    /// transform to simulate camera rotation
    /// </summary>
    [SerializeField]
    private Transform simulationCameraTransform;
    /// <summary>
    /// player input scriptable object containing the logic to move / rotate the controller
    /// </summary>
    [SerializeField]
    private PlayerInputScriptableObject playerInputScriptableObject;
    /// <summary>
    /// the playerController attached to the gameobject
    /// </summary>
    [SerializeField] PlayerController playerController;
    /// <summary>
    /// the size of the buffer
    /// </summary>
    private readonly static int BUFFER_SIZE = 1024;
    /// <summary>
    /// an array container the input
    /// </summary>
    private PlayerInputSerialization[] inputBuffer = new PlayerInputSerialization[BUFFER_SIZE];
    // ticks starts at 1 , the array starts at 0 -> we have to deduct 1 to keep the ticks and the array coherent
    // modulo 1024 is to avoid index out of bound exception 
    private int TickToIndex(int tick) => ((tick - 1) < 0 ? 0 : ( (tick-1)% 1024) );


    /// <summary>
    /// called when the player spawn
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;

        NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
    }

    /// <summary>
    /// called when the player despawn
    /// </summary>
    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
    }

    /// <summary>
    /// capture the move input and write it in the buffer
    /// </summary>
    private void CaptureAndSendInput()
    {
        inputBuffer.SetValue(new PlayerInputSerialization
        {
            movement = playerController.moveInput,
            rotation = playerController.lookArroundInput,
            tick = NetworkManager.LocalTime.Tick
        }, TickToIndex(NetworkManager.LocalTime.Tick));

 
        //Debug.Log($"storing value {playerController.moveInput} at tick {NetworkManager.LocalTime.Tick} at index {TickToIndex(NetworkManager.LocalTime.Tick)}");
    }

    /// <summary>
    /// executed at each tick , if the owner execute the method it capture the player input and add it to the buffer
    /// if the server execute the method it simulate the movement at the tick and reconciliate with the client
    /// </summary>
    private void OnNetworkTick()
    {
        if (IsOwner)
        {
         //   CaptureAndSendInput();
        }

        if (IsServer)
        {
       //     SimulateMovementAndReconciliate();
        }
    }


    /// <summary>
    /// called at each tick
    /// </summary>
    private void SimulateMovementAndReconciliate()
    {
        int index = TickToIndex(NetworkManager.LocalTime.Tick);
        Vector2 moveValue = inputBuffer[index].movement;


        // Debug.Log($" Tick {NetworkManager.LocalTime.Tick} buffer value at index {index} : {positionBuffer[index].movement}");
        //playerInputScriptableObject.MoveCharacter(simulateCharacterController, inputBuffer[index].movement);
        //playerInputScriptableObject.RotateCharacter(simulateCharacterController.transform, simulationCameraTransform, inputBuffer[index].rotation);

        //ReconcilitationClientRpc(simulateCharacterController.transform.position, simulationCameraTransform.transform.rotation, NetworkManager.LocalTime.Tick);
    }


    /// <summary>
    /// Reconciles the client's predicted position with the server's authoritative position for a given tick.
    /// </summary>
    /// <param name="serverPosition">The authoritative position computed by the server.</param>
    /// <param name="serverTick">The tick this position corresponds to.</param>
    [Rpc(SendTo.Owner)]
    public void ReconcilitationClientRpc(Vector3 simulatedPosition, Quaternion simulatedRotation, int serverTick)
    {
        bool positionMismatch = Vector3.Distance(simulatedPosition, transform.position) > 1f;
        bool rotationMismatch = Quaternion.Angle(simulatedRotation, transform.rotation) > 1f;

     //   Debug.Log($"simulated rotation {simulatedRotation} real rotation {transform.rotation}");

        if (!positionMismatch && !rotationMismatch)
        {
            Debug.Log("client and server are in the same position");
            return;
        }

        transform.position = simulatedPosition;
     //   transform.rotation = simulatedRotation;

        int currentTick = NetworkManager.LocalTime.Tick;
        var characterController = playerController.GetComponent<CharacterController>();

        for (int t = serverTick + 1; t <= currentTick; t++)
        {
            int replayIndex = TickToIndex(t);
            Vector2 replayMove = inputBuffer[replayIndex].movement;
            Vector2 replayLook = inputBuffer[replayIndex].rotation;

            playerInputScriptableObject.RotateCharacter(transform, simulationCameraTransform, replayLook);
            playerInputScriptableObject.MoveCharacter(characterController, replayMove);
        }
    }
}
