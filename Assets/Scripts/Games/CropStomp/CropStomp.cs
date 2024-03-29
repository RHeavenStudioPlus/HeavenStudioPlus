using DG.Tweening;
using NaughtyBezierCurves;
using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrCropLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("cropStomp", "Crop Stomp", "c0f0b8", false, false, new List<GameAction>()
            {
                new GameAction("start marching", "Start Marching")
                {
                    function = delegate { CropStomp.instance.StartMarching(eventCaller.currentEntity.beat); },
                    defaultLength = 2f,
                    inactiveFunction = delegate { CropStomp.MarchInactive(eventCaller.currentEntity.beat); }
                },
                new GameAction("veggies", "Veggies")
                {
                    defaultLength = 4f,
                    resizable = true
                },
                new GameAction("mole", "Mole")
                {
                    preFunction = delegate
                    {
                        if (eventCaller.currentEntity["mute"]) return;
                        CropStomp.MoleSound(eventCaller.currentEntity.beat);
                    },
                    defaultLength = 2f,
                    parameters = new List<Param>()
                    {
                        new Param("mute", false, "Mute", "Toggle if the mole laugh sound should be muted.")
                    },
                    preFunctionLength = 3
                },
                new GameAction("end", "End")
                {
                    parameters = new List<Param>()
                    {
                        new Param("mute", true, "Toggle if Stomp Farmer should stopp humming.")
                    }
                },
                new GameAction("plantCollect", "Set Veggie Collection Thresholds")
                {
                    function = delegate { var e = eventCaller.currentEntity; 
                        CropStomp.instance.SetCollectThresholds(e["threshold"], e["limit"], e["force"], e["forceAmount"]);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("threshold", new EntityTypes.Integer(1, 80, 8), "Threshold", "Set how many veggies it takes for a new plant to appear in the collection bag."),
                        new Param("limit", new EntityTypes.Integer(1, 1000, 80), "Limit", "Set the limit for the amount of plants to be collected and count towards the threshold."),
                        new Param("force", false, "Set Amount Of Collected Plants", "Toggle if this event should automatically set the collected plants to a certain number.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "forceAmount" })
                        }),
                        new Param("forceAmount", new EntityTypes.Integer(0, 1000, 0), "Set Amount", "Set the amount of plants to be collected automatically.")
                    }
                }
            },
            new List<string>() {"ntr", "keep"},
            "ntrstomp", "en",
            new List<string>() {},
            chronologicalSortKey: 12
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_CropStomp;

    public class CropStomp : Minigame
    {
        const float stepDistance = 2.115f;
        //public static float[] moleSoundOffsets = new float[]{ 0.134f, 0.05f, 0.061f };

        float scrollRate => stepDistance / (Conductor.instance.pitchedSecPerBeat * 2f);
        float grassWidth;
        float dotsWidth = 19.2f;

        private double newBeat = -1f; // So that marching can happen on beat 0.
        private double marchStartBeat = -1f;
        private double marchEndBeat = double.MaxValue;
        private bool willNotHum = true;
        private double marchOffset;
        private int currentMarchBeat;
        private int stepCount;
        private bool isStepping;
        
        private static double inactiveStart = -1f;

        public bool isMarching => marchStartBeat != -1f;

        [NonSerialized] public bool isFlicking;

        public GameObject baseVeggie;
        public GameObject baseMole;
        public Animator legsAnim;
        public Animator bodyAnim;
        public Transform farmerTrans;
        public SpriteRenderer grass;
        public Transform grassTrans;
        public Transform dotsTrans;
        public Transform scrollingHolder;
        public Transform veggieHolder;
        public Farmer farmer;
        public BezierCurve3D pickCurve;
        public BezierCurve3D moleCurve;

        private Tween shakeTween;

        public ParticleSystem hitParticle;

        public static CropStomp instance;

        public static PlayerInput.InputAction InputAction_Flick =
            new("NtrStompFlick", new int[] { IAEmptyCat, IAFlickCat, IAEmptyCat },
            IA_Empty, IA_TouchFlick, IA_Empty);

        private void Awake()
        {
            instance = this; // Finding grass sprite width for grass scrolling.
            farmer.Init();
            Sprite sprite = grass.sprite;
            float borderLeft = sprite.rect.xMin + sprite.border.x;
            float borderRight = sprite.rect.xMax - sprite.border.z;
            float borderWidthPixels = borderRight - borderLeft;
            grassWidth = borderWidthPixels / sprite.pixelsPerUnit;

            legsAnim.Play("LiftFront", 0, 1); // Start with leg up.
        }

        public override void OnGameSwitch(double beat)
        {
            double startBeat;
            double endBeat = double.MaxValue;
            var entities = GameManager.instance.Beatmap.Entities;
            if (inactiveStart == -1f)
            {
                // Find the beat of the closest "start marching" event.
                var lastMarch = entities.Find(c => c.datamodel == "cropStomp/start marching" && beat <= c.beat);
                startBeat = lastMarch?.beat ?? beat;
            }
            else
            {
                // Find the beat of the next step, assuming marching started at inactiveStart.
                int stepsPassed = 0;

                while (inactiveStart + (stepsPassed * 2f) < beat)
                {
                    stepsPassed++;

                    if (stepsPassed > 1000)
                    {
                        return;
                    }
                }

                startBeat = inactiveStart + (stepsPassed * 2f);

                // Cue the marching proper to begin when applicable.
                BeatAction.New(this, new() { new(startBeat - 0.25f, delegate { StartMarching(startBeat); }) });

                inactiveStart = -1f;
            }

            // find out when the next game switch (or remix end) happens
            RiqEntity firstEnd = entities.Find(c => c.datamodel is "gameManager/switchGame/cropStomp" or "gameManager/end" && c.beat > startBeat);
            endBeat = firstEnd?.beat ?? double.MaxValue;

            // Veggie and mole events.
            List<RiqEntity> vegEvents = entities.FindAll(v => v.datamodel == "cropStomp/veggies");
            List<RiqEntity> moleEvents = entities.FindAll(m => m.datamodel == "cropStomp/mole");

            // Spawn veggies.
            for (int i = 0; i < vegEvents.Count; i++) {
                var vegBeat = vegEvents[i].beat;
                var vegLength = vegEvents[i].length;

                // Only consider veggie events that aren't past the start point.
                if (startBeat <= vegBeat + vegLength) {
                    int veggiesInEvent = Mathf.CeilToInt(vegLength + 1) / 2;

                    for (int b = 0; b < veggiesInEvent; b++)
                    {
                        var targetVeggieBeat = vegBeat + (2f * b);
                        if (startBeat <= targetVeggieBeat && targetVeggieBeat < endBeat)
                        {
                            SpawnVeggie(targetVeggieBeat, startBeat, false);
                        }
                    }
                }
            }

            // Spawn moles.
            for (int i = 0; i < moleEvents.Count; i++) {
                var moleBeat = moleEvents[i].beat;

                if (startBeat <= moleBeat && moleBeat < endBeat) {
                    SpawnVeggie(moleBeat, startBeat, true);
                }
            }
            SetInitTresholds(beat);
            SetMarchEndBeat(beat);
        }

        public override void OnPlay(double beat)
        {
            OnGameSwitch(beat);
        }

        private void SetMarchEndBeat(double beat)
        {
            var nextEnd = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame", "end" }).Find(e => e.beat > beat);
            double nextEndBeat = nextEnd?.beat ?? double.MaxValue;

            var firstEnd = GameManager.instance.Beatmap.Entities.Find(c => c.datamodel == "cropStomp/end" && c.beat >= beat && c.beat < nextEndBeat);
            if (firstEnd != null) {
                marchEndBeat = firstEnd.beat;
                willNotHum = firstEnd["mute"];
            }
        }

        public static void MoleSound(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("cropStomp/moleNyeh", beat - 2,   offset: 0.134),
                new MultiSound.Sound("cropStomp/moleHeh1", beat - 1.5, offset: 0.05),
                new MultiSound.Sound("cropStomp/moleHeh2", beat - 1,   offset: 0.061)
            }, forcePlay: true);
        }

        private void Update()
        {
            var cond = Conductor.instance;

            if (!cond.isPlaying || !isMarching) return;

            // Debug.Log(newBeat);

            bool cameraLocked = cond.songPositionInBeats >= marchEndBeat;
            bool isHumming = !(cameraLocked && willNotHum);

            if (cond.ReportBeat(ref newBeat, marchOffset, true))
            {
                currentMarchBeat += 1;

                PlayAnims();
                if (currentMarchBeat % 2 != 0 && isHumming) //step sound
                {
                    MultiSound.Play(new MultiSound.Sound[] {new MultiSound.Sound("cropStomp/hmm", newBeat + marchOffset)});
                }
            }

            if (PlayerInput.GetIsAction(InputAction_BasicRelease) && !IsExpectingInputNow(InputAction_BasicRelease))
            {
                bodyAnim.DoScaledAnimationAsync("Raise", 0.5f);
            }
            if (PlayerInput.GetIsAction(InputAction_Flick) && !IsExpectingInputNow(InputAction_FlickRelease))
            {
                bodyAnim.DoScaledAnimationAsync("Pick", 0.5f);
            }

            if (cameraLocked) return;

            // Object scroll.
            var scrollPos = scrollingHolder.localPosition;
            var newScrollX = scrollPos.x + (scrollRate * Time.deltaTime);
            scrollingHolder.localPosition = new Vector3(newScrollX, scrollPos.y, scrollPos.z);

            // Grass scroll.
            var grassPos = grassTrans.localPosition;

            var newGrassX = grassPos.x + (scrollRate * Time.deltaTime);
            newGrassX %= grassWidth * 4.5f;

            grassTrans.localPosition = new Vector3(newGrassX, grassPos.y, grassPos.z);

            // Dots scroll
            var dotsPos = dotsTrans.localPosition;

            var newDotsX = dotsPos.x + (scrollRate * Time.deltaTime);
            newDotsX %= dotsWidth;

            dotsTrans.localPosition = new Vector3(newDotsX, dotsPos.y, dotsPos.z);
        }

        private void LateUpdate()
        {
            if (!isMarching) return;
            isFlicking = false;
        }

        public void SetCollectThresholds(int thresholdEvolve, int limit, bool force, int forceAmount)
        {
            farmer.plantThreshold = thresholdEvolve;
            farmer.plantLimit = limit;
            if (force) Farmer.collectedPlants = forceAmount;
            farmer.UpdatePlants();
        }

        private void SetInitTresholds(double beat)
        {
            var lastCollect = GameManager.instance.Beatmap.Entities.FindLast(c => c.datamodel == "cropStomp/plantCollect" && c.beat < beat);
            if (lastCollect == null) return;
            SetCollectThresholds(lastCollect["threshold"], lastCollect["limit"], lastCollect["force"], lastCollect["forceAmount"]);
        }

        public void CollectPlant(int veggieType)
        {
            farmer.CollectPlant(veggieType);
        }

        private void PlayAnims()
        {
            // Step.
            if (currentMarchBeat % 2 != 0)
            {
                // Don't step if already stomped.
                if (!isStepping)
                {
                    stepCount += 1;

                    var stepAnim = (stepCount % 2 != 0) ? "StepFront" : "StepBack";
                    legsAnim.DoScaledAnimationAsync(stepAnim, 0.5f);

                    isStepping = true;
                }
            }
            // Lift.
            else
            {
                var liftAnim = (stepCount % 2 != 0) ? "LiftBack" : "LiftFront";
                legsAnim.DoScaledAnimationAsync(liftAnim, 0.5f);

                var farmerPos = farmerTrans.localPosition;
                farmerTrans.localPosition = new Vector3(farmerPos.x - stepDistance, farmerPos.y, farmerPos.z);

                isStepping = false;
            }
        }

        public void StartMarching(double beat)
        {
            marchStartBeat = beat;
            marchOffset = marchStartBeat % 1;
            currentMarchBeat = 0;
            stepCount = 0;

            farmer.nextStompBeat = beat;
        }

        public void Stomp()
        {
            // Don't increment step counter if autostep stepped already.
            if (!isStepping) stepCount += 1;

            var stompAnim = (stepCount % 2 != 0) ? "StompFront" : "StompBack";
            
            legsAnim.DoScaledAnimationAsync(stompAnim, 0.5f);

            SoundByte.PlayOneShotGame("cropStomp/stomp");

            if (shakeTween != null) shakeTween.Kill(true);

            DOTween.Punch(() =>
                GameCamera.AdditionalPosition,
                x => GameCamera.AdditionalPosition = x,
                new Vector3(0, 0.75f, 0), Conductor.instance.pitchedSecPerBeat * 0.5f, 18, 1f
            );

            isStepping = true;
        }

        private void SpawnVeggie(double beat, double startBeat, bool isMole)
        {
            var newVeggie = Instantiate(isMole ? baseMole : baseVeggie, veggieHolder).GetComponent<Veggie>();

            newVeggie.targetBeat = beat;

            var veggieX = (beat - startBeat) * -stepDistance / 2f;
            newVeggie.transform.localPosition = new Vector3((float)veggieX, 0f, 0f);
            newVeggie.Init();
            newVeggie.gameObject.SetActive(true);
        }
        
        public static void MarchInactive(double beat)
        {
            if (GameManager.instance.currentGame == "cropStomp") return;
            inactiveStart = beat;
            RiqEntity gameSwitch = GameManager.instance.Beatmap.Entities.Find(c => c.beat >= beat && c.datamodel == "gameManager/switchGame/cropStomp");
            if (gameSwitch == null) return;
            int length = (int)Math.Ceiling((gameSwitch.beat - beat) / 2);
            MultiSound.Sound[] sounds = new MultiSound.Sound[length];
            for(int i = 0; i < length; i++) {
                sounds[i] = new MultiSound.Sound("cropStomp/hmm", beat + (i * 2));
            }
            MultiSound.Play(sounds, forcePlay: true);
        }
    }
}
