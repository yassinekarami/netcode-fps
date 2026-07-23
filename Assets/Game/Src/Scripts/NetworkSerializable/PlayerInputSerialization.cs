using Unity.Netcode;
using UnityEngine;

/// <summary>
/// structure holding the player's movement input and the associated tick
/// </summary>
public struct PlayerInputSerialization : INetworkSerializable
{
    /// <summary>
    /// movement input being read
    /// </summary>
    public Vector2 movement;
    /// <summary>
    /// rotation input being read
    /// </summary>
    public Vector3 rotation;
    /// <summary>
    /// the tick
    /// </summary>
    public int tick;

    /// <summary>
    /// serialize the struct properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref movement);
        serializer.SerializeValue(ref rotation);
        serializer.SerializeValue(ref tick);
    }
}