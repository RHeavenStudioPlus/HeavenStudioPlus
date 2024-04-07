using System.Collections.Generic;
using System.Linq;
using HeavenStudio.Util;
using Jukebox;
using UnityEngine;
using UnityEngine.Rendering;

namespace HeavenStudio.Games.Scripts_CatchOfTheDay
{
    public class LakeScene : MonoBehaviour
    {
        [SerializeField] public bool IsDummy = false;

        [SerializeField] public Animator FishAnimator;
        [SerializeField] public Animator BGAnimator;
        [SerializeField] public SpriteRenderer GradientBG;
        [SerializeField] public SpriteRenderer TopBG;
        [SerializeField] public SpriteRenderer BottomBG;
        [SerializeField] public BGFish[] BGFishes;
        [SerializeField] public SortingGroup _SortingGroup;
        [SerializeField] public GameObject BigManta;
        [SerializeField] public GameObject SmallManta;
        [SerializeField] public GameObject FishSchool;
        [SerializeField] public GameObject[] SchoolFishes;
        [SerializeField] public ParticleSystem[] Bubbles;

        [SerializeField] Color[] TopColors;
        [SerializeField] Color[] BottomColors;

        public RiqEntity Entity;
        public PlayerActionEvent ReelAction;
        public CatchOfTheDay Minigame;
        public bool FishOut = false;

        public MultiSound _MultiSound;

        private RenderTexture _RenderTexture;
        private Texture2D _DisplayTexture;

        private double? _CrossfadeStartBeat;
        [SerializeField] GameObject Renderer;
        //private bool _FirstUpdate = false;    Unused value - Marc

        [SerializeField] Animator CrossfadeAnimator;

