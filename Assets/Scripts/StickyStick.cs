using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StickyStick : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    private Queue<float> SwingMomentum = new Queue<float>();
    private int SwingMomentumQueueSize = 10;
    public float ChopMomentumThreshold = 300;
    public float BreakVelocity = 3000;
    public float BreakTorque = 1500;
    public Collider HeadCollider;

    private void Awake() {
        _meshRenderer= GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
    }

    private void OnDisable() {
    }
    private void Update() {
        if(SwingMomentum.Count>SwingMomentumQueueSize){
            SwingMomentum.Dequeue();
        }
        var velocity = _rigidbody.velocity.magnitude;
        var momentum = velocity * _rigidbody.mass;

        SwingMomentum.Enqueue(momentum);

        var hingeJoint = GetComponent<HingeJoint>();
     
        if(hingeJoint == null){
            _meshRenderer.material.color = Color.white;
        }else{
            _meshRenderer.material.color = Color.red;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"StickyStick OnCollisionEnter {collision.gameObject.name} {HeadCollider?.name} {collision.contacts[0].thisCollider.name} {collision.contacts[0].otherCollider.name}!");

        if(HeadCollider != null && collision.contacts[0].thisCollider != HeadCollider){
            return;
        }

        Debug.Log($"StickyStick OnCollisionEnter with Head {collision.gameObject.name}!");

        var damage = GetChopDamage();

        Debug.Log($"StickyStick damage {damage}!");

        if(damage > ChopMomentumThreshold){
            var hingeJoint = GetComponent<HingeJoint>();
            if(hingeJoint == null){
                hingeJoint = this.gameObject.AddComponent<HingeJoint>();
            }
            
            hingeJoint.connectedBody = collision.rigidbody;
            hingeJoint.breakForce = BreakVelocity;
            hingeJoint.breakTorque = BreakTorque;
            
            var contact = collision.GetContact(0);
            var globalContactPoint = contact.point;
            
            hingeJoint.autoConfigureConnectedAnchor = false;
            hingeJoint.anchor = this.transform.InverseTransformPoint(globalContactPoint);;
            hingeJoint.connectedAnchor = collision.rigidbody.transform.InverseTransformPoint(globalContactPoint);
            hingeJoint.axis = Vector3.up;

            hingeJoint.useLimits = true;
            var limits = hingeJoint.limits;

            Debug.Log($"StickyStick contactPoint:{globalContactPoint} {hingeJoint.anchor} at {hingeJoint.connectedAnchor}");
        }
    }

    private float GetChopDamage(){
       
        int idx = 0;
        float damageSum = 0;
        foreach (var momentum in SwingMomentum)
        {
            idx++;
            damageSum+=idx*momentum;
        }

        return damageSum;
    }
}
