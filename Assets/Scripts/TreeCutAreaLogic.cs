using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCutAreaLogic : MonoBehaviour
{
    private List<TreeCutChunk> _treeCutChunkList = new List<TreeCutChunk>();
    private AudioSource _audioSource;
    private int _chunkIndex = 0;

    public Material ActiveMaterial;
    public GameObject LogTop;
    public GameObject LogBottom;

    public AudioClip[] CrackAudio;

    public AudioClip[] FallAudio;
    public float FallAudioDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _treeCutChunkList.AddRange(GetComponentsInChildren<TreeCutChunk>());
        _audioSource = GetComponent<AudioSource>();
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

        PlayRandomCrackAndFallAudio();
    }

    private void PlayRandomCrackAndFallAudio() {
        if(CrackAudio.Length != 0 ){
            var crackIndex = Random.Range(0, CrackAudio.Length);

            Debug.Log($"PlayRandomChopAudio crackIndex{crackIndex}!");

            _audioSource.PlayOneShot(CrackAudio[crackIndex]);
        }

        if(FallAudio.Length != 0 ){
            var fallIndex = Random.Range(0, FallAudio.Length);

            Debug.Log($"PlayRandomChopAudio fallIndex:{fallIndex}!");

            _audioSource.clip = FallAudio[fallIndex];
            _audioSource.PlayDelayed(FallAudioDelay);
        }
    }
}
