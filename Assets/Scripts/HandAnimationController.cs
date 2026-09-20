using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public struct HandAnimationData
{
    public readonly List<Finger> Fingers;//ÚËÔ Í‡Ê‰Ó„Ó Ô‡Î¸ˆ‡
    public readonly List<float> TargetValues;

    public HandAnimationData(List<Finger> fingers, List<float> targetValues)
    {
        Fingers = fingers;
        TargetValues = targetValues;
    }
}

public class Finger
{
    public FingerType Type;
    public float BlendTargetValue = 0.0f;
    public float BlendCurrentValue = 0.0f;
    public Finger(FingerType type)
    {
        Type = type;
    }
}

public enum FingerType { Thumb, Index, Middle, Ring, Pinky }

public class HandAnimationController : MonoBehaviour
{
    [SerializeField] private XRBaseInputInteractor[] _interactors;
    [SerializeField] private GameObject _handModel;
    [SerializeField] private float _animationSpeed = 10.0f;
    [SerializeField] private Animator _animator;
    [SerializeField] private InputActionReference _thumbTouchedAction;
    [SerializeField] private InputActionReference _triggerAction;
    [SerializeField] private InputActionReference _gripAction;
    [SerializeField] private InputActionReference _otherButtonAction;

    private HandAnimationData _thumbTouched;
    private HandAnimationData _triggerPressed;
    private HandAnimationData _gripPressed;
    private HandAnimationData _otherButtonPressed;

    private float _currentTriggerValue;
    private float _currentGripValue;

    private void Awake()
    {
        InitializeHandAnimations();
    }

    private void InitializeHandAnimations()
    {
        // thumbstick touched
        _thumbTouched = new HandAnimationData(
             fingers:
             new List<Finger>() 
             { 
                 new Finger(FingerType.Thumb) 
             },
                targetValues: new List<float> 
                { 
                    0.7f 
                }
        );

        // trigger pressed
        _triggerPressed = new HandAnimationData(
             fingers: new List<Finger>() 
             { 
                 new Finger(FingerType.Index) 
             },
                targetValues: new List<float> 
                { 
                    1.0f 
                }
        );

        // grip pressed
        _gripPressed = new HandAnimationData(
             fingers: new List<Finger>() 
             { 
                 new Finger(FingerType.Middle), 
                 new Finger(FingerType.Ring), 
                 new Finger(FingerType.Pinky) 
             },
                targetValues: new List<float> 
                { 
                    0.9f, 
                    0.9f, 
                    0.9f 
                }
        );

        //¬”À ¿Õ— »… —¿Àﬁ“ other pressed
        _otherButtonPressed = new HandAnimationData(
            fingers: new List<Finger>()
            {
                new Finger(FingerType.Thumb),
                new Finger(FingerType.Index),
                new Finger(FingerType.Middle),
                new Finger(FingerType.Ring),
                new Finger(FingerType.Pinky)
            },
                targetValues: new List<float>
                {
                    0.2f, // Thumb
                    0.2f,  // Index
                    0.2f,  // Middle
                    0.2f,  // Ring
                    0.2f   // Pinky
                }
        );
    }

    private void OnEnable()
    {
        foreach (var interactor in _interactors)
        {
            interactor.selectEntered.AddListener(OnSelect);
            interactor.selectExited.AddListener(OnDeselect);
        }
    }

    private void OnDisable()
    {
        foreach (var interactor in _interactors)
        {
            interactor.selectEntered.RemoveListener(OnSelect);
            interactor.selectExited.RemoveListener(OnDeselect);
        }
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("[HAND_ANIMATION_CONTROLLER] grabbed!");
        HideHands(true);
    }

    private void OnDeselect(SelectExitEventArgs args)
    {
        HideHands(false);
    }

    private void HideHands(bool state)
    {
        _handModel.SetActive(!state);
    }

    private void Update()
    {
        // check values
        CheckThumb();
        CheckGrip();
        CheckTrigger();
        CheckOtherButtons();

        // get blended values
        SmoothFinger(_thumbTouched);
        SmoothFinger(_triggerPressed);
        SmoothFinger(_gripPressed);
        SmoothFinger(_otherButtonPressed);

        // apply finger animation
        AnimateFingers(_thumbTouched);
        AnimateFingers(_triggerPressed);
        AnimateFingers(_gripPressed);
        AnimateFingers(_otherButtonPressed);
    }

    private void CheckGrip()
    {
        if (_gripAction == null)
            return;

        _currentGripValue = _gripAction.action.ReadValue<float>();
        SetFingerTargetValues(_gripPressed, _currentGripValue);
    }

    private void CheckTrigger()
    {
        if (_triggerAction == null)
            return;

        _currentTriggerValue = _triggerAction.action.ReadValue<float>();
        SetFingerTargetValues(_triggerPressed, _currentTriggerValue);
    }

    private void CheckTrigger()
    {
        if (_triggerAction == null)
            return;

        float triggerValue = _triggerAction.action.ReadValue<float>();
        SetFingerTargetValues(_triggerPressed, triggerValue);
    }

    private void CheckOtherButtons()
    {
        if (_otherButtonAction == null)
            return;

        float otherValue = _otherButtonAction.action.ReadValue<float>();

        bool noTriggerOrGrip = _currentTriggerValue < 0.1f && _currentGripValue < 0.1f;

        float otherButtonValue = (otherValue > .5f && noTriggerOrGrip) ? 1.0f: 0.0f;

        SetFingerTargetValues(_otherButtonPressed, otherButtonValue);
    }

    private void SetFingerTargetValues(HandAnimationData handAnimation, float value)
    {
        for (int i = 0; i < handAnimation.Fingers.Count; i++)
            handAnimation.Fingers[i].BlendTargetValue = handAnimation.TargetValues[i] * value;
    }

    private void SmoothFinger(HandAnimationData handAnimation)
    {
        for (int i = 0; i < handAnimation.Fingers.Count; i++)
        {
            float time = _animationSpeed * Time.deltaTime;
            handAnimation.Fingers[i].BlendCurrentValue = Mathf.MoveTowards(handAnimation.Fingers[i].BlendCurrentValue, handAnimation.Fingers[i].BlendTargetValue, time);
        }
    }

    private void AnimateFingers(HandAnimationData handAnimation)
    {
        for (int i = 0; i < handAnimation.Fingers.Count; i++)
        {
            AnimateFinger(handAnimation.Fingers[i].Type.ToString(), handAnimation.Fingers[i].BlendCurrentValue);
        }
    }

    private void AnimateFinger(string finger, float blendValue)
    {
        _animator.SetFloat(finger, blendValue);
    }
}
