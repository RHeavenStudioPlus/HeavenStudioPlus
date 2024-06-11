using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using NaughtyBezierCurves;

namespace HeavenStudio.Games.Scripts_NipInTheBud
{
public class Mayfly : MonoBehaviour
{
    [SerializeField] NipInTheBud game;

    public double startBeat;
    public double approachBeat;
    public double fleeBeat;
    public double exitBeat;

    public bool isStarting;
    public bool isApproaching;
    public bool isFleeing;
    public bool isExiting;

    public BezierCurve3D startCurve;
    public BezierCurve3D approachCurve;
    public BezierCurve3D fleeCurve;
    public BezierCurve3D exitCurve;

    [SerializeField] GameObject mayflyStart;

    public SpriteRenderer body;
    public SpriteRenderer wing;

    public int bodySort = 1000;
    public int wingSort = 1001;

    [Header("Animators")]
        [SerializeField] Animator mayflyAnim;
        Animator Leilani;
    // Start is called before the first frame update
    void Start()
    {
        Leilani = game.Leilani;
        isStarting = true;
        game.ScheduleInput(startBeat, 2f, NipInTheBud.InputAction_FlickPress, Hit, Miss, null);
        approachBeat = startBeat+1;
        exitBeat = startBeat+2;
        transform.Rotate(new Vector3 (0,180,0));
    }

    // Update is called once per frame
    void Update()
    {
        body.sortingOrder = bodySort;
        wing.sortingOrder = wingSort;

        if (isStarting)
        {
            float flyPosStart = game.conductor.GetPositionFromBeat(startBeat, 1f);
            transform.position = startCurve.GetPoint(flyPosStart);
            if (flyPosStart > 1f) {
                
                transform.Rotate(new Vector3 (0,180,0));
                bodySort = 50;
                wingSort = 51;
                isStarting = false;
                isApproaching = true;
            }
        }
        else if (isApproaching)
        {
            
            float flyPosApproach = game.conductor.GetPositionFromBeat(approachBeat, 1f);
            transform.position = approachCurve.GetPoint(flyPosApproach);
            if (flyPosApproach > 1f) {
                bodySort = 1000;
                wingSort = 1001;
                isApproaching = false;
                isExiting = true;
            }
            
        }
        else if (isFleeing)
        {
            float flyPosFlee = game.conductor.GetPositionFromBeat(fleeBeat, 1f);
            transform.position = fleeCurve.GetPoint(flyPosFlee);
            if (flyPosFlee > 1f)
            {Destroy(gameObject);}
        }
        else if (isExiting)
        {
            float flyPosExit = game.conductor.GetPositionFromBeat(exitBeat, 1f);
            transform.position = exitCurve.GetPoint(flyPosExit);
            if (flyPosExit > 1f)
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