using UnityEngine;
using Framework.Core.Event;

public enum PlayerState
{
    Idle, Run
}

public class PlayerController : MonoBehaviour
{
    EventMachine _machine = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _machine.Update();
    }
}
