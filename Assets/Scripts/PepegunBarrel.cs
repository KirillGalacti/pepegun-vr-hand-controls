using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))]
public class PepegunBarrel : MonoBehaviour
{
    public State CurrentState { get; private set; }
    public enum State { Unloaded, Loaded }

    public string CurrentAmmoTag { get; private set; }
    public AmmoType CurrentAmmoType { get; private set; }

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor _socket;
    private GameObject _ammoObject;

    private void Awake()
    {
        _socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        _ammoObject = null;
        CurrentState = State.Unloaded;
        CurrentAmmoTag = string.Empty;
    }

    private void OnEnable()
    {
        _socket.selectEntered.AddListener(OnAmmoAdded);
        _socket.selectExited.AddListener(OnAmmoRemoved);
    }

    private void OnDisable()
    {
        _socket.selectEntered.RemoveListener(OnAmmoAdded);
        _socket.selectExited.RemoveListener(OnAmmoRemoved);
    }

    public void OnAmmoAdded(SelectEnterEventArgs args)
    {
        //PlayerLogger.Message("Ammo added!");
        CurrentState = State.Loaded;
        _ammoObject = args.interactableObject.transform.gameObject;
        CurrentAmmoType = _ammoObject.GetComponent<AmmoType>();

        //Debug.Log($"Добавлен {_ammoObject.tag}");
    }

    public void OnAmmoRemoved(SelectExitEventArgs args)
    {
        //PlayerLogger.Message("Ammo removed!");
        CurrentState = State.Unloaded;
        _ammoObject = null;
        CurrentAmmoType = null;
    }

    public bool UseAmmo()
    {
        if (CurrentState == State.Unloaded)
            return false;

        Destroy(_ammoObject);
        _ammoObject = null;
        CurrentState = State.Unloaded;
        CurrentAmmoTag = string.Empty;
        return true;
    }
}
