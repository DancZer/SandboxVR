using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

/**
 * Represents a really badly written shatter script! use for reference purposes only.
 */
public class RuntimeShatterExample : MonoBehaviour {

    public Material crossSectionMaterial;

    public bool isShattered = false;

    public void CutByPlane(Vector3 worldPosition, Vector3 worldOrientation) {
        Debug.Log($"CutByPlane {this.gameObject.name} isShattered:{isShattered}!");
        if (!isShattered) {
            GameObject[] shatters = this.gameObject.SliceInstantiate(worldPosition, worldOrientation, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);

            if (shatters != null && shatters.Length > 0) {
                Debug.Log($"Shatters {shatters.Length}!");

                this.gameObject.SetActive(false);

                // add rigidbodies and colliders
                foreach (GameObject shatteredObject in shatters) {
                    shatteredObject.AddComponent<MeshCollider>().convex = true;
                    shatteredObject.AddComponent<Rigidbody>();
                    var shatterer = shatteredObject.AddComponent<RuntimeShatterExample>();
                    shatterer.crossSectionMaterial = crossSectionMaterial;
                }

                isShattered = true;
            }
        }
    }
}
