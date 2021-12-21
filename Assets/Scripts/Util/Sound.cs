using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class Sound : MonoBehaviour
    {
        public AudioClip clip;
        public float pitch;

        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(clip);
            StartCoroutine(play());
        }

        private IEnumerator play()
        {
            yield return new WaitForSeconds(clip.length);
            Destroy(this.gameObject);
        }
    }
}
