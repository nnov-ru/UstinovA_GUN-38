using UnityEngine;

public class Idle
{
    private AIMotion _controller;
    public Idle(AIMotion controller)
    {
        _controller = controller;
    }
    public void Update()
    {
        if (_controller.stateTimer >= _controller.idleTime)
        {
            _controller.ChangeState(AIState.Search);
        }
    }
}
