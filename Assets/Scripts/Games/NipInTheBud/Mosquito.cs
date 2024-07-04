using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using NaughtyBezierCurves;

namespace HeavenStudio.Games.Scripts_NipInTheBud
{
public class Mosquito : MonoBehaviour
{
    
    [SerializeField] NipInTheBud game;

    public double startBeat;
    public double approachBeat;
    public double fleeBeat;

    public bool isStarting;
    public bool isApproaching;
    public bool isFleeing;

    public BezierCurve3D startCurve;
    public BezierCurve3D approachCurve;
    public BezierCurve3D fleeCurve;

    public SpriteRenderer body;
    public SpriteRenderer wingA;
    public SpriteRenderer wingB;

    public int bodySort = 0;
    public int wingSort = 1;

    [SerializeField] GameObject mosquitoStart;

    [Header("Animators")]
        [SerializeField] Animator mosquitoAnim;
        Animator Leilani;
    // Start is called before the first frame update
    private void Awake()
    {


    }
    
    void Start()
    {
        Leilani = game.Leilani;
        isStarting = true;
        game.ScheduleInput(startBeat, 2f, NipInTheBud.InputAction_FlickPress, Hit, Miss, null);
        approachBeat = startBeat+1;
    }



    // Update is called once per frame
    void Update()
    {
        body.sortingOrder = bodySort;
        wingA.sortingOrder = wingSort;
        wingB.sortingOrder = wingSort;

        if (isStarting)
        {
            float flyPosStart = game.conductor.GetPositionFromBeat(startBeat, 1f);
            transform.position = startCurve.GetPoint(flyPosStart);
            if (flyPosStart > 1f) {
                isStarting = false;
                isApproaching = true;
            }
        }
        else if (isApproaching)
        {
            
            float flyPosApproach = game.conductor.GetPositionFromBeat(approachBeat, 1f);
            transform.position = approachCurve.GetPoint(flyPosApproach);
            if (flyPosApproach == 1f) {
                bodySort = 1000;
                wingSort = 1001;
            }
            if (flyPosApproach > 3f) {Destroy(gameObject);}
            
        }
        else if (isFleeing)
        {
            float flyPosFlee = game.conductor.GetPositionFromBeat(fleeBeat, 1f);
            transform.position = fleeCurve.GetPoint(flyPosFlee);
            if (flyPosFlee > 1f)
            {Destroy(gameObject);}
        }

        
    }

    public void Hit (PlayerActionEvent caller, float state)
    {
        game.StopPrepare();
        if (state >= 1f || state <= -1f) {
            Leilani.DoScaledAnimationAsync("SnapMiss", 0.5f);
            SoundByte.PlayOneShotGame("nipInTheBud/barely");
            isApproaching = false;
            isFleeing = true;
            fleeBeat = game.conductor.songPositionInBeatsAsDouble;
            transform.Rotate(new Vector3 (0,180,0));

        }
        else {
            Leilani.DoScaledAnimationAsync("Snap", 0.5f);
            SoundByte.PlayOneShotGame("nipInTheBud/catch");
            Destroy(gameObject);

        }

        
    }

    public void Miss (PlayerActionEvent caller)
    {
        if (!game.preparing) return;
            Leilani.DoScaledAnimationAsync("Unprepare", 0.5f);
            game.StopPrepare();

    }
}
}
