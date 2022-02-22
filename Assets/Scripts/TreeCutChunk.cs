using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCutChunk : MonoBehaviour
{   
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateChunk(Material mat){
        Debug.Log($"ActivateChunk {this.gameObject.name}!");
        GetComponent<MeshRenderer>().material = mat;
        _isActive = true;
    }

    public void CutChunk(){
        if(!_isActive) return;
        
        Debug.Log($"CutChunk {this.gameObject.name}!");
        this.gameObject.SetActive(false);
        GetComponentInParent<TreeCutAreaLogic>().CutNext();
    }
}
