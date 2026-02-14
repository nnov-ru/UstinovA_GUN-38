using System.Collections;
using UnityEngine;

public class Collect
{
    private AIMotion _controller;
    private Transform _closestSphere;
    public bool collecting;
    public Collect(AIMotion controller) 
    {  _controller = controller; }
    public void Enter(Transform closestSphere)
    {
        _closestSphere = closestSphere;
        collecting = false;
    }
    public void Update()
    {
        if (!collecting && _closestSphere != null)
        {
            _controller.StartCoroutine(CollectCurrentSphere());
        }
    }
    private IEnumerator CollectCurrentSphere()
    {
        collecting = true;
        _controller.agent.isStopped = true;
        if (_controller.animator != null)
        {
            _controller.SetAnimation("Collect");
            yield return new WaitForSeconds(_controller.animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return new WaitForSeconds(.7f);
        }
        if (_closestSphere != null)
        {
            _controller.availableSpheres.Remove(_closestSphere);
            _closestSphere.gameObject.SetActive(false);
        }
        collecting = false;
        _closestSphere = null;
        _controller.ChangeState(AIState.Idle);
    }
}
