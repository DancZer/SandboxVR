using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using EzySlice;

public class Axe : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"OnTriggerEnter {other.gameObject.name}!");

        var shatterer = other.gameObject.GetComponent<TreeCutChunk>();

        if(shatterer != null){
            shatterer.CutChunk();
            Debug.Log($"CutChunk {shatterer.gameObject.name}!");
        }else{
            Debug.Log($"TreeCutChunk not for {other.gameObject.name} found!");
        }
    }
}
