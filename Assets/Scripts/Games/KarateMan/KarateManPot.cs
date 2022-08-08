using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_KarateMan
{
    public class KarateManPot : PlayerActionObject
    {
        public float startBeat;
        public ItemType type;
        public int path = 1;

        public GameObject Shadow;
        public GameObject ShadowInstance;
        SpriteRenderer shadowRenderer;

        //hit effects
        public GameObject HitMark;
        public ParticleSystem[] HitParticles;

        public string awakeAnim;
        FlyStatus status = FlyStatus.Fly;
        Color effectTint = Color.white;

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
            ComboBarrel,// path 5

            CookingLid, //only used for hit
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
        public BezierCurve3D[] ItemCurves;
        public BezierCurve3D CurrentCurve;
        public float curveTargetBeat;

        public SpriteRenderer BulbLight;

        public void SetBulbColor(Color c) { 
            effectTint = c; 
            BulbLight.color = c;
        }

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
                floorHeight + (HitPosition[path].position.y - floorHeight + (StartPositionOffset[path].y * (1 - Mathf.Min(cond.GetPositionFromBeat(startBeat, 1f), 1f)))) * flyHeight,
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
                    KarateMan.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_ALT_DOWN, ComboStartJustOrNg, ComboStartThrough, ComboStartOut);
                    KarateMan.instance.ScheduleUserInput(startBeat, 1f, InputType.STANDARD_DOWN | InputType.DIRECTION_DOWN, ComboStartWrongAction, ComboStartOut, ComboStartOut);
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
                    //check for button release
                    KarateMan.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_ALT_UP, ComboEndJustOrNg, ComboEndThrough, ComboEndOut);
                    //button presses
                    KarateMan.instance.ScheduleUserInput(startBeat, 1f, InputType.STANDARD_DOWN | InputType.DIRECTION_DOWN, ComboEndWrongAction, ItemOut, ItemOut);
                    KarateMan.instance.ScheduleUserInput(startBeat, 1f, InputType.STANDARD_ALT_DOWN, ComboEndWrongActionAlt, ItemOut, ItemOut);
                    path = 5;
                    break;
                case ItemType.KickBarrel:
                    KarateMan.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_DOWN | InputType.DIRECTION_DOWN, KickChargeJustOrNg, ItemThrough, ItemOut);
                    KarateMan.instance.ScheduleUserInput(startBeat, 1f, InputType.STANDARD_ALT_DOWN, ItemWrongAction, ItemOut, ItemOut);
                    path = 1;
                    comboId = -1;
                    break;
                case ItemType.KickBomb:
                    KarateMan.instance.ScheduleInput(startBeat, 0.75f, InputType.STANDARD_UP | InputType.DIRECTION_UP, KickJustOrNg, KickThrough, KickOut);
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 2 * 0.75f;
                    path = 1;
                    comboId = -1;
                    break;
                case ItemType.CookingLid:
                    CurrentCurve = ItemCurves[9];
                    path = 1;
                    curveTargetBeat = 2f;
                    status = FlyStatus.Hit;
                    comboId = -1;
                    break;
                default:
                    KarateMan.instance.ScheduleInput(startBeat, 1f, InputType.STANDARD_DOWN | InputType.DIRECTION_DOWN, ItemJustOrNg, ItemThrough, ItemOut);
                    KarateMan.instance.ScheduleUserInput(startBeat, 1f, InputType.STANDARD_ALT_DOWN, ItemWrongAction, ItemOut, ItemOut);
                    path = 1;
                    comboId = -1;
                    break;
            }

            float floorHeight = HitPosition[0].position.y;
            if (CurrentCurve == null)
                transform.position = ProgressToFlyPosition();
            else
                transform.position = CurrentCurve.GetPoint(0f);

            Animator mobjAnim = GetComponent<Animator>();
            mobjAnim.Play(awakeAnim, -1, 0);
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-360f * Time.deltaTime) + UnityEngine.Random.Range(0f, 360f));

            ShadowInstance = GameObject.Instantiate(Shadow, KarateMan.instance.ItemHolder);
            shadowRenderer = ShadowInstance.GetComponent<SpriteRenderer>();
            shadowRenderer.color = KarateMan.instance.GetShadowColor();
            ShadowInstance.SetActive(true);
            ShadowInstance.transform.position = new Vector3(transform.position.x, floorHeight - 0.5f, transform.position.z);
        }

        void Update()
        {
            var cond = Conductor.instance;
            float floorHeight = HitPosition[0].position.y;
            switch (status)
            {
                case FlyStatus.Fly:
                    float prog = cond.GetPositionFromBeat(startBeat, 2f);
                    if (CurrentCurve == null)
                        transform.position = ProgressToFlyPosition();
                    else
                    {
                        prog = cond.GetPositionFromBeat(startBeat, curveTargetBeat);
                        transform.position = CurrentCurve.GetPoint(Mathf.Min(prog, 1f));
                    }

                    if (prog >= 2f || (type == ItemType.KickBomb && prog >= 1f)) {
                        if (type == ItemType.KickBomb)
                        {
                            ParticleSystem p = Instantiate(HitParticles[7], ItemCurves[6].GetPoint(1f), Quaternion.identity, KarateMan.instance.ItemHolder);
                            p.Play();
                        }
                        GameObject.Destroy(ShadowInstance.gameObject);
                        GameObject.Destroy(gameObject);
                        return;
                    }
                    else if (CurrentCurve == null && prog < 1f - ItemSlipRt[path]) {
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (125f * Time.deltaTime * (1/cond.pitchedSecPerBeat)));
                    }
                    else if (CurrentCurve != null && prog < 1f)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-90f * Time.deltaTime * (1/cond.pitchedSecPerBeat)));
                    }
                    break;
                case FlyStatus.Hit:
                    prog = cond.GetPositionFromBeat(startBeat, curveTargetBeat);
                    if (cond.songPositionInBeats >= startBeat + Mathf.Max(2f, curveTargetBeat) || CurrentCurve == null) {

                        if (type == ItemType.KickBomb)
                        {
                            ParticleSystem p = Instantiate(HitParticles[6], ItemCurves[7].GetPoint(1f), Quaternion.identity, KarateMan.instance.ItemHolder);
                            p.Play();
                        }

                        GameObject.Destroy(ShadowInstance.gameObject);
                        GameObject.Destroy(gameObject);
                        return;
                    }
                    else {
                        if (prog <= 1f)
                        {
                            transform.position = CurrentCurve.GetPoint(prog);
                            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-270f * Time.deltaTime * (1/cond.pitchedSecPerBeat)));
                        }
                        else
                        {
                            transform.position = CurrentCurve.GetPoint(1f);
                        }
                    }
                    break;
                case FlyStatus.NG:
                    prog = cond.GetPositionFromBeat(startBeat, curveTargetBeat);
                    if (cond.songPositionInBeats >= startBeat + Mathf.Max(2f, curveTargetBeat) || (type == ItemType.KickBomb && prog >= 1f) || CurrentCurve == null) {
                        if (type == ItemType.KickBomb)
                        {
                            ParticleSystem p = Instantiate(HitParticles[7], ItemCurves[8].GetPoint(1f), Quaternion.identity, KarateMan.instance.ItemHolder);
                            p.Play();
                        }
                        GameObject.Destroy(ShadowInstance.gameObject);
                        GameObject.Destroy(gameObject);
                        return;
                    }
                    else {
                        if (prog <= 1f)
                        {
                            transform.position = CurrentCurve.GetPoint(prog);
                            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-90f * Time.deltaTime * (1/cond.pitchedSecPerBeat)));
                        }
                        else
                        {
                            transform.position = CurrentCurve.GetPoint(1f);
                        }
                    }
                    break;
                case FlyStatus.HitWeak:
                    break;
            }
            ShadowInstance.transform.position = new Vector3(transform.position.x, floorHeight - 0.5f, transform.position.z);
            shadowRenderer.color = KarateMan.instance.GetShadowColor();
        }

        void CreateHitMark(bool useLocalPos = false)
        {
            GameObject hitMark = GameObject.Instantiate(HitMark, KarateMan.instance.ItemHolder);
            if (useLocalPos)
                hitMark.transform.localPosition = transform.position;
            else
                hitMark.transform.position = HitPosition[path].position;
            
            hitMark.SetActive(true);
        }

        //handles hitsound and particles
        void ItemHitEffect(bool straight = false)
        {
            ParticleSystem p;
            CreateHitMark(type == ItemType.KickBomb);
            switch (type)
            {
                case ItemType.Bulb:
                    CurrentCurve = ItemCurves[straight ? 1 : 0];
                    curveTargetBeat = straight ? 1f : 1.5f;;
                    Jukebox.PlayOneShotGame("karateman/lightbulbHit", forcePlay: true);
                    p = Instantiate(HitParticles[5], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);

                    if (effectTint.a == 0)
                        break;

                    //change gradient to match own colour
                    var col = p.colorOverLifetime;
                    col.enabled = true;
                    Gradient grad = new Gradient();
                    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 0.25f), new GradientColorKey(effectTint, 0.5f), new GradientColorKey(effectTint, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) });
                    col.color = grad;

                    var col2 = p.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().colorOverLifetime;
                    col2.enabled = true;
                    Gradient grad2 = new Gradient();
                    grad2.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(effectTint, 0.5f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
                    col2.color = grad2;

                    p.Play();
                    break;
                case ItemType.Rock:
                    CurrentCurve = ItemCurves[1];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/rockHit", forcePlay: true);
                    p = Instantiate(HitParticles[4], HitPosition[1].position, Quaternion.identity, KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.Ball:
                    CurrentCurve = ItemCurves[1];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/soccerHit", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.Cooking:
                    CurrentCurve = ItemCurves[1];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/cookingPot", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    KarateMan.instance.CreateItemInstance(startBeat + 1f, "Item09", ItemType.CookingLid);
                    GetComponent<Animator>().Play("Item08", -1, 0);
                    break;
                case ItemType.Alien:
                    CurrentCurve = ItemCurves[1];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/alienHit", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.TacoBell:
                    CurrentCurve = ItemCurves[1];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/rockHit", forcePlay: true);
                    Jukebox.PlayOneShotGame("karateman/tacobell", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboPot1:
                    CurrentCurve = ItemCurves[straight ? 1 : 0];
                    curveTargetBeat = 1.5f;
                    Jukebox.PlayOneShotGame("karateman/comboHit1", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboPot2:
                    CurrentCurve = ItemCurves[0];
                    curveTargetBeat = 1.5f;
                    Jukebox.PlayOneShotGame("karateman/comboHit1", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboPot3:
                    CurrentCurve = ItemCurves[2];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/comboHit2", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[2].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboPot4:
                    CurrentCurve = ItemCurves[3];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/comboHit3", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[3].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboPot5:
                    CurrentCurve = ItemCurves[4];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShotGame("karateman/comboHit3", forcePlay: true);
                    p = Instantiate(HitParticles[1], HitPosition[4].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.ComboBarrel:
                    Jukebox.PlayOneShotGame("karateman/comboHit4", forcePlay: true);
                    p = Instantiate(HitParticles[0], HitPosition[5].position, Quaternion.identity, KarateMan.instance.ItemHolder);
                    p.Play();
                    p = Instantiate(HitParticles[1], HitPosition[5].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.KickBarrel:
                    KarateMan.instance.CreateItemInstance(startBeat + 1f, "Item04", ItemType.KickBomb);
                    Jukebox.PlayOneShotGame("karateman/barrelBreak", forcePlay: true);
                    p = Instantiate(HitParticles[0], HitPosition[1].position, Quaternion.Euler(0, 0, -5f), KarateMan.instance.ItemHolder);
                    p.Play();
                    p = Instantiate(HitParticles[1], HitPosition[1].position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)), KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                case ItemType.KickBomb:
                    Jukebox.PlayOneShotGame("karateman/bombKick", forcePlay: true);
                    p = Instantiate(HitParticles[2], ItemCurves[6].GetPoint(0.5f), Quaternion.identity, KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
                default:
                    CurrentCurve = ItemCurves[straight ? 1 : 0];
                    curveTargetBeat = straight ? 1f : 1.5f;
                    Jukebox.PlayOneShotGame("karateman/potHit", forcePlay: true);
                    p = Instantiate(HitParticles[3], HitPosition[1].position, Quaternion.identity, KarateMan.instance.ItemHolder);
                    p.Play();
                    break;
            }

            startBeat = Conductor.instance.songPositionInBeats;
            status = FlyStatus.Hit;
        }

        int ItemPunchHand()
        {
            switch (type)
            {
                case ItemType.Rock:
                case ItemType.Ball:
                case ItemType.Cooking:
                case ItemType.Alien:
                case ItemType.TacoBell:
                case ItemType.KickBarrel:
                    return 2;
                default:
                    return 0;
            }
        }

        void JoeComboSequence()
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (joe.GetShouldComboId() != comboId || !joe.inCombo) return;
            switch (type)
            {
                case ItemType.ComboPot2:
                    joe.Punch(2);
                    if (joe.GetComboId() != comboId)
                        Jukebox.PlayOneShotGame("karateman/swingNoHit_Alt", forcePlay: true);
                    else
                    {
                        ItemHitEffect();
                    }
                    break;
                case ItemType.ComboPot3:
                    joe.ComboSequence(0);
                    if (joe.GetComboId() != comboId) {}
                    else
                    {
                        ItemHitEffect();
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
                        joe.lockedInCombo = true;
                        ItemHitEffect();
                    }
                    break;
                case ItemType.ComboPot5:
                    joe.ComboSequence(2);
                    if (joe.GetComboId() != comboId) {}
                    else
                    {
                        ItemHitEffect();
                    }
                    break;
                default:
                    break;
            }
        }

        public void ItemJustOrNg(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && !joe.inCombo) {
                bool straight = joe.Punch(ItemPunchHand());
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;

                    joe.SetFaceExpression((int) KarateMan.KarateManFaces.Sad);
                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 2f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        }),
                    });
                }
                else {
                    ItemHitEffect(straight);
                    status = FlyStatus.Hit;
                }
            }
        }

        public void ItemWrongAction(PlayerActionEvent caller, float state)
        { 
            if (GameManager.instance.currentGame != "karateman") return;
            //hitting a normal object with the alt input
            //WHEN SCORING THIS IS A MISS
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && !joe.inCombo) {
                joe.ForceFailCombo(Conductor.instance.songPositionInBeats);
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    ItemHitEffect();
                }

                BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(startBeat + 4f, delegate {
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Sad);
                    }),
                    new BeatAction.Action(startBeat + 6f, delegate {
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                    }),
                });
            }
        }

        public void ItemOut(PlayerActionEvent caller) {}

        public void ItemThrough(PlayerActionEvent caller)
        {
            var joe = KarateMan.instance.Joe;
            if (GameManager.instance.currentGame != "karateman") return;
            if (status != FlyStatus.Fly || gameObject == null) return;
            BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 2f, delegate { 
                    //TODO: play miss sound
                    //deduct flow if applicable
                    joe.SetFaceExpression((int) KarateMan.KarateManFaces.Surprise);
                }),
                new BeatAction.Action(startBeat + 6f, delegate {
                    if (joe.wantFace == -1)
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                }),
            });
        }

        public void ComboStartJustOrNg(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && !(joe.inKick || joe.wantKick || joe.inCombo)) {
                joe.inCombo = true;
                joe.Punch(1);
                joe.SetComboId(comboId);
                joe.SetShouldComboId(comboId);
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    ItemHitEffect();
                }
            }
        }

        public void ComboStartOut(PlayerActionEvent caller) {}
        public void ComboStartThrough(PlayerActionEvent caller) 
        {
            var joe = KarateMan.instance.Joe;
            if (GameManager.instance.currentGame != "karateman") return;
            if (status != FlyStatus.Fly || gameObject == null) return;
            BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 2f, delegate { 
                    //TODO: play miss sound
                    //deduct flow if applicable
                    joe.SetFaceExpression((int) KarateMan.KarateManFaces.Surprise);
                }),
                new BeatAction.Action(startBeat + 6f, delegate {
                    if (joe.wantFace == -1)
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                }),
            });
        }

        public void ComboStartWrongAction(PlayerActionEvent caller, float state)
        { 
            if (GameManager.instance.currentGame != "karateman") return;
            //hitting a combo start with the normal input
            //WHEN SCORING THIS IS A MISS
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && !joe.inCombo) {
                bool straight = joe.Punch(ItemPunchHand());
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;
                }
                else {
                    ItemHitEffect(straight);
                }
            }
        }

        public void ComboEndJustOrNg(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && joe.inCombo && joe.GetComboId() == comboId) {
                joe.inCombo = false;
                joe.SetComboId(-1);
                joe.SetShouldComboId(-1);
                joe.ComboSequence(3);
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[5];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;

                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 2f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Sad);
                        }),
                        new BeatAction.Action(startBeat + 6f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        }),
                    });
                }
                else {
                    ItemHitEffect();
                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 1.5f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Happy);
                        }),
                        new BeatAction.Action(startBeat + 3.5f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        })
                    });
                }
            }
        }

        public void ComboEndOut(PlayerActionEvent caller) {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && joe.inCombo && joe.GetComboId() == comboId && joe.comboWaiting)
            {
                joe.inCombo = false;
                joe.SetComboId(-1);
                joe.SetShouldComboId(-1);
                joe.ComboSequence(3);
                Jukebox.PlayOneShotGame("karateman/swingKick", forcePlay: true);
            }
        }

        public void ComboEndThrough(PlayerActionEvent caller) 
        {
            if (GameManager.instance.currentGame != "karateman") return;
            if (status != FlyStatus.Fly || gameObject == null) return;
            var joe = KarateMan.instance.Joe;
            if (joe.GetComboId() != comboId || !joe.inCombo)
            {
                BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(startBeat + 2f, delegate { 
                        //TODO: play miss sound
                        //deduct flow if applicable
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Surprise);
                    }),
                    new BeatAction.Action(startBeat + 6f, delegate {
                        if (joe.wantFace == -1)
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                    }),
                });
            }
            else
            {
                joe.SetFaceExpression((int) KarateMan.KarateManFaces.VerySad);
                BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(startBeat + 1.5f, delegate { 
                        joe.inCombo = false;
                        joe.SetComboId(-1);
                        joe.SetShouldComboId(-1);
                        joe.ComboSequence(4);
                    }),
                    new BeatAction.Action(startBeat + 2f, delegate { 
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.VerySad);
                    }),
                    new BeatAction.Action(startBeat + 5f, delegate { 
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                    })
                });
            }
        }

        public void ComboEndWrongAction(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            KarateMan.instance.Joe.Punch(1);
        }

        public void ComboEndWrongActionAlt(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            KarateMan.instance.Joe.ForceFailCombo(Conductor.instance.songPositionInBeats);
        }

        public void KickChargeJustOrNg(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && !(joe.inKick || joe.wantKick || joe.inCombo)) {
                joe.Punch(ItemPunchHand());
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[6];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;

                    joe.SetFaceExpression((int) KarateMan.KarateManFaces.Sad);
                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 2f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        }),
                    });
                }
                else {
                    joe.StartKickCharge(startBeat + 1.25f);
                    ItemHitEffect();
                    status = FlyStatus.Hit;
                }
            }
        }

        public void KickChargeOut(PlayerActionEvent caller) {}

        public void KickChargeThrough(PlayerActionEvent caller)
        {
            var joe = KarateMan.instance.Joe;
            if (GameManager.instance.currentGame != "karateman") return;
            if (status != FlyStatus.Fly || gameObject == null) return;
            BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 2f, delegate { 
                    //TODO: play miss sound
                    //deduct flow if applicable
                    joe.SetFaceExpression((int) KarateMan.KarateManFaces.Surprise);
                }),
                new BeatAction.Action(startBeat + 6f, delegate {
                    if (joe.wantFace == -1)
                        joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                }),
            });
        }

        public void KickJustOrNg(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            var joe = KarateMan.instance.Joe;
            if (status == FlyStatus.Fly && joe.inKick) {
                joe.Kick(Conductor.instance.songPositionInBeats);
                if (state <= -1f || state >= 1f) {
                    startBeat = Conductor.instance.songPositionInBeats;
                    CurrentCurve = ItemCurves[8];
                    curveTargetBeat = 1f;
                    Jukebox.PlayOneShot("miss");
                    status = FlyStatus.NG;

                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 1.25f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Sad);
                        }),
                        new BeatAction.Action(startBeat + 4.25f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        }),
                    });
                }
                else {
                    ItemHitEffect();
                    status = FlyStatus.Hit;
                    CurrentCurve = ItemCurves[7];
                    startBeat = Conductor.instance.songPositionInBeats;
                    curveTargetBeat = 3f;

                    
                    BeatAction.New(joe.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(startBeat + 1.25f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Smirk);
                        }),
                        new BeatAction.Action(startBeat + 4.25f, delegate {
                            joe.SetFaceExpression((int) KarateMan.KarateManFaces.Normal);
                        })
                    });
                }
            }
        }

        public void KickOut(PlayerActionEvent caller) {}

        public void KickThrough(PlayerActionEvent caller)
        {
            if (GameManager.instance.currentGame != "karateman") return;
            if (status != FlyStatus.Fly || gameObject == null) return;
            BeatAction.New(KarateMan.instance.Joe.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 2f, delegate { 
                    //TODO: play miss sound
                    //deduct flow if applicable
                    KarateMan.instance.Joe.SetFaceExpression((int) KarateMan.KarateManFaces.VerySad);
                })
            });
        }
    }
}