
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Network
{
    public class ClientContainer
    {
        private List<ClientInfo> _clients = new();

        public List<ClientInfo> Clients => _clients;


        /// <summary>
        /// クライアントの情報を登録する
        /// </summary>
        /// <param name="id"></param>
        public void Register(ClientID id)
        {
            if (Contains(id))
            {
                DebugEx.LogError($"ID は既に登録されています: {id}");
            }

            _clients.Add(new ClientInfo(id));
        }

        /// <summary>
        /// クライアントの情報の登録解除をする
        /// </summary>
        /// <param name="id"></param>
        public void Unregister(ClientID id)
        {
            if (!Contains(id))
            {
                DebugEx.LogError($"ID が登録されていません: {id}");
            }

            _clients.Remove(new ClientInfo(id));
        }


        public bool Contains(ClientID id)
        {
            bool isContains = false;

            _clients.ForEach((ClientInfo info) => isContains = info.Id == id);
            return isContains;
        }
    }
}
