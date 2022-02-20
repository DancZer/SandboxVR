using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Weapon : MonoBehaviour
{
    public Transform Barrel = null;
    public GameObject ProjectilePrefab = null;

    private XRGrabInteractable interactable = null; 

    private void Awake() {
        interactable = GetComponent<XRGrabInteractable>();    
    }

    private void OnEnable() {
        interactable.activated.AddListener(Fire);
    }

    private void OnDisable() {
        interactable.activated.RemoveListener(Fire);    
    }

    private void Fire(ActivateEventArgs eventArgs){
        CreateProjectile();
    }

    private void CreateProjectile(){
        GameObject projectileObj = Instantiate(ProjectilePrefab, Barrel.position, Barrel.rotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();

        projectile.Launch();
    }
}
