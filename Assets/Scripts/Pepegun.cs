using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pepegun : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [Header("Pepegun properties")]
    [SerializeField] private PepegunBarrel _barrel;

    [Header("Particles systems ammo")]
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Material _monkeyParticles;
    [SerializeField] private Material _catParticles;
    [SerializeField] private Material _dogParticles;
    [SerializeField] private Material _guzeParticles;
    [SerializeField] private Material _pepeParticles;

    private bool _isGrabbed = false;
    private bool _onCooldown = false;

    //protected позволяет производным классам изменять и использовать защищённые типы, но скрывает их от других, не связанных скриптов.
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _isGrabbed = true;
        //PlayerLogger.Message("[Pepegun] Grabbed!");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        _isGrabbed = false;
       // PlayerLogger.Message("[Pepegun] Released!");
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Shoot();
    }

    private void Shoot()
    {
        if (_onCooldown)
        {
            //PlayerLogger.Message("[Pepegun] On cooldown!");
            return;
        }

        var ammoType = _barrel.CurrentAmmoType;
        _particleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = ammoType.ParticleMaterial;
        var _shape = _particleSystem.shape;
        _shape.shapeType = ammoType.ShapeType;

        GetParticlesSystemByTag(_barrel.CurrentAmmoTag);

        if (!_barrel.UseAmmo())
        {
            //PlayerLogger.Message("[Pepegun] Insufficient ammo!");
            return;
        }

        //PlayerLogger.Message("[Pepegun] Shooting!");
        StartCoroutine(ShootingRoutine());
    }

    private void GetParticlesSystemByTag(string tag)
    {
        var _renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
        var _shape = _particleSystem.shape;

        switch (tag)
        {
            case "MonkeyAmmo":
                _renderer.sharedMaterial = _monkeyParticles;

                _shape.shapeType = ParticleSystemShapeType.Hemisphere;
                break;
            case "CatAmmo":
                _renderer.sharedMaterial = _catParticles;

                _shape.shapeType = ParticleSystemShapeType.Sphere;
                break;
            case "DogAmmo":
                _renderer.sharedMaterial = _dogParticles;

                _shape.shapeType = ParticleSystemShapeType.Circle;
                break;
            case "GuzeAmmo":
                _renderer.sharedMaterial = _guzeParticles;

                _shape.shapeType = ParticleSystemShapeType.Box;
                break;
            default:
                _renderer.sharedMaterial = _pepeParticles;
                _shape.shapeType = ParticleSystemShapeType.Cone;
                break;
        }
    }

    private IEnumerator ShootingRoutine()
    {
        _onCooldown = true;
        _particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        _particleSystem.Stop();
        _onCooldown = false;
    }
}
