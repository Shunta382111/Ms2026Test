using System;
using Unity.Collections;
using Unity.Netcode;

namespace Network
{
    public class ClientID
    {
        public ulong id = 0;
    }

    [Serializable]
    public class ClientInfo : INetworkSerializable, IEquatable<ClientInfo>
    {
        public enum ConnectionState
        {
            Wait,
            Ready
        }

        public ulong id = 0;
        public FixedString64Bytes name = new FixedString64Bytes();
        public ConnectionState state = ConnectionState.Wait;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref state);
        }

        public bool Equals(ClientInfo other) => id == other.id;
    }
}
