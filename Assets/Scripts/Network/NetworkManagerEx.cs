using UnityEngine;
using Framework.Core;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Framework;

[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerEx : Singleton<NetworkManagerEx>
{
    private UnityTransport _transport = null;
    [SerializeField] private ushort _port = 7777;
    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private uint _maxMember = 4;

    public void StartAsClient()
    {
        /*--- 基本設定 ---*/
        // クライアント
        _transport.SetConnectionData(_ip, _port);

        bool isSuccess = NetworkManager.Singleton.StartClient();
        if (!isSuccess)
        {
            DebugEx.LogError($"ホストへの接続に失敗しました");
            return;
        }

        bool isCapacityMember = NetworkManager.Singleton.ConnectedClientsIds.Count > _maxMember;
        if (isCapacityMember)
        {
            DebugEx.LogError($"接続人数が最大数を超えました: {_maxMember}");
            //NetworkManager.


        }
    }

    public void StartAsHost()
    {
        /*--- 基本設定 ---*/
        // ホスト
        _transport.ConnectionData.ServerListenAddress = "0.0.0.0";
        _transport.ConnectionData.Port = _port;

        bool isSuccess = NetworkManager.Singleton.StartHost();
        if (!isSuccess)
        {
            DebugEx.LogError("ホストの起動に失敗しました");
            return;
        }        
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent(out _transport))
        {
            _transport = gameObject.AddComponent<UnityTransport>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
