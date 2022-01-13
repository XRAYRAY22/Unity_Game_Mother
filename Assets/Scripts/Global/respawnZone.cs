using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawnZone : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPos;
    [SerializeField]
    private Transform player;
    private cCharacterMovement c;
    [SerializeField]
    private float yoffset;
    [SerializeField]
    private fadeOnDeath fadeOnDeath;
    [SerializeField]
    private float fadetime;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            fadeOnDeath.Fade(fadetime);
            Invoke("Respawn", fadetime);
            
        }

    }


    void Respawn(){
        fadeOnDeath.Reset(fadetime);
        player.transform.position = respawnPos.transform.position;
        player.transform.rotation = respawnPos.transform.rotation;
        Physics.SyncTransforms();
        
    }
}
