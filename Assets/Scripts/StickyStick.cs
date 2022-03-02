using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StickyStick : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
     private XRGrabInteractable _interactable = null; 
    private Queue<float> SwingMomentum = new Queue<float>();
    private int SwingMomentumQueueSize = 10;
    public float ChopMomentumThreshold = 300;
    public float BreakVelocity = 3000;
    public float BreakTorque = 1500;
    public Collider HeadCollider = null;

    public bool UseLimit = true;
    public float HingeMoveRange = 20;

    public bool UseHinge = false;
    public float HingeSpring = 500;
    public float HingeDamper = 300;

    private void Awake() {
        _meshRenderer= GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _interactable = GetComponent<XRGrabInteractable>();   
    }

    private void OnEnable() {
        _interactable.selectExited.AddListener(SetupAfterSelectExited);
    }

    private void OnDisable() {
        _interactable.selectExited.RemoveListener(SetupAfterSelectExited);
    }

    private void SetupAfterSelectExited(SelectExitEventArgs args){

        var hingeJoint = GetComponent<HingeJoint>();
        
        _rigidbody.useGravity = hingeJoint == null;

        Debug.Log($"StickyStick SetupAfterSelectExited {args} useGravity {_rigidbody.useGravity}!");

        if(hingeJoint != null){
            var spring = hingeJoint.spring;
            spring.targetPosition = hingeJoint.angle;

            Debug.Log($"StickyStick SetupAfterSelectExited hingeJoint.angle {hingeJoint.angle}!");
            hingeJoint.spring = spring;
        }
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

        if(collision.rigidbody == null) return;
        
        var hingeJoint = GetComponent<HingeJoint>();
        if(hingeJoint != null) return;

        var damage = GetChopDamage();

        Debug.Log($"StickyStick damage {damage}!");

        if(damage > ChopMomentumThreshold){
            
            hingeJoint = this.gameObject.AddComponent<HingeJoint>();

            Debug.Log($"StickyStick create HingeJoint with RigidBody {collision.rigidbody.name}!");
            
            hingeJoint.connectedBody = collision.rigidbody;
            hingeJoint.breakForce = BreakVelocity;
            hingeJoint.breakTorque = BreakTorque;
            
            var contact = collision.GetContact(0);
            var globalContactPoint = contact.point;
            
            hingeJoint.autoConfigureConnectedAnchor = false;
            hingeJoint.anchor = this.transform.InverseTransformPoint(globalContactPoint);

            if(hingeJoint.connectedBody != null){
                hingeJoint.connectedAnchor = hingeJoint.connectedBody.transform.InverseTransformPoint(globalContactPoint);
            }
            hingeJoint.axis = Vector3.up;

            hingeJoint.useLimits = UseLimit;
            var limits = hingeJoint.limits;
            limits.min = -HingeMoveRange;
            limits.max = HingeMoveRange;
            hingeJoint.limits = limits;

            hingeJoint.useSpring = UseHinge;
            var spring = hingeJoint.spring;
            spring.spring = HingeSpring;
            spring.damper = HingeDamper;
            hingeJoint.spring = spring;

            Debug.Log($"StickyStick HingeJoint created with {collision.gameObject.name}");
        }
    }

    private void OnJointBreak(float breakForce)
    {
        _rigidbody.useGravity = true;

        Debug.Log($"StickyStick OnJointBreak {breakForce}");
        Debug.Log($"StickyStick rigidbody {_rigidbody} useGravity {_rigidbody.useGravity}");
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
