using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using EzySlice;

public class Sword : MonoBehaviour
{
    public GameObject Head = null;
     private XRGrabInteractable interactable = null; 

    private bool ReadyToCut = false;
    private bool IsCutDone = false;
    private void Awake() {
        interactable = GetComponent<XRGrabInteractable>();   
    }

    private void OnEnable() {
        interactable.activated.AddListener(ReadyToCutActivated);
        interactable.deactivated.AddListener(ReadyToCutDeactivated);
    }

    private void OnDisable() {
        interactable.activated.RemoveListener(ReadyToCutActivated);
        interactable.deactivated.AddListener(ReadyToCutDeactivated);
    }

    private void ReadyToCutActivated(ActivateEventArgs eventArgs){
        ReadyToCut = true;
    }
    private void ReadyToCutDeactivated(DeactivateEventArgs eventArgs){
        IsCutDone = ReadyToCut = false;
    }

    private void Update() {
        DrawPlane(Head.transform.position, Head.transform.up, 0); //TODO get plane normal normal
    }

     void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 2);
        }

        if(!ReadyToCut || IsCutDone) return;

        var shatterer = collision.gameObject.GetComponent<RuntimeShatterExample>();

        if(shatterer != null){
            shatterer.CutByPlane(Head.transform.position, Head.transform.up);
            Debug.Log($"CutByPlane {collision.gameObject.name}!");
            DrawPlane(Head.transform.position, Head.transform.up);
            IsCutDone = true;
        }else{
            Debug.Log($"RuntimeShatterExample not for {collision.gameObject.name} found!");
        }
    }

    private void DrawPlane(Vector3 position,  Vector3 normal, float duration = 10f) {

        Vector3 v3;
    
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
            
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.green, duration);
        Debug.DrawLine(corner1, corner3, Color.green, duration);
        Debug.DrawLine(corner0, corner1, Color.green, duration);
        Debug.DrawLine(corner1, corner2, Color.green, duration);
        Debug.DrawLine(corner2, corner3, Color.green, duration);
        Debug.DrawLine(corner3, corner0, Color.green, duration);
        Debug.DrawRay(position, normal, Color.red, duration);
    }
}
