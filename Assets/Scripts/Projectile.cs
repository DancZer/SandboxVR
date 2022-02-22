using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float LaunchForce = 10f;
    public float DestroyTime = 5f;
    private Rigidbody _rigidbody = null;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(){
        _rigidbody.AddRelativeForce(Vector3.forward*LaunchForce, ForceMode.Impulse);
        Destroy(gameObject, DestroyTime);
    }
}
