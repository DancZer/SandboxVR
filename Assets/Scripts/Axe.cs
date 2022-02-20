using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using EzySlice;

public class Axe : MonoBehaviour
{
    public GameObject Head = null;
     private XRGrabInteractable interactable = null; 

    private bool IsCutDone = false;
    private void Awake() {
        interactable = GetComponent<XRGrabInteractable>();   
    }

    private void OnEnable() {
        interactable.activated.AddListener(ResetCut);
    }

    private void OnDisable() {
        interactable.activated.RemoveListener(ResetCut);
    }

    private void ResetCut(ActivateEventArgs eventArgs){
        IsCutDone = false;
    }

    private void Update() {
        
    }

     void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 2);
        }

        if(IsCutDone) return;
    }
}
