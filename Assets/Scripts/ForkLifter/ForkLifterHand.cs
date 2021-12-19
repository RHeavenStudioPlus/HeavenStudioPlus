using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForkLifterHand : MonoBehaviour
{
    public SpriteRenderer fastSprite;

    public Sprite[] fastSprites;

    List<GameManager.Event> allPlayerActions;

    public static ForkLifterHand instance { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public void CheckNextFlick()
    {
        allPlayerActions = GameManager.instance.Events.FindAll(c => c.eventName != "gulp" && c.eventName != "sigh" && c.eventName != "prepare");

        if (GameManager.instance.currentEventPlayer < allPlayerActions.Count)
        {
            switch (allPlayerActions[GameManager.instance.currentEventPlayer].eventName)
            {
                case "pea":
                    ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[0];
                    fastSprite.sprite = fastSprites[0];
                    break;
                case "topbun":
                    fastSprite.sprite = fastSprites[0];
                    ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[1];
                    break;
                case "burger":
                    fastSprite.sprite = fastSprites[1];
                    ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[2];
                    break;
                case "bottombun":
                    fastSprite.sprite = fastSprites[0];
                    ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[3];
                    break;
            }
        }
        else
        {
            ForkLifter.instance.peaPreview.sprite = null;
        }
    }

    public void Prepare()
    {
        Jukebox.PlayOneShot("flickPrepare");
        GetComponent<Animator>().Play("Hand_Prepare");
    }
}
