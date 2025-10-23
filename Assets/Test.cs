using Framework.Attribute;
using Network;
using Unity.Netcode;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button()]
    public void StartAsServer() => GetComponent<NetworkManagerEx>()?.StartAsServer();
    [Button()]
    public void StartAsClient() => GetComponent<NetworkManagerEx>()?.StartAsClient();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
