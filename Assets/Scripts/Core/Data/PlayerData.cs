
using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString128Bytes playerName;
    public FixedString64Bytes playerId;
    public bool isMuted;
    
    public bool Equals(PlayerData other)
    {
        return 
            clientId == other.clientId && 
            colorId == other.colorId &&
            playerName == other.playerName &&
            playerId == other.playerId &&
            isMuted == other.isMuted;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref isMuted);
    }
}
