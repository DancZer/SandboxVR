using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float LaunchForce = 10f;
    public float DestroyTime = 5f;
    private Rigidbody rigidbody = null;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(){
        rigidbody.AddRelativeForce(Vector3.forward*LaunchForce, ForceMode.Impulse);
        Destroy(gameObject, DestroyTime);
    }
}
