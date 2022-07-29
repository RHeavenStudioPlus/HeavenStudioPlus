using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_KarateMan
{
    public class KarateManPotNew : PlayerActionObject
    {
        public float startBeat;
        public ItemType type;
        public int path = 1;

        public GameObject Shadow;
        public GameObject ShadowInstance;

        public string awakeAnim;
        FlyStatus status = FlyStatus.Fly;

        public int comboId = -1;
        static int _lastCombo = -1;
        public static int LastCombo { get { return _lastCombo; } }
        public static int GetNewCombo() { _lastCombo++; return _lastCombo; }
        public static void ResetLastCombo() { _lastCombo = -1; }

        public enum ItemType {
            Pot,        // path 1
            Bulb,       // path 1
            Rock,       // path 1
            Ball,       // path 1
            Cooking,    // path 1
            Alien,      // path 1
            TacoBell,   // path 1

            KickBarrel, // path 1
            KickBomb,   // no path

            ComboPot1,  // path 1
            ComboPot2,  // path 1
            ComboPot3,  // path 2
            ComboPot4,  // path 3
            ComboPot5,  // path 4
            ComboBarrel // path 5
        }

        public enum FlyStatus {
            Fly,
            Hit,
            NG,
            HitWeak
        }

        //pot trajectory stuff
        public Transform[] HitPosition;
        public float[] HitPositionOffset;
        public Vector3[] StartPositionOffset;
        public float[] ItemSlipRt;

        float ProgressToHitPosition(float progress) {
            return progress + (HitPositionOffset[path] - 0.5f);
        }

        Vector3 ProgressToFlyPosition()
        {
            var cond = Conductor.instance;
            float progress = Mathf.Min(cond.GetPositionFromBeat(startBeat, 2f), 1f - ItemSlipRt[path]);
            float progressToHitPosition = ProgressToHitPosition(progress);

            Vector3 hitPosition = HitPosition[path].position;

            //https://www.desmos.com/calculator/ycn9v62i4f
            float offset = HitPositionOffset[path];
            float flyHeight = (progressToHitPosition*(progressToHitPosition-1f))/(offset*(offset-1f));
            float floorHeight = HitPosition[0].position.y;

            Vector3 startPosition = hitPosition + StartPositionOffset[path];
            Vector3 endPosition = hitPosition - StartPositionOffset[path];
            Vector3 flyPosition = new Vector3(
                Mathf.Lerp(startPosition.x, endPosition.x, progress),
                floorHeight + (HitPosition[path].position.y - floorHeight) * flyHeight,
                Mathf.Lerp(startPosition.z, endPosition.z, progress)
            );

            if (progress >= 0.5f && flyPosition.y < HitPosition[0].position.y) {
                flyPosition.y = floorHeight;
            }
            return flyPosition;
        }

        void Awake()
        {
            switch (type)
            {
                case ItemType.ComboPot1:
                    KarateManNew.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_ALT_DOWN, ComboStartJustOrNg, ComboStartThrough, ComboStartOut);
                    path = 1;
                    break;
                case ItemType.ComboPot2:
                    path = 1;
                    BeatAction.New(gameObject, new List<BeatAction.Action>() { new BeatAction.Action(startBeat + 1f, delegate { JoeComboSequence(); }) });
                    break;
                case ItemType.ComboPot3:
                    path = 2;
                    BeatAction.New(gameObject, new List<BeatAction.Action>() { new BeatAction.Action(startBeat + 1f, delegate { JoeComboSequence(); }) });
                    break;
                case ItemType.ComboPot4:
                    path = 3;
                    //if the button isn't held anymore make Joe spin
                    BeatAction.New(gameObject, new List<BeatAction.Action>() { new BeatAction.Action(startBeat + 1f, delegate { JoeComboSequence(); }) });
                    break;
                case ItemType.ComboPot5:
                    path = 4;
                    BeatAction.New(gameObject, new List<BeatAction.Action>() { new BeatAction.Action(startBeat + 1f, delegate { JoeComboSequence(); }) });
                    break;
                case ItemType.ComboBarrel:
                    KarateManNew.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_ALT_UP, ComboEndJustOrNg, ComboEndThrough, ComboEndOut);
                    path = 5;
                    //check for button release
                    break;
                default:
                    KarateManNew.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_DOWN | InputType.DIRECTION_DOWN, ItemJustOrNg, ItemThrough, ItemOut);
                    path = 1;
                    comboId = -1;
                    break;
            }

            float floorHeight = HitPosition[0].position.y;
            transform.position = ProgressToFlyPosition();

            Animator mobjAnim = GetComponent<Animator>();
            mobjAnim.Play(awakeAnim, -1, 0);
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-360f * Time.deltaTime) + UnityEngine.Random.Range(0f, 360f));

            ShadowInstance = GameObject.Instantiate(Shadow, KarateManNew.instance.ItemHolder);
            ShadowInstance.SetActive(true);
            ShadowInstance.transform.position = new Vector3(transform.position.x, floorHeight - 0.5f, transform.position.z);
        }

        void Update()
        {
            var cond = Conductor.instance;
            float floorHeight = HitPosition[0].position.y;
            ShadowInstance.transform.position = new Vector3(transform.position.x, floorHeight - 0.5f, transform.position.z);
            switch (status)
            {
                case FlyStatus.Fly:
                    float prog = cond.GetPositionFromBeat(startBeat, 2f);
                    transform.position = ProgressToFlyPosition();
                    if (prog >= 2f) {
                        GameObject.Destroy(ShadowInstance.gameObject);
                        GameObject.Destroy(gameObject);
                        return;
                    }
                    else if (prog < 1f - ItemSlipRt[path]) {
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (90f * Time.deltaTime * (1/cond.pitchedSecPerBeat)));
                    }
                    break;
                case FlyStatus.Hit:
                    //TEMPORARY
                    GameObject.Destroy(ShadowInstance.gameObject);
                    GameObject.Destroy(gameObject);
                    return;
                case FlyStatus.NG:
                    //TEMPORARY
                    GameObject.Destroy(ShadowInstance.gameObject);
                    GameObject.Destroy(gameObject);
                    return;
                case FlyStatus.HitWeak:
                    break;
            }
        }

        void JoeComboSequence()
        {
            var joe = KarateManNew.instance.Joe;
            if (joe.GetShouldComboId() != comboId || !joe.inCombo) return;
            switch (type)
            {
                case ItemType.ComboPot2:
                    joe.Punch(2);
                    if (joe.GetComboId() != comboId)
                        Jukebox.PlayOneShotGame("karateman/swingNoHit_Alt", forcePlay: true);
                    else
                    {
                        Jukebox.PlayOneShotGame("karateman/comboHit1", forcePlay: true);
                        status = FlyStatus.Hit;
                    }
                    break;
                case ItemType.ComboPot3:
                    joe.ComboSequence(0);
                    if (joe.GetComboId() != comboId) {}
                    else
                    {
                        Jukebox.PlayOneShotGame("karateman/comboHit2", forcePlay: true);
                        status = FlyStatus.Hit;
                    }
                    break;
                case ItemType.ComboPot4:
                    //if the button isn't held anymore make Joe spin
                    if (joe.GetComboId() != comboId) {
                        joe.ComboMiss(startBeat + 1f);
                        Jukebox.PlayOneShotGame("karateman/comboMiss", forcePlay: true);
                        joe.SetShouldComboId(-2);
                    }
                    else
                    {
                        joe.ComboSequence(1);
                        Jukebox.PlayOneShotGame("karateman/comboHit3", forcePlay: true);
                        status = FlyStatus.Hit;
                    }
                    break;
                case ItemType.ComboPot5:
                    joe.ComboSequence(2);
                    if (joe.GetComboId() != comboId) {}
                    else
                    {
                        Jukebox.PlayOneShotGame("karateman/comboHit3", forcePlay: true);
                        status = FlyStatus.Hit;
                    }
                    break;
                default:
                    break;
            }
        }

        public void ItemJustOrNg(PlayerActionEvent caller, float state)
        {
            if (status == FlyStatus.Fly) {
                KarateManNew.instance.Joe.Punch();
                if (state <= -1f || state >= 1f) {
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    Jukebox.PlayOneShotGame("karateman/potHit", forcePlay: true);
                    status = FlyStatus.Hit;
                }
            }
        }

        public void ItemWrongAction(PlayerActionEvent caller, float state)
        { 
            //hitting a normal object with the alt input
        }

        public void ItemOut(PlayerActionEvent caller) {}

        public void ItemThrough(PlayerActionEvent caller)
        {
            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 2f, delegate { 
                    //TODO: play miss sound
                    //deduct flow if applicable
                })
            });
        }

        public void ComboStartJustOrNg(PlayerActionEvent caller, float state)
        {
            var joe = KarateManNew.instance.Joe;
            if (status == FlyStatus.Fly && !joe.inCombo) {
                joe.inCombo = true;
                joe.Punch(1);
                joe.SetComboId(comboId);
                joe.SetShouldComboId(comboId);
                if (state <= -1f || state >= 1f) {
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    Jukebox.PlayOneShotGame("karateman/comboHit1", forcePlay: true);
                    status = FlyStatus.Hit;
                }
            }
        }

        public void ComboStartOut(PlayerActionEvent caller) {}
        public void ComboStartThrough(PlayerActionEvent caller) {}

        public void ComboStartWrongAction(PlayerActionEvent caller, float state)
        { 
            //hitting a combo start object with the normal input
        }

        public void ComboEndJustOrNg(PlayerActionEvent caller, float state)
        {
            var joe = KarateManNew.instance.Joe;
            if (status == FlyStatus.Fly && joe.inCombo && joe.GetComboId() == comboId) {
                joe.inCombo = false;
                joe.SetComboId(-1);
                joe.SetShouldComboId(-1);
                //UpperCut
                if (state <= -1f || state >= 1f) {
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    Jukebox.PlayOneShotGame("karateman/comboHit4", forcePlay: true);
                    status = FlyStatus.Hit;
                }
            }
        }

        public void ComboEndOut(PlayerActionEvent caller) {}
        public void ComboEndThrough(PlayerActionEvent caller) {}
    }
}