using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCutAreaLogic : MonoBehaviour
{
    private List<TreeCutChunk> _treeCutChunkList = new List<TreeCutChunk>();
    private int _chunkIndex = 0;

    public Material ActiveMaterial;
    public GameObject LogTop;
    public GameObject LogBottom;

    // Start is called before the first frame update
    void Start()
    {
        _treeCutChunkList.AddRange(GetComponentsInChildren<TreeCutChunk>());
        CutNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CutNext(){
        Debug.Log($"CutNext {this.gameObject.name} index: {_chunkIndex}!");
        if(_chunkIndex >= _treeCutChunkList.Count){
            ReleaseLog();
        }else{
            _treeCutChunkList[_chunkIndex].ActivateChunk(ActiveMaterial);
            _chunkIndex++;
        }
    }

    private void ReleaseLog(){
        Debug.Log($"ReleaseLog {this.gameObject.name}!");
        LogTop.GetComponent<Rigidbody>().isKinematic = false;
    }
}
