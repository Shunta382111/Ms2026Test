using System;
using Unity.Collections;
using Unity.Netcode;

namespace Network
{
    [Serializable]
    public class ClientID : INetworkSerializable, IEquatable<ClientID>
    {
        public ulong Value = 0;
        public static readonly ClientID Empty = new ClientID(0);
        public ClientID(ulong id) { this.Value = id; }

        public void NetworkSerialize<T>(BufferSerializer<T> serialize) where T : IReaderWriter
        {
            serialize.SerializeValue(ref Value);
        }

        /// <summary>
        /// 等比較
        /// </summary>
        public bool Equals(ClientID other) => Value == other.Value;

        public override string ToString() => Value.ToString();
    }



    [Serializable]
    public class ClientInfo : INetworkSerializable, IEquatable<ClientInfo>
    {
        public enum ConnectionState
        {
            Wait,
            Ready
        }

        /*--- フィールド ---*/

        public ClientID Id = ClientID.Empty;
        public FixedString64Bytes Name = new FixedString64Bytes();
        public ConnectionState State = ConnectionState.Wait;





        /*--- メソッド ---*/

        public ClientInfo() { }
        public ClientInfo(ClientID id)
        {
            Id = id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id.Value);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref State);
        }

        public bool Equals(ClientInfo other) => Id == other.Id;
    }
}
