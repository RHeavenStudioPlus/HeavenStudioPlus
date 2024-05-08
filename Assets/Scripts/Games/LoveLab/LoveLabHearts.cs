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
        float playBackSpeed;
        public double intervalSpeed;
        public float _step;
        public bool tweenComplete;
        Tween movePos;
        public bool onlyOne = true;
        public Transform end;
        public double timer;

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
        {
            if(heartCount == 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 2f);
            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 4f);
            }
            
            playBackSpeed = Timeline.instance.PlaybackSpeed.value;
            addPos = 2.5f; //3f
        }

        void Start()
        {
            game = LoveLab.instance;
            cond = Conductor.instance;
            timeLine = Timeline.instance;
            origPos = transform.position;
            //Debug.LogWarning(testContainer);
            speedMultiplier = game.speedForHeartsMultiplier;
            //test = (((float)intervalSpeed / cond.GetBpmAtBeat(heartBeat)) * playBackSpeed);

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
            DOTween.timeScale = playBackSpeed;
            var a = (float)(length * cond.secPerBeat) / playBackSpeed;
            Debug.LogWarning("Length: " + length);
            Debug.LogWarning("Sec per beat: " + cond.secPerBeat);
            Debug.LogWarning("Playback: " + playBackSpeed);
            debugSmth<float>(ref a);
            endValue = transform.position.y + addPos;
            upTween = transform.DOMoveY(endValue, (float)((length * cond.secPerBeat) / playBackSpeed)).SetEase(Ease.OutBack).OnComplete(restartTween);

            //s
        }

        public void debugSmth<T>(ref T idk)
        {
            Debug.LogWarning(idk);
        }

        public void updateBeat()
        {
            //test = .2f * ((60 / cond.GetBpmAtBeat(heartBeat) * _step) * playBackSpeed);
        }

        public void continousUp()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f * playBackSpeed);
        }

        public void restartTween()
        {
            upTween.ChangeEndValue(endValue);
            upTween.Restart();
        }

        public Tweener upTween;
        public float endValue;

        void FixedUpdate()
        {
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
                // goUp((float)((intervalSpeed * cond.secPerBeat / cond.GetBpmAtBeat(heartBeat)) * 60) * playBackSpeed);
                //goUp((float)((length * cond.secPerBeat)) / playBackSpeed);    
                upTween.ChangeEndValue(endValue);
                var t = ((float)cond.secPerBeat * length) * (transform.position.y - origPos.y) / ((origPos.y + addPos) - origPos.y);
                upTween.Goto((float)t, true);
            }
            else if(whatHeartType == heartType.completeHeart)
            {
                transform.DOMoveY(-1f, (float)(cond.secPerBeat / intervalSpeed) * playBackSpeed).SetEase(Ease.InBack);
            }

            //transform.position = Vector3.Lerp(origPos, new Vector3(origPos.x, origPos.y + addPos), Mathf.SmoothStep(0, 1, testContainer));
        }

        public void goUp(float timer)
        {
            transform.DOMoveY(origPos.y + (addPos), timer).SetEase(Ease.OutBack);
        }

        public void goDown()
        {
            Debug.LogWarning("go down");
            //DOTween.timeScale = cond.secPerBeat * playBackSpeed;
            transform.DOMoveY(end.position.y, 2.25f).SetEase(Ease.OutBack);
            // sorting layer = 1020
        }
        

        public void destroyWhenDone()
        {
            Destroy(this.gameObject);
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

