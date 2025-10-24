using Framework.Attribute;
using Network;
using Unity.Netcode;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button()]
    public void StartAsServer() => GetComponent<NetworkController>()?.StartAsServer();
    [Button()]
    public void StartAsClient() => GetComponent<NetworkController>()?.StartAsClient();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
