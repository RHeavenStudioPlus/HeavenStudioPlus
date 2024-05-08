using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using HeavenStudio.Editor.Track;

namespace HeavenStudio.Games.Scripts_LoveLab
{
    public class LoveLabHearts : MonoBehaviour
    {
        [SerializeField] heartType whatHeartType;
        public double prevHeartBeat;
        public double heartBeat;
        public double nextHeartBeat;
        public Animator heartAnim;
        [SerializeField] ParticleSystem death;
        [SerializeField] GameObject outline;
        [SerializeField] GameObject fill;
        LoveLab game;
        Conductor cond;
        Timeline timeLine;
        //public double counter;
        public bool stop;
        //float testContainer;
        //public float testSmth;
        //float speed;
        Vector3 origPos;
        public float addPos = 0;
        float speedMultiplier;
        public float startPos = 0f;
        public double length;
        double duration;
        double startTime;
        double endTime;
        public int heartCount;
        public bool hasChecked;
        public float playBackSpeed;
        public double intervalSpeed;
        public float _step;
        public bool tweenComplete;
        Tween movePos;
        public bool onlyOne = true;
        public Vector3 end;
        public double timer;
        public bool isWaiting = true;
        SpriteRenderer heartRenderer;
        public Vector3 dropStart;
        public double currentBeat;

        //y = 0 (endpoint)

        public int getHeartType()
        {
            return (int)whatHeartType;
        }

        enum heartType
        {
            guyHeart,
            girlHeart,
            completeHeart
        }

        private void Awake()
        {if (whatHeartType != heartType.completeHeart)
            {
            if(heartCount == 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 2f);
            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 4f);
            }
            }
            
