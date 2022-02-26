using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCutChunk : MonoBehaviour
{   
    private bool _isActive = false;

    public float Health = 3000;

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

    public float CutChunk(float damage){
        if(!_isActive) return -1;

        Health-=damage;

        if(Health<=0){
            Health = 0;
            Debug.Log($"CutChunk {this.gameObject.name}!");
            this.gameObject.SetActive(false);
            GetComponentInParent<TreeCutAreaLogic>().CutNext();
        }

        return Health;
    }
}
