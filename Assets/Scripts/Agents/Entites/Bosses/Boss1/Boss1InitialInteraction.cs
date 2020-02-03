using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1InitialInteraction : InteractionListener
{
    [SerializeField] private Animator _doorAnimator;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _doorCam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _bossRoomCam;

    private static bool _initialCutSeen;

    private Animator _bossAnimator;

    protected override void Start()
    {
        base.Start();
        _bossAnimator = GetComponent<Animator>();
    }

    protected override void InteractionFinishedAction()
    {
        _initialCutSeen = true;
    }

    protected override void InteractionStartedAction()
    {
        _bossAnimator.SetBool("InitialCutSeen", _initialCutSeen);
        _bossAnimator.SetTrigger("Interacted");
        Player.Instance.SetInteractionInputReading(false);
    }

    public void ToggleDoorCam()
    {
        _doorCam.enabled = !_doorCam.enabled;
    }

    public void CloseLeftDoor()
    {
        _doorAnimator.SetTrigger("Close");
    }

    // Called on interaction animation
    public void ToggleBossRoomCamera()
    {
        _bossRoomCam.enabled = !_bossRoomCam.enabled;
    }

    // Called on cutscene animation events 
    public void TriggerColliderOn()
    {
        GetComponentInChildren<Collider2D>().enabled = true;
    }

    // Called on cutscene animation events 
    public void TriggerCollideroff()
    {
        GetComponentInChildren<Collider2D>().enabled = false;
    }
}
