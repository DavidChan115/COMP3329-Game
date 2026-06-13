using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // play intro music then follow by loop music

    public AudioSource introSource, loopSource;
    // Start is called before the first frame update
    void Start()
    {
        introSource.Play();
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
        // dspTime = current time in audio system components 
        // play loop source after (current time + length of introSource)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
