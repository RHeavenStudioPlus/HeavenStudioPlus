using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;
namespace RhythmHeavenMania.Games.RhythmRally
{
    public class RhythmRally : Minigame
    {
        public Transform renderQuadTrans;

        public Transform cameraPos;

        public GameObject ball;
        public NaughtyBezierCurves.BezierCurve3D curve3D;

        public Animator playerAnim;
        public Animator opponentAnim;

        public GameEvent bop = new GameEvent();
        
        public static RhythmRally instance;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            var cam = GameCamera.instance.camera;
            var camHeight = 2f * cam.orthographicSize;
            var camWidth = camHeight * cam.aspect;
            renderQuadTrans.localScale = new Vector3(camWidth, camHeight, 1f);

            playerAnim.Play("Idle", 0, 0);
            opponentAnim.Play("Idle", 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            var cond = Conductor.instance;

            ball.transform.position = curve3D.GetPoint(Mathf.Clamp(cond.GetPositionFromBeat(0, 2f), 0, 1));
            ball.transform.GetChild(0).transform.position = new Vector3(ball.transform.position.x, -0.399f, ball.transform.position.z);

            if (cond.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (cond.songPositionInBeats >= bop.startBeat && cond.songPositionInBeats < bop.startBeat + bop.length)
                {
                    var playerState = playerAnim.GetCurrentAnimatorStateInfo(0);
                    if (playerAnim.IsAnimationNotPlaying() || playerState.IsName("Idle") || playerState.IsName("Beat"))
                        playerAnim.Play("Beat", 0, 0);

                    var opponentState = opponentAnim.GetCurrentAnimatorStateInfo(0);
                    if (opponentAnim.IsAnimationNotPlaying() || opponentState.IsName("Idle") || opponentState.IsName("Beat"))
                        opponentAnim.Play("Beat", 0, 0);
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.length = length;
            bop.startBeat = beat;
        }
    }
}