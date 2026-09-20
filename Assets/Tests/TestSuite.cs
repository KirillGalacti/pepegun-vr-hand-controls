using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using System.Reflection;

public class TestSuite
{
   /* private GameObject pepegunGameObject;
    private PepegunBarrel barrel;
    private Pepegun pepegun;

    private ParticleSystem particle;
    private ParticleSystemRenderer particleRenderer;

    private GameObject ammoGameObject;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable ammoInteractable;
    
    //для второго патрона в тестах с кулдауном
    private GameObject ammoSecondGameObject;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable ammoSecondInteractable;

    //Атрибут указывающий на то, что это метод вызывается перед каждым тестом
    [SetUp]
    public void Setup()
    {
        pepegunGameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Pepegun"));
        pepegun = pepegunGameObject.GetComponent<Pepegun>();
        barrel = pepegunGameObject.GetComponentInChildren<PepegunBarrel>();

        particle = pepegunGameObject.GetComponentInChildren<ParticleSystem>();
        particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
    }

    //Атрибут указывающий на то, что этот метод вызывается после выполнения каждого теста
    [TearDown]
    public void TearDown()
    {
        Object.Destroy(pepegunGameObject);
        Object.Destroy(ammoGameObject);
        Object.Destroy(ammoSecondGameObject);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.

    [UnityTest]
    public IEnumerator AmmoAddedInPepegun()
    {
        ammoGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        //фейкуем
        var args = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        barrel.OnAmmoAdded(args);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(PepegunBarrel.State.Loaded, barrel.CurrentState);
        Assert.AreEqual("CatAmmo", barrel.CurrentAmmoTag);

    }

    [UnityTest]
    public IEnumerator AmmoRemoveFromPepegun()
    {
        ammoGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        //фейкуем
        var argsAdded = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        barrel.OnAmmoAdded(argsAdded);

        var argsRemove = new SelectExitEventArgs{ };

        barrel.OnAmmoRemoved(argsRemove);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(PepegunBarrel.State.Unloaded, barrel.CurrentState);
        Assert.AreEqual(string.Empty, barrel.CurrentAmmoTag);
    }

    [UnityTest]
    public IEnumerator AmmoAddedInPepegunAndShooting()
    {
        ammoGameObject = 
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow")); 
        ammoInteractable = 
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        //фейкуем
        var argsAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        var argsShoot = new ActivateEventArgs 
        { 
            
        };

        barrel.OnAmmoAdded(argsAmmo);

        //string name = "OnActivated";
        //MethodInfo method = this.GetType().GetMethod(name);
        //method.Invoke(this, argsShoot);

        pepegun.SendMessage("OnActivated", argsShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(PepegunBarrel.State.Unloaded, barrel.CurrentState);
        Assert.AreEqual(string.Empty, barrel.CurrentAmmoTag);
    }

    [UnityTest]
    public IEnumerator RepeatShootAfterCooldown()
    {
        ammoGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        ammoSecondGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoSecondInteractable =
            ammoSecondGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        //фейкуем
        //делаем все для первого выстрела
        var argsFirstAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        var argsFirstShoot = new ActivateEventArgs
        {

        };

        barrel.OnAmmoAdded(argsFirstAmmo);

        pepegun.SendMessage("OnActivated", argsFirstShoot);

        //ждем кулдаун
        yield return new WaitForSeconds(0.6f);//кулдаун

        //стреляем второй раз
        var argsSecondAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoSecondInteractable
        };

        var argsSecondShoot = new ActivateEventArgs
        {

        };

        barrel.OnAmmoAdded(argsSecondAmmo);

        pepegun.SendMessage("OnActivated", argsSecondShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(PepegunBarrel.State.Unloaded, barrel.CurrentState);
        Assert.AreEqual(string.Empty, barrel.CurrentAmmoTag);
    }

    [UnityTest]
    public IEnumerator RepeatShootInCooldown()
    {
        ammoGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        ammoSecondGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoSecondInteractable =
            ammoSecondGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        //фейкуем
        //делаем все для первого выстрела
        var argsAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        var argsFirstShoot = new ActivateEventArgs
        {

        };

        barrel.OnAmmoAdded(argsAmmo);

        pepegun.SendMessage("OnActivated", argsFirstShoot);

        //ждем кулдаун
        yield return new WaitForSeconds(0.5f);//кулдаун

        //стреляем второй раз
        var argsSecondAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoSecondInteractable
        };

        var argsSecondShoot = new ActivateEventArgs
        {

        };

        barrel.OnAmmoAdded(argsSecondAmmo);

        pepegun.SendMessage("OnActivated", argsSecondShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(PepegunBarrel.State.Loaded, barrel.CurrentState);
        Assert.AreEqual("CatAmmo", barrel.CurrentAmmoTag);
    }

    [UnityTest]
    public IEnumerator GunDontShootWhenBarrelEmpty()
    {
        var argsShoot = new ActivateEventArgs { };

        pepegun.SendMessage("OnActivated", argsShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.IsFalse(particle.isPlaying);
    }

    [UnityTest]
    public IEnumerator GunShootWhenBarrelNoEmpty()
    {
        ammoGameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));
        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        var argsAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        var argsShoot = new ActivateEventArgs { };

        barrel.OnAmmoAdded(argsAmmo);
        pepegun.SendMessage("OnActivated", argsShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(particle.isPlaying);
    }

    [UnityTest]
    public IEnumerator CorrectMaterialOfTheParticlesByTag()
    {
        var catField = 
            typeof(Pepegun).GetField("_catParticles", BindingFlags.Instance | BindingFlags.NonPublic);
        var expectedCatMaterial = (Material)catField.GetValue(pepegun);

        ammoGameObject =
             Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletYellow"));

        ammoGameObject.tag = "CatAmmo";

        ammoInteractable =
            ammoGameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        var argsAmmo = new SelectEnterEventArgs
        {
            interactableObject = ammoInteractable
        };

        var argsShoot = new ActivateEventArgs { };

        barrel.OnAmmoAdded(argsAmmo);
        pepegun.SendMessage("OnActivated", argsShoot);

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(expectedCatMaterial, particleRenderer.sharedMaterial);
    }*/
}

