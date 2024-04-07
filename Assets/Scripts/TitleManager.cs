using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using HeavenStudio.Util;
using HeavenStudio.Common;
using HeavenStudio.InputSystem;

using System.Linq;
using BurstLinq;

using SFB;
using Jukebox;
using TMPro;

namespace HeavenStudio
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private Button createButton;

        [SerializeField] private Animator logoAnim;

        [SerializeField] private List<Animator> starAnims;

        [SerializeField] private Animator pressAnyKeyAnim;

        [SerializeField] private float bpm = 114f;
        [SerializeField] private float offset = 0f;

        [SerializeField] private RawImage bg;

        [SerializeField] private float bgXSpeed;
        [SerializeField] private float bgYSpeed;

        [SerializeField] private Collider2D logoHoverCollider;

        [SerializeField] private SettingsDialog settingsPanel;

        [SerializeField] private GameObject snsPanel;
        [SerializeField] private TMP_Text snsVersionText;

        [SerializeField] private GameObject playPanel;
        [SerializeField] private TMP_Text chartTitleText;
        [SerializeField] private TMP_Text chartMapperText;
        [SerializeField] private TMP_Text chartIdolText;
        [SerializeField] private TMP_Text chartDescText;
        [SerializeField] private GameObject flashWarning;
        [SerializeField] private TMP_Text chartStyleText;
        [SerializeField] private Image campaignOption;
        [SerializeField] private Sprite campaignOn;
        [SerializeField] private Sprite campaignOff;

        [SerializeField] private Selectable[] mainSelectables;
        [SerializeField] private Selectable defaultSelectable;
        [SerializeField] private RectTransform selectedDisplayRect;
        [SerializeField] private GameObject selectedDisplayIcon;
        [SerializeField] private GameObject[] otherHiddenOnMouse;
        //static bool firstBoot = true;    Unused value - Marc

        private AudioSource musicSource;

        private double songPosBeat;
        private double songPos;
        private double time;
        private double targetBopBeat;

        private int loops;

        private double lastAbsTime;
        private double startTime;
        private float playPanelRevealTime;

        private bool altBop;

        private bool logoRevealed;

        private bool menuMode, snsRevealed, playMenuRevealed, exiting, firstPress, usingMouse, waitingForButtonUp;

        private Animator menuAnim, selectedDisplayAnim;
        private Selectable currentSelectable, mouseSelectable;
        private RectTransform currentSelectableRect, lastSelectableRect;
        private float selectableLerpTimer;


        private void Start()
        {
            exiting = false;
            menuAnim = GetComponent<Animator>();
            musicSource = GetComponent<AudioSource>();
            musicSource.PlayScheduled(AudioSettings.dspTime);
            startTime = Time.realtimeSinceStartupAsDouble;
            var _rand = new System.Random();
            starAnims = starAnims.OrderBy(_ => _rand.Next()).ToList();
            selectedDisplayRect.gameObject.SetActive(false);

            if (selectedDisplayRect.TryGetComponent(out Animator anim))
            {
                selectedDisplayAnim = anim;
            }

#if HEAVENSTUDIO_PROD
            versionText.text = GlobalGameManager.friendlyReleaseName;
#elif UNITY_EDITOR
            versionText.text = "EDITOR";
#else
            versionText.text = Application.buildGUID.Substring(0, 8) + " " + AppInfo.Date.ToString("dd/MM/yyyy hh:mm:ss");
#endif
        }

        private void Update()
        {
            bg.uvRect = new Rect(bg.uvRect.position + (new Vector2(bgXSpeed, bgYSpeed) * Time.deltaTime), bg.uvRect.size);
            if (songPos >= musicSource.clip.length)
            {
                time = 0;
                targetBopBeat = 0;
                loops++;
            }
            double absTime = Time.realtimeSinceStartupAsDouble - startTime;
            double dt = absTime - lastAbsTime;
            lastAbsTime = absTime;

            time += dt;

            songPos = time + offset;

            songPosBeat = SecsToBeats(songPos);

            if (selectedDisplayAnim.isActiveAndEnabled)
            {
                selectedDisplayAnim.DoNormalizedAnimation("Idle", GetPositionFromBeat(0, 2));
            }

            if (!menuMode && songPosBeat >= 0.5)
            {
                var controllers = PlayerInput.GetInputControllers();
                foreach (var newController in controllers)
                {
                    if (newController.GetLastButtonDown(true) > 0)
                    {
                        if (logoRevealed)
                        {
                            menuMode = true;
                            firstPress = true;
                            currentSelectable = defaultSelectable;
                            SetSelectableRectTarget(currentSelectable);
                            selectableLerpTimer = 1;

                            menuAnim.Play("Revealed", 0, 0);
                            pressAnyKeyAnim.Play("PressKeyFadeOut", 0, 0);
                            SoundByte.PlayOneShot("ui/UIEnter");

                            var nextController = newController;
                            var lastController = PlayerInput.GetInputController(1);

                            Debug.Log("Assigning controller: " + newController.GetDeviceName());

                            if (lastController != nextController)// && !firstBoot)
                            {
                                //firstBoot = false;    Unused value - Marc
                                if (nextController == null)
                                {
                                    Debug.Log("invalid controller, using keyboard");
                                    nextController = controllers[0];
                                }
                                lastController.SetPlayer(null);
                                nextController.SetPlayer(1);
                                nextController.OnSelected();
                                PlayerInput.CurrentControlStyle = nextController.GetDefaultStyle();
                                usingMouse = PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch;
                                selectedDisplayIcon.SetActive(!usingMouse);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(!usingMouse);
                                }
                            }
                            break;
                        }
                        else
                        {
                            SkipToBeat(3.5);
                        }
                    }
                }
            }

            if (loops == 0 && !logoRevealed)
            {
                float normalizedBeat = GetPositionFromBeat(3, 1);
                if (normalizedBeat > 0 && normalizedBeat <= 1f)
                {
                    logoAnim.DoNormalizedAnimation("Reveal", normalizedBeat);
                    pressAnyKeyAnim.DoNormalizedAnimation("PressKeyFadeIn", normalizedBeat);
                }
                else if (normalizedBeat < 0)
                {
                    logoAnim.DoNormalizedAnimation("Reveal", 0);
                }
                else if (normalizedBeat > 1f)
                {
                    logoRevealed = true;
                }
            }
            if (songPosBeat - 1 >= targetBopBeat)
            {
                if (targetBopBeat <= 3 && loops == 0)
                {
                    starAnims[(int)targetBopBeat].Play("StarAppearBop", 0, 0);
                    if (targetBopBeat == 3) starAnims[4].Play("StarAppearBop", 0, 0);
                    for (int i = 0; i < (int)targetBopBeat; i++)
                    {
                        starAnims[i].Play("StarBopNoRot", 0, 0);
                    }
                }
                else
                {
                    foreach (var star in starAnims)
                    {
                        star.Play("StarBop", 0, 0);
                    }
                }
                if (targetBopBeat > 2 || loops > 0)
                {
                    logoAnim.Play(altBop ? "LogoBop2" : "LogoBop", 0, 0);
                    altBop = !altBop;
                }
                targetBopBeat += 1;
            }

            var controller = PlayerInput.GetInputController(1);
            if (menuMode && !(exiting || GlobalGameManager.IsShowingDialog || waitingForButtonUp))
            {
                if (playMenuRevealed)
                {
                    if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
                    {
                        switch (controller.GetLastActionDown())
                        {
                            case (int)InputController.ActionsPad.East:
                                PlayPanelAccept();
                                break;
                            case (int)InputController.ActionsPad.South:
                                PlayPanelBack();
                                break;
                            case (int)InputController.ActionsPad.North:
                                ToggleCampaign();
                                break;
                        }
                    }
                }
                else if (snsRevealed)
                {
                    if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
                    {
                        if (controller.GetLastActionDown() == (int)InputController.ActionsPad.South)
                        {
                            SocialsClose();
                        }
                    }
                }
                else if (settingsPanel.IsOpen)
                {
                    // if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
                    // {
                    //     if (controller.GetLastActionDown() == (int)InputController.ActionsPad.South)
                    //     {
                    //         SettingsClose();
                    //     }
                    // }
                }
                else if (!firstPress)
                {
                    UpdateSelectable(controller);
                }
            }
            if (waitingForButtonUp)
            {
                if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
                {
                    if (controller.GetActionUp(PlayerInput.CurrentControlStyle, (int)InputController.ActionsPad.East, out _))
                    {
                        waitingForButtonUp = false;
                    }
                }
            }
            if (firstPress) firstPress = false;
        }

        void UpdateSelectable(InputController controller)
        {
            selectableLerpTimer += Time.deltaTime;
            if (selectableLerpTimer < 0.2f)
            {
                float prog = Mathf.Clamp01(selectableLerpTimer / 0.2f);
                prog = 1f - Mathf.Pow(1f - prog, 3);
                selectedDisplayRect.position = Vector3.Lerp(lastSelectableRect.position, currentSelectableRect.position, prog);
                selectedDisplayRect.sizeDelta = Vector2.Lerp(lastSelectableRect.sizeDelta, currentSelectableRect.sizeDelta, prog);
            }
            else
            {
                selectedDisplayRect.position = currentSelectableRect.position;
                selectedDisplayRect.sizeDelta = currentSelectableRect.sizeDelta;
            }

            if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
            {
                switch (controller.GetLastActionDown())
                {
                    case (int)InputController.ActionsPad.East:
                        if (currentSelectable != null)
                        {
                            if (usingMouse)
                            {
                                usingMouse = false;
                                selectedDisplayIcon.SetActive(true);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(true);
                                }
                            }
                            currentSelectable.GetComponent<Button>()?.onClick.Invoke();
                        }
                        break;
                    case (int)InputController.ActionsPad.Up:
                        if (currentSelectable != null)
                        {
                            if (currentSelectable.FindSelectableOnUp() != null)
                            {
                                usingMouse = false;
                                selectedDisplayIcon.SetActive(true);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(true);
                                }
                                currentSelectable = currentSelectable.FindSelectableOnUp();
                                currentSelectable.Select();
                                SetSelectableRectTarget(currentSelectable);
                                SoundByte.PlayOneShot("ui/UIOption");
                            }
                            else
                            {
                                // SoundByte.PlayOneShot("ui/UISelect");
                            }
                        }
                        break;
                    case (int)InputController.ActionsPad.Down:
                        if (currentSelectable != null)
                        {
                            if (currentSelectable.FindSelectableOnDown() != null)
                            {
                                usingMouse = false;
                                selectedDisplayIcon.SetActive(true);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(true);
                                }
                                currentSelectable = currentSelectable.FindSelectableOnDown();
                                currentSelectable.Select();
                                SetSelectableRectTarget(currentSelectable);
                                SoundByte.PlayOneShot("ui/UIOption");
                            }
                            else
                            {
                                // SoundByte.PlayOneShot("ui/UISelect");
                            }
                        }
                        break;
                    case (int)InputController.ActionsPad.Left:
                        if (currentSelectable != null)
                        {
                            if (currentSelectable.FindSelectableOnLeft() != null)
                            {
                                usingMouse = false;
                                selectedDisplayIcon.SetActive(true);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(true);
                                }
                                currentSelectable = currentSelectable.FindSelectableOnLeft();
                                currentSelectable.Select();
                                SetSelectableRectTarget(currentSelectable);
                                SoundByte.PlayOneShot("ui/UIOption");
                            }
                            else
                            {
                                // SoundByte.PlayOneShot("ui/UISelect");
                            }
                        }
                        break;
                    case (int)InputController.ActionsPad.Right:
                        if (currentSelectable != null)
                        {
                            if (currentSelectable.FindSelectableOnRight() != null)
                            {
                                usingMouse = false;
                                selectedDisplayIcon.SetActive(true);
                                foreach (var obj in otherHiddenOnMouse)
                                {
                                    obj.SetActive(true);
                                }
                                currentSelectable = currentSelectable.FindSelectableOnRight();
                                currentSelectable.Select();
                                SetSelectableRectTarget(currentSelectable);
                                SoundByte.PlayOneShot("ui/UIOption");
                            }
                            else
                            {
                                // SoundByte.PlayOneShot("ui/UISelect");
                            }
                        }
                        break;
                }
            }
            foreach (var selectable in mainSelectables)
            {
                // get button the mouse is hovering over
                if (RectTransformUtility.RectangleContainsScreenPoint(selectable.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
                {
                    if (mouseSelectable != selectable)
                    {
                        mouseSelectable = selectable;
                        usingMouse = true;
                        selectedDisplayIcon.SetActive(false);
                        foreach (var obj in otherHiddenOnMouse)
                        {
                            obj.SetActive(false);
                        }
                    }
                    if (currentSelectable != selectable && usingMouse)
                    {
                        currentSelectable = selectable;
                        SetSelectableRectTarget(currentSelectable);
                        SoundByte.PlayOneShot("ui/UIOption");
                        break;
                    }
                }
            }

        }

        void SetSelectableRectTarget(Selectable selectable)
        {
            if (selectable == null)
            {
                selectedDisplayRect.gameObject.SetActive(false);
                return;
            }
            selectedDisplayRect.gameObject.SetActive(true);
            lastSelectableRect = currentSelectableRect;
            currentSelectableRect = selectable.GetComponent<RectTransform>();

            if (lastSelectableRect == null)
            {
                lastSelectableRect = currentSelectableRect;
            }
            selectedDisplayRect.position = lastSelectableRect.position;
            selectedDisplayRect.sizeDelta = lastSelectableRect.sizeDelta;
            selectableLerpTimer = 0;
        }

        public double SecsToBeats(double s)
        {
            return s / 60f * bpm;
        }

        public double BeatsToSecs(double beats, float bpm)
        {
            return beats / bpm * 60f;
        }

        public float GetPositionFromBeat(float startBeat, float length)
        {
            float a = MathUtils.Normalize((float)songPosBeat, startBeat, startBeat + length);
            return a;
        }

        public void SkipToBeat(double beat)
        {
            if (songPosBeat >= beat) return;
            double seconds = BeatsToSecs(beat, bpm);
            time = seconds;
            songPos = time + offset;
            songPosBeat = SecsToBeats(songPos);
            musicSource.time = (float)time;
        }

        public void CreatePressed()
        {
            if (exiting) return;
            exiting = true;
            GlobalGameManager.PlayOpenFile = null;
            GlobalGameManager.LoadScene("Editor");
            SoundByte.PlayOneShot("ui/UIEnter");
        }

        public void PlayPressed()
        {
            SoundByte.PlayOneShot("ui/UISelect");
            // temp: open file browser then go into quickplay
            OpenQuickplayFileDialog();
            // go into the play mode menu
        }

        void OpenQuickplayFileDialog()
        {
            var extensions = new[]
            {
                new ExtensionFilter("Heaven Studio Remix File ", new string[] { "riq" }),
            };

#if HEAVENSTUDIO_PROD && !UNITY_EDITOR
            string lvpath = "";
            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                lvpath = Application.dataPath + "/../Levels/";
                if (!Directory.Exists(lvpath))
                {
                    Directory.CreateDirectory(lvpath);
                }
            }
            StandaloneFileBrowser.OpenFilePanelAsync("Open Remix", lvpath, extensions, false, (string[] paths) =>
#else
            StandaloneFileBrowser.OpenFilePanelAsync("Open Remix", "", extensions, false, (string[] paths) =>
#endif
            {
                var path = Path.Combine(paths);
                if (path == string.Empty)
                {
                    SoundByte.PlayOneShot("ui/UICancel");
                    return;
                }

                try
                {
                    RiqFileHandler.ClearCache();
                    string tmpDir = RiqFileHandler.ExtractRiq(path);
                    Debug.Log("Imported RIQ successfully!");
                    RiqBeatmap beatmap = RiqFileHandler.ReadRiq();
                    GlobalGameManager.PlayOpenFile = path;
                    chartTitleText.text = beatmap["remixtitle"];
                    chartMapperText.text = beatmap["remixauthor"];
                    chartDescText.text = beatmap["remixdesc"];
                    chartIdolText.text = "♪ " + beatmap["idolcredit"];
                    chartStyleText.text = $"Recommended Control Style: {beatmap["playstyle"].ToString()}";
                    flashWarning.SetActive(beatmap["accessiblewarning"]);

                    if (PersistentDataManager.gameSettings.perfectChallengeType == PersistentDataManager.PerfectChallengeType.On)
                    {
                        campaignOption.sprite = campaignOn;
                    }
                    else
                    {
                        campaignOption.sprite = campaignOff;
                    }

                    // waitingForButtonUp = true;
                    playPanelRevealTime = Time.realtimeSinceStartup + 0.5f;
                    playPanel.SetActive(true);
                    playMenuRevealed = true;
                    SoundByte.PlayOneShot("ui/UISelect");
                }
                catch (System.Exception e)
                {
                    Debug.Log($"Error importing RIQ: {e.Message}");
                    Debug.LogException(e);
                    GlobalGameManager.ShowErrorMessage("Error Loading RIQ", e.Message + "\n\n" + e.StackTrace);
                    return;
                }
            });
        }

        public void PlayPanelAccept()
        {
            if (playPanelRevealTime > Time.realtimeSinceStartup) return;
            if (exiting) return;
            exiting = true;
            SoundByte.PlayOneShot("ui/UIEnter");
            PersistentDataManager.SaveSettings();
            GlobalGameManager.LoadScene("Game", 0.35f, -1);
        }

        public void PlayPanelBack()
        {
            RiqFileHandler.ClearCache();
            SoundByte.PlayOneShot("ui/UICancel");
            PersistentDataManager.SaveSettings();
            playPanel.SetActive(false);
            playMenuRevealed = false;
            waitingForButtonUp = false;
        }

        public void ToggleCampaign()
        {
            SoundByte.PlayOneShot("ui/UIOption");
            if (PersistentDataManager.gameSettings.perfectChallengeType == PersistentDataManager.PerfectChallengeType.On)
            {
                PersistentDataManager.gameSettings.perfectChallengeType = PersistentDataManager.PerfectChallengeType.Off;
                campaignOption.sprite = campaignOff;
            }
            else
            {
                PersistentDataManager.gameSettings.perfectChallengeType = PersistentDataManager.PerfectChallengeType.On;
                campaignOption.sprite = campaignOn;
            }
        }

        public void SocialsPressed()
        {
//             snsRevealed = true;
// #if HEAVENSTUDIO_PROD
//             snsVersionText.text = GlobalGameManager.friendlyReleaseName;
// #else
//             snsVersionText.text = GlobalGameManager.buildTime;
// #endif
//             snsPanel.SetActive(true);
            SoundByte.PlayOneShot("ui/UISelect");
            Application.OpenURL("https://linktr.ee/RHeavenStudio");
            // show a panel with our SNS links
        }

        public void SocialsClose()
        {
            snsRevealed = false;
            snsPanel.SetActive(false);
            SoundByte.PlayOneShot("ui/UICancel");
        }

        public void SettingsPressed()
        {
            if (!settingsPanel.IsOpen)
            {
                settingsPanel.SwitchSettingsDialog();
                SoundByte.PlayOneShot("ui/UISelect");
            }
            // notes:
            //  gameplay settings currently don't work due to the overlay preview requiring the screen composition setup from a gameplay prefab
            //  adding the attract screen will fix this since we'd need to add that prefab for it anyways
        }

        public void SettingsClose()
        {
            if (settingsPanel.IsOpen)
            {
                settingsPanel.SwitchSettingsDialog();
                SoundByte.PlayOneShot("ui/UICancel");
            }
        }

        public void QuitPressed()
        {
            SoundByte.PlayOneShot("ui/PauseQuit");
            PersistentDataManager.SaveSettings();
            Application.Quit();
        }
    }
}

