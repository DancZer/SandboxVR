using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using EzySlice;
using UnityEngine.UI;
public class Axe : MonoBehaviour
{   
    public AudioClip[] ChopAudio;

    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    
    private Queue<float> SwingMomentum = new Queue<float>();
    private int SwingMomentumQueueSize = 10;

    public Text Label1;
    public Text Label2;
    public float ChopMomentumThreshold = 1500;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Update() {
        if(SwingMomentum.Count>SwingMomentumQueueSize){
            SwingMomentum.Dequeue();
        }
        var velocity = _rigidbody.velocity.magnitude;
        var momentum = velocity * _rigidbody.mass;

        Debug.Log($"Axe Momentum:{momentum}");

        SwingMomentum.Enqueue(momentum);
    }

    private void OnTriggerEnter(Collider other) {        
        Debug.Log($"OnTriggerEnter {other.gameObject.name}!");

        var damage = GetChopDamage();
        if(ChopMomentumThreshold < damage) {
            var shatterer = other.gameObject.GetComponent<TreeCutChunk>();

            if(shatterer != null){
                float remainedHealth = shatterer.CutChunk(damage);
                if(remainedHealth>=0){
                    Label2.text = $"Chunk Remaining Health: {remainedHealth:00.##}";
                }
                
                Debug.Log($"CutChunk {shatterer.gameObject.name}!");
            }else{
                Debug.Log($"TreeCutChunk not for {other.gameObject.name} found!");
            }

            PlayRandomChopAudio();
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
        Debug.Log($"Axe MomentumSum:{damageSum:00.##}");
        Label1.text = $"Last Chop Damage: {damageSum:00.##}";

        return damageSum;
    }

    private void PlayRandomChopAudio() {
        
        if(ChopAudio.Length == 0 || _audioSource.isPlaying) return;

        var index = Random.Range(0, ChopAudio.Length);

        Debug.Log($"PlayRandomChopAudio {index}!");

        _audioSource.PlayOneShot(ChopAudio[index]);
    }
}