            playBackSpeed = Timeline.instance.PlaybackSpeed.value;
            addPos = 2.5f; //3f
            heartRenderer = this.GetComponent<SpriteRenderer>();
            //dropStart = new Vector3 (transform.position.x, origPos.y+(addPos), transform.position.z);
            
        }

        void Start()
        {
            DOTween.SetTweensCapacity(2000, 100);
            game = LoveLab.instance;
            cond = Conductor.instance;
            timeLine = Timeline.instance;
            origPos = transform.position;
            //Debug.LogWarning(testContainer);
            speedMultiplier = game.speedForHeartsMultiplier;
            //end.position = new Vector2(-2.7f, -3f);
            //test = (((float)intervalSpeed / cond.GetBpmAtBeat(heartBeat)) * playBackSpeed);

            //var currentBeat = cond.songPositionInBeatsAsDouble;
            //if (whatHeartType == heartType.completeHeart)
            //{
            //    tempDestroy(currentBeat+1.25f); //temporary until I can get the drop to work -- Wook
            //}

            if(heartCount == 0)
            {
                _step = 1;
            }
            //test = .2f * ((60 / cond.GetBpmAtBeat(heartBeat) * _step) * playBackSpeed);
            //Debug.LogWarning(test);

            //DOTween.timeScale = 1f * playBackSpeed;
            //DOTween.timeScale = (float)((intervalSpeed / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed;
            //Debug.LogWarning("Sec per beat: " + cond.secPerBeat);

            //if (length <= .5f && onlyOne)
            //{
            //    addPos = 1.5f;
            //}
            //DOTween.timeScale = playBackSpeed;
            var a = (float)(length * cond.secPerBeat) / playBackSpeed;
            //Debug.LogWarning("Length: " + length);
            //Debug.LogWarning("Sec per beat: " + cond.secPerBeat);
            //Debug.LogWarning("Playback: " + playBackSpeed);
            //debugSmth<float>(ref a);
            endValue = transform.position.y + addPos;
            upTween = transform.DOMoveY(endValue, (float)((length * cond.secPerBeat) / playBackSpeed)).SetEase(Ease.OutBack);
            //.OnComplete(restartTween);

            //s
        }

        public void debugSmth<T>(ref T idk)
        {
            //Debug.LogWarning(idk);
        }

        public void startDrop()
        {
            
        }

        public void updateBeat()
        {
            //test = .2f * ((60 / cond.GetBpmAtBeat(heartBeat) * _step) * playBackSpeed);
        }

        public void continousUp()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f * playBackSpeed);
        }

        

        public Tweener upTween;
        public float endValue;

        void FixedUpdate()
        {
            currentBeat = cond.songPositionInBeatsAsDouble;

            if(!stop && whatHeartType != heartType.completeHeart)
            {
                //DOTween.timeScale = .7f * playBackSpeed;
                //DOTween.timeScale = (float)cond.secPerBeat * playBackSpeed;
                //transform.DOMoveY(origPos.y + (1 + game.guyHearts.Count * .5f), 2 * (float)((length / cond.GetBpmAtBeat(heartBeat)) * 60)).SetEase(Ease.OutBack);
                //transform.DOMoveY(origPos.y + (addPos), (float)((length / cond.GetBpmAtBeat(heartBeat)) * 60 )).SetEase(Ease.OutBack);
                //transform.position = new Vector3(transform.position.x, transform.position.y + .1f * playBackSpeed);
                //transform.position = new Vector3(transform.position.x , (transform.position.y + (float)(intervalSpeed / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed);
                //transform.DOMoveY(origPos.y + (addPos), (float)((intervalSpeed / cond.GetBpmAtBeat(heartBeat)) * 60)).SetEase(Ease.OutBack);
                //transform.DOMoveY(origPos.y + (addPos), (float)((intervalSpeed * cond.secPerBeat / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed).SetEase(Ease.OutBack);
                //transform.DOMoveY(origPos.y + (addPos), (float)((intervalSpeed * cond.secPerBeat / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed).SetEase(Ease.OutBack);
                goUp((float)((intervalSpeed * cond.secPerBeat / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed);
                //goUp((float)((length*cond.secPerBeat))/playBackSpeed);
                //DOTween.timeScale = playBackSpeed;
                //DOTween.timeScale = cond.pitchedSecPerBeat;
                //float normalizedBeat = Conductor.instance.GetPositionFromBeat(heartBeat, length);
                //if (normalizedBeat <= 1) goUp((float)(((length * cond.secPerBeat)) / playBackSpeed));    
                //upTween.ChangeEndValue(new Vector3(transform.position.x, endValue, transform.position.z));
                //var t = ((float)cond.secPerBeat * length) * (transform.position.y - origPos.y) / addPos;
                //upTween.Goto((float)t, true);
            }
            else if (!isWaiting)
            {

                float normalizedBeat = Conductor.instance.GetPositionFromBeat(heartBeat, ((timer/2)*playBackSpeed));
                
                float newPosY = EasingFunction.EaseInQuad(dropStart.y, end.y, normalizedBeat);

                if (normalizedBeat<=1) transform.position = new Vector2(dropStart.x, newPosY);
                else Destroy(this.gameObject);
                //else destroyWhenDone();

                //if (normalizedBeat >= 1) {destroyWhenDone();}
                //transform.position = Vector2.Lerp(dropStart, end.position, newPosY);
                //transform.position = new Vector2(transform.position.x, newPosY);

                //if (isWaiting)
                //{
                    
                    //transform.DOMoveY(transform.position.y, (float)(cond.secPerBeat / intervalSpeed) * playBackSpeed).SetEase(Ease.InBack);
                //}
                //else if (!isWaiting)
                //{
                    //DOTween.timeScale = cond.songPositionInBeats;
                    
                    //HandleHeartDrop( (float)timer);
                //}
                //transform.DOMoveY(transform.position.y, (float)(cond.secPerBeat / intervalSpeed) * playBackSpeed).SetEase(Ease.InBack);
                //goDown();
                
            }
            //else {transform.position = new Vector2(dropStart.x, dropStart.y);}

            //transform.position = Vector3.Lerp(origPos, new Vector3(origPos.x, origPos.y + addPos), Mathf.SmoothStep(0, 1, testContainer));
        }

        public void goUp(float timer)
        {
            
            transform.DOMoveY(origPos.y + (addPos), timer).SetEase(Ease.OutBack);
            //float newPosY = EasingFunction.EaseInQuad(origPos.y, (origPos.y+(addPos)), timer);
            //transform.position = new Vector2(transform.position.x, newPosY);
        }

        public void goDown()
        {
            //DOTween.timeScale = cond.pitchedSecPerBeat;
            //transform.DOLocalMoveY(end.position.y, (float)timer).From(true).SetEase(Ease.InBack);
            //Debug.LogWarning("go down");
            
            //DOTween.timeScale = playBackSpeed;
            //DOTween.timeScale = cond.secPerBeat * playBackSpeed;
            //double fallingTimeScale = cond.secPerBeat * playBackSpeed;
            
            //transform.DOMoveY(end.position.y, 2.25f).SetEase(Ease.InBack);
             //sorting layer = 1020
        }

        public void HandleHeartDrop(float fallDuration, double currentBeat)
        {

           


            //float dropTimeScale = length * cond.pitchedSecPerBeat;
            //float newYValue = Mathf.Lerp(dropStart.y, end.position.y, dropTimeScale);
            //transform.position = new Vector3 (transform.position.x, newYValue, transform.position.z);
            //heartRenderer.sortingOrder = 1020;
            //float dropTimeScale = (length/cond.secPerBeat)/playBackSpeed;
            //transform.DOMove (new Vector2 (transform.position.x, end.position.y), ((length*cond.pitchedSecPerBeat)), false).SetEase(Ease.InQuad);
            //if (transform.position.y == end.position.y) destroyWhenDone();

            //var currentBeat = cond.songPositionInBeatsAsDouble;
            //float currentBeatPosition = ((float)currentBeat*fallDuration);
            ////float newPosY = EasingFunction.EaseInSine(dropStart.y, end.position.y, currentBeatPosition);
            //transform.position = new Vector2(transform.position.x, newPosY);
            
        }
        

        public void destroyWhenDone()
        {
            Destroy(this.gameObject);
        }

        public void tempDestroy(double startBeat)
        {
            BeatAction.New(game, new List<BeatAction.Action>()
            {
            new BeatAction.Action(startBeat + 1f, delegate
                {Destroy(this.gameObject);}),});

        }

        public async void deadHeart()
        {
            //just instantiate this shit idk
            outline.SetActive(false);
            fill.SetActive(false);
            death.Play();

            await Task.Delay(500);
            destroyWhenDone();
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}

