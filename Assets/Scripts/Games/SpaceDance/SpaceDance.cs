using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbSpaceDanceLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("spaceDance", "Space Dance \n<color=#eb5454>[WIP don't use]</color>", "000000", false, false, new List<GameAction>()
            {
                new GameAction("turn right",                 delegate { SpaceDance.instance.DoTurnRight(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("sit down",                   delegate { SpaceDance.instance.DoSitDown(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("punch",                      delegate { SpaceDance.instance.DoPunch(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("bop",                      delegate { SpaceDance.instance.Bop(eventCaller.currentEntity.beat); }, 1.0f, false),
            });
        }
    }
}

namespace HeavenStudio.Games
{
    // using Scripts_SpaceDance;
    public class SpaceDance : Minigame
    {
        public Animator DancerP;
        public Animator Dancer1;
        public Animator Dancer2;
        public Animator Dancer3;
        public Animator Gramps;
        public GameObject Player;

        public static SpaceDance instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

            if (PlayerInput.Pressed() && !IsExpectingInputNow())
            {
            
            }
        }

        public void DoTurnRight(float beat)
        {
            ScheduleInput(beat, 1f, InputType.STANDARD_DOWN, RightSuccess, RightMiss, RightEmpty);
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("spaceDance/turn1_sound", beat),
            new MultiSound.Sound("spaceDance/turn1_dancers", beat),
            new MultiSound.Sound("spaceDance/turn2_dancers", beat),
            });

            BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { DancerP.Play("TurnRightStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer1.Play("TurnRightStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer2.Play("TurnRightStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer3.Play("TurnRightStart", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer1.Play("TurnRightDo", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer2.Play("TurnRightDo", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer3.Play("TurnRightDo", -1, 0);}),
                });

        }

        public void DoSitDown(float beat)
        {
            ScheduleInput(beat, 1f, InputType.STANDARD_DOWN, SitSuccess, SitMiss, SitEmpty);
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("spaceDance/sit1_sound", beat),
            new MultiSound.Sound("spaceDance/sit1_dancers", beat),
            new MultiSound.Sound("spaceDance/sit2_sound", beat + 0.5f),
            new MultiSound.Sound("spaceDance/sit2_dancers", beat + 0.5f),
            new MultiSound.Sound("spaceDance/sit3_dancers", beat + 1f),
            });

            BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { DancerP.Play("SitDownStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer1.Play("SitDownStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer2.Play("SitDownStart", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer3.Play("SitDownStart", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer1.Play("SitDownDo", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer2.Play("SitDownDo", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer3.Play("SitDownDo", -1, 0);}),
                });

        }

        public void DoPunch(float beat)
        {
            ScheduleInput(beat, 1.5f, InputType.STANDARD_DOWN, PunchSuccess, PunchMiss, PunchEmpty);
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("spaceDance/punch1_sound", beat),
            new MultiSound.Sound("spaceDance/punch1_dancers", beat),
            new MultiSound.Sound("spaceDance/punch1_sound", beat + 0.5f),
            new MultiSound.Sound("spaceDance/punch1_dancers", beat + 0.5f),
            new MultiSound.Sound("spaceDance/punch1_sound", beat + 1f),
            new MultiSound.Sound("spaceDance/punch1_dancers", beat + 1f),
            new MultiSound.Sound("spaceDance/punch2_dancers", beat + 1.5f),
            });

            BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { DancerP.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer1.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer2.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer3.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat + 0.5f,     delegate { DancerP.Play("PunchStartOuter", -1, 0);}),
                new BeatAction.Action(beat + 0.5f,     delegate { Dancer1.Play("PunchStartOuter", -1, 0);}),
                new BeatAction.Action(beat + 0.5f,     delegate { Dancer2.Play("PunchStartOuter", -1, 0);}),
                new BeatAction.Action(beat + 0.5f,     delegate { Dancer3.Play("PunchStartOuter", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { DancerP.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer1.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer2.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat + 1f,     delegate { Dancer3.Play("PunchStartInner", -1, 0);}),
                new BeatAction.Action(beat + 1.5f,     delegate { Dancer1.Play("PunchDo", -1, 0);}),
                new BeatAction.Action(beat + 1.5f,     delegate { Dancer2.Play("PunchDo", -1, 0);}),
                new BeatAction.Action(beat + 1.5f,     delegate { Dancer3.Play("PunchDo", -1, 0);}),
                });

        }

        public void Bop(float beat)
        {
            BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { DancerP.Play("Bop", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer1.Play("Bop", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer2.Play("Bop", -1, 0);}),
                new BeatAction.Action(beat,     delegate { Dancer3.Play("Bop", -1, 0);}),
                });

        }

        public void RightSuccess(PlayerActionEvent caller, float state)
            {
            Jukebox.PlayOneShotGame("spaceDance/right2_sound");
            DancerP.Play("TurnRightDo", -1, 0);
             }

        public void RightMiss(PlayerActionEvent caller)
            {

             }

        public void RightEmpty(PlayerActionEvent caller)
            {

             }

        public void SitSuccess(PlayerActionEvent caller, float state)
            {
            Jukebox.PlayOneShotGame("spaceDance/sit3_sound");
            DancerP.Play("SitDownDo", -1, 0);
             }

        public void SitMiss(PlayerActionEvent caller)
            {

             }

        public void SitEmpty(PlayerActionEvent caller)
            {

             }

        public void PunchSuccess(PlayerActionEvent caller, float state)
            {
            Jukebox.PlayOneShotGame("spaceDance/punch2_sound");
            DancerP.Play("PunchDo", -1, 0);
             }

        public void PunchMiss(PlayerActionEvent caller)
            {

             }

        public void PunchEmpty(PlayerActionEvent caller)
            {

             }


    }
}