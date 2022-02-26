using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlicker : MonoBehaviour
{

    public float flickerInterval;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        InvokeRepeating("ToggleVisibility", 0f, flickerInterval);
    }

    // Update is called once per frame
    void ToggleVisibility()
    {
        sr.enabled = !sr.enabled;
    }
}
