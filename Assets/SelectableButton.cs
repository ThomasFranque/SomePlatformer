using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] private AudioClip _selectedSound;
    
    private ControlledCanvasMenu _targetCanvasMenu;
    private void Awake()
    {
        _targetCanvasMenu = GetComponentInParent<ControlledCanvasMenu>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _targetCanvasMenu.PlaySound(_selectedSound);
    }
}