        void Update()
        {
            if (FishSchool.activeSelf)
                FishSchool.transform.localEulerAngles = new Vector3(0, 0, Conductor.instance.songPositionInBeats * 45);
            if (BigManta.activeSelf)
                BigManta.transform.localPosition = new Vector3((float)Entity.beat - Conductor.instance.songPositionInBeats + 4.5f, BigManta.transform.localPosition.y, BigManta.transform.localPosition.z);
            if (SmallManta.activeSelf)
                SmallManta.transform.localPosition = new Vector3(1.25f + ((Conductor.instance.songPositionInBeats - (float)Entity.beat) * 0.13f), SmallManta.transform.localPosition.y, SmallManta.transform.localPosition.y);

            if (!IsDummy)
                RenderScene();

            if (_CrossfadeStartBeat is double startBeat)
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 1);
                DisplaySprite.color = new Color(1, 1, 1, 1 - normalizedBeat);
                float scale = 1 + (float)(normalizedBeat * 0.875);// * 2);
                transform.localScale = new Vector3(scale, scale, 1);
                if (normalizedBeat >= 1f) Destroy(gameObject);
            }
        }
        void OnDestroy()
        {
            if (ReelAction != null)
                ReelAction.Disable();
        }

        public int Setup(RiqEntity e, CatchOfTheDay minigame, int? lastLayout = null, int sortingIndex = 0)
        {
            Entity = e;
            Minigame = minigame;

            switch (e.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationAsync("Fish1_Wait", 0.5f);

                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(e.beat, delegate { ReelAction = minigame.ScheduleInput(e.beat, 2f, CatchOfTheDay.InputAction_BasicPress, Hit, Through, Out); }),

                        new BeatAction.Action(e.beat, delegate { DoPickAnim(e.beat); }),
                        new BeatAction.Action(e.beat + 1, delegate { DoPickAnim(e.beat + 1); }),
                        new BeatAction.Action(e.beat + 2 - 0.1f, delegate { DoBiteAnim(e.beat + 2 - 0.1f); }),
                        new BeatAction.Action(e.beat + 2 + (float)e["sceneDelay"], delegate { minigame.DisposeLake(this, e.beat + 2 + (float)e["sceneDelay"]); }),
                    });
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationAsync("Fish2_Wait", 0.5f);

                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(e.beat, delegate { ReelAction = minigame.ScheduleInput(e.beat, 3f, CatchOfTheDay.InputAction_BasicPress, Hit, Through, Out); }),

                        new BeatAction.Action(e.beat, delegate { DoPickAnim(e.beat); }),
                        new BeatAction.Action(e.beat + 0.5f, delegate { DoPickAnim(e.beat + 0.5f); }),
                        new BeatAction.Action(e.beat + 1, delegate { DoPickAnim(e.beat + 1); }),
                        new BeatAction.Action(e.beat + 3 - 0.1f, delegate { DoBiteAnim(e.beat + 3 - 0.1f); }),
                        new BeatAction.Action(e.beat + 3 + (float)e["sceneDelay"], delegate { minigame.DisposeLake(this, e.beat + 3 + (float)e["sceneDelay"]); }),
                    });
                    break;
                case "catchOfTheDay/fish3":
                    List<BeatAction.Action> beatActions = new();
                    if ((bool)e["fakeOut"])
                    {
                        FishAnimator.DoScaledAnimationAsync("Fish1_Wait", 0.5f);
                        beatActions.Add(new BeatAction.Action(e.beat - 4, delegate { FishAnimator.DoScaledAnimationFromBeatAsync("Fish3_WaitB", timeScale: 0.5f, startBeat: e.beat - 4); }));
                    }
                    else
                        FishAnimator.DoScaledAnimationAsync("Fish3_Wait", 0.5f);

                    beatActions.AddRange(new List<BeatAction.Action>(){
                        new BeatAction.Action(e.beat, delegate { ReelAction = minigame.ScheduleInput(e.beat, 4.5f, CatchOfTheDay.InputAction_BasicPress, Hit, Through, Out); }),

                        new BeatAction.Action(e.beat, delegate { DoPickAnim(e.beat); }),
                        new BeatAction.Action(e.beat + 0.25f, delegate { DoPickAnim(e.beat + 0.25f, down: true); }),
                        new BeatAction.Action(e.beat + 0.5f, delegate { DoPickAnim(e.beat + 0.5f); }),
                        new BeatAction.Action(e.beat + 1f, delegate { DoPickAnim(e.beat + 1f, down: true); }),
                        new BeatAction.Action(e.beat + 4.5f - 0.1f, delegate { DoBiteAnim(e.beat + 4.5f - 0.1f); }),
                        new BeatAction.Action(e.beat + 4.5f + (float)e["sceneDelay"], delegate { minigame.DisposeLake(this, e.beat + 4.5f + (float)e["sceneDelay"]); }),
                    });
                    BeatAction.New(this, beatActions);
                    break;
                default:
                    break;
            }

            int layout = e["layout"];
            if (layout == (int)CatchOfTheDay.FishLayout.Random)
            {
                if (lastLayout is int ll)
                {
                    List<int> layouts = new() { 0, 1, 2 };
                    layouts.Remove(ll);
                    layout = layouts[UnityEngine.Random.Range(0, layouts.Count)];
                }
                else
                    layout = 0;
            }
            switch (layout)
            {
                case (int)CatchOfTheDay.FishLayout.LayoutC:
                    BGAnimator.DoScaledAnimationAsync("LayoutC", 0.5f);
                    break;
                case (int)CatchOfTheDay.FishLayout.LayoutB:
                    BGAnimator.DoScaledAnimationAsync("LayoutB", 0.5f);
                    break;
                case (int)CatchOfTheDay.FishLayout.LayoutA:
                default:
                    BGAnimator.DoScaledAnimationAsync("LayoutA", 0.5f);
                    break;
            }

            if (e["useCustomColor"])
            {
                SetBGColors(e["colorTop"], e["colorBottom"]);
            }
            else
            {
                SetBGColors(TopColors[layout], BottomColors[layout]);
            }

            float xOffset = UnityEngine.Random.Range(-0.5f, 0.5f);
            float yOffset = UnityEngine.Random.Range(-0.3f, 0.3f);
            Renderer.transform.position += new Vector3(xOffset, yOffset, 0);

            if ((bool)e["schoolFish"])
            {
                FishSchool.SetActive(true);
                SetFishDensity(e["fishDensity"]);
            }
            else
                FishSchool.SetActive(false);

            BigManta.SetActive(e["fgManta"]);
            SmallManta.SetActive(e["bgManta"]);

            int bubbleCount = UnityEngine.Random.Range(0, 4);
            foreach (ParticleSystem particle in Bubbles.OrderBy(_ => UnityEngine.Random.Range(0.0f, 1.0f)).ToArray()[0..bubbleCount])
                particle.PlayScaledAsync(0.5f);

            DisplaySprite.sortingOrder = sortingIndex;
            Renderer.transform.localPosition -= new Vector3(0, 0, sortingIndex);
            RenderCamera.transform.localPosition -= new Vector3(0, 0, sortingIndex);

            return layout; // returning this so we can catalogue the most recent layout so we don't double up
        }
        public void SetBGColors(Color topColor, Color bottomColor)
        {
            GradientBG.color = topColor;
            TopBG.color = topColor;
            BottomBG.color = bottomColor;
            foreach (BGFish fish in BGFishes)
            {
                fish.SetColor(bottomColor);
            }
        }
        public void SetFishDensity(float density)
        {
            if (density <= 0.0f)
            {
                FishSchool.SetActive(false);
                return;
            }

            float fishCount = density * SchoolFishes.Length;
            for (int i = 0; i < SchoolFishes.Length; i++)
            {
                SchoolFishes[i].SetActive(i < fishCount);
            }
        }

        public void Hit(PlayerActionEvent caller, float state)
        {
            if (FishOut)
                return;

            if (state is < 1 and > (-1))
            {
                Just();
            }
            else
            {
                Miss();
            }
        }
        public void Just()
        {
            if (FishOut)
                return;

            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationAsync("Fish1_Just", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/quick3");
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationAsync("Fish2_Just", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/pausegill4");
                    break;
                case "catchOfTheDay/fish3":
                    FishAnimator.DoScaledAnimationAsync("Fish3_Just", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/threefish5");
                    break;
                default:
                    break;
            }
            Minigame.DoJustAnim();
            FishOut = true;
        }
        public void Miss()
        {
            if (FishOut)
                return;

            FishOut = true;

            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationAsync("Fish1_Miss", 0.5f);
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationAsync("Fish2_Miss", 0.5f);
                    break;
                case "catchOfTheDay/fish3":
                    FishAnimator.DoScaledAnimationAsync("Fish3_Miss", 0.5f);
                    break;
                default:
                    break;
            }
            Minigame.DoMissAnim();
            SoundByte.PlayOneShotGame("catchOfTheDay/nearMiss");
            FishOut = true;
        }
        public void Through(PlayerActionEvent caller)
        {
            if (FishOut)
                return;

            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationAsync("Fish1_Through", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/quick_laugh");
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationAsync("Fish2_Through", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/pausegill_laugh");
                    break;
                case "catchOfTheDay/fish3":
                    FishAnimator.DoScaledAnimationAsync("Fish3_Through", 0.5f);
                    SoundByte.PlayOneShotGame("catchOfTheDay/threefish_laugh");
                    break;
                default:
                    break;
            }
            Minigame.DoThroughAnim();
        }
        public void Out(PlayerActionEvent caller)
        {
            if (FishOut)
                return;

            FishOut = true;
            Minigame.ScoreMiss();
            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationAsync("Fish1_Out", 0.5f);
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationAsync("Fish2_Out", 0.5f);
                    break;
                case "catchOfTheDay/fish3":
                    FishAnimator.DoScaledAnimationAsync("Fish3_Out", 0.5f);
                    break;
                default:
                    break;
            }
            Minigame.DoOutAnim();
            _MultiSound.StopAll();

            ReelAction.CanHit(false);

            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.75)
            {
                foreach (BGFish fish in BGFishes)
                {
                    fish.Flee();
                }
            }

            SoundByte.PlayOneShot("miss");
        }

        public void DoPickAnim(double beat, bool down = false)
        {
            if (FishOut)
                return;

            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationFromBeatAsync("Fish1_Pick", 0.5f, beat);
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationFromBeatAsync("Fish2_Pick", 0.5f, beat);
                    break;
                case "catchOfTheDay/fish3":
                    if (down)
                        FishAnimator.DoScaledAnimationFromBeatAsync("Fish3_PickDown", 0.5f, beat);
                    else
                        FishAnimator.DoScaledAnimationFromBeatAsync("Fish3_PickUp", 0.5f, beat);
                    break;
                default:
                    break;
            }
            Minigame.DoPickAnim();
        }
        public void DoBiteAnim(double beat)
        {
            if (FishOut)
                return;

            switch (Entity.datamodel)
            {
                case "catchOfTheDay/fish1":
                    FishAnimator.DoScaledAnimationFromBeatAsync("Fish1_Bite", 0.5f, beat);
                    break;
                case "catchOfTheDay/fish2":
                    FishAnimator.DoScaledAnimationFromBeatAsync("Fish2_Bite", 0.5f, beat);
                    break;
                case "catchOfTheDay/fish3":
                    FishAnimator.DoScaledAnimationFromBeatAsync("Fish3_Bite", 0.5f, beat);
                    break;
                default:
                    break;
            }
        }

        [SerializeField] Camera RenderCamera;
        [SerializeField] SpriteRenderer DisplaySprite;
        public void RenderScene()
        {
            if (!DisplaySprite.enabled)
                return;

            Rect rect = new Rect(0, 0, 1024, 1024);
            if (_RenderTexture == null)
            {
                _RenderTexture = new RenderTexture(1024, 1024, 32);
                RenderCamera.targetTexture = _RenderTexture;

                _DisplayTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
                Sprite sprite = Sprite.Create(_DisplayTexture, new Rect(0, 0, 1024, 1024), new Vector2(0.5f, 0.5f));
                DisplaySprite.sprite = sprite;
            }

            RenderCamera.Render();

            RenderTexture currentRenderTexture = RenderTexture.active;
            RenderTexture.active = _RenderTexture;
            _DisplayTexture.ReadPixels(rect, 0, 0);
            _DisplayTexture.Apply();

            RenderTexture.active = currentRenderTexture;
        }

        public void Crossfade(double beat)
        {
            _CrossfadeStartBeat = beat;
            //CrossfadeAnimator.DoScaledAnimationAsync("Crossfade", 0.5f);
        }

        [SerializeField] GameObject SchoolFishPrefab;
        [ContextMenu("Spawn a bunch of fish")]
        void SpawnABunchOfFish()
        {
            Transform container = transform.Find("Renderer/FishSchool");

            List<Transform> toDestroy = new();
            for (int i = 0; i < container.childCount; i++)
            {
                toDestroy.Add(container.GetChild(i));
            }
            foreach (Transform child in toDestroy)
                DestroyImmediate(child.gameObject);

            SchoolFishes = new GameObject[250];
            for (int i = 1; i <= 250; i++)
            {
                var randRot = UnityEngine.Random.Range(-180f, 180f);
                container.eulerAngles = new Vector3(0, 0, randRot);

                GameObject fish = Instantiate(SchoolFishPrefab, container);
                fish.name = $"SchoolFish{i:D3}";

                fish.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = i * 2;
                fish.transform.Find("Fin").GetComponent<SpriteRenderer>().sortingOrder = (i * 2) + 1;
                fish.transform.Find("Eye").GetComponent<SpriteRenderer>().sortingOrder = (i * 2) + 1;

                var yOffset = UnityEngine.Random.Range(3f, 11f);
                fish.transform.position += new Vector3(0, yOffset, 0);
                fish.transform.eulerAngles -= new Vector3(0, 0, randRot + 180);

                SchoolFishes[i-1] = fish;
            }
            container.eulerAngles = Vector3.zero;
        }
    }

}