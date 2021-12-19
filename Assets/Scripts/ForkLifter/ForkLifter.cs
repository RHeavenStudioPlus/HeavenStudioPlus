using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkLifter : MonoBehaviour
{
    public static ForkLifter instance;

    GameManager GameManager;

    [Header("Objects")]
    public Animator handAnim;
    public GameObject flickedObject;
    public SpriteRenderer peaPreview;

    public Sprite[] peaSprites;
    public Sprite[] peaHitSprites;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager = GameManager.instance;
    }

    public void Flick(float beat, int type)
    {
        Jukebox.PlayOneShot("flick");
        handAnim.Play("Hand_Flick", 0, 0);
        GameObject fo = Instantiate(flickedObject);
        fo.GetComponent<Pea>().startBeat = beat;
        fo.GetComponent<Pea>().type = type;
        fo.SetActive(true);
    }
}
