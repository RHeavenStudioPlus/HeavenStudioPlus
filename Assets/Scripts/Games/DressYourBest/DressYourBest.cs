using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class PcoDressYourBestLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("dressYourBest", "Dress Your Best", "d593dd", false, false, new List<GameAction>()
            {
                new GameAction("start interval", "Start Interval")
                {
                    defaultLength = 4f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    }
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate { var e = eventCaller.currentEntity; ForkLifter.instance.BackgroundColor(e.beat, e.length, e["start"], e["end"], e["ease"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("start", DressYourBest.DefaultBGColor, "Start Color", "Set the color at the start of the event."),
                        new Param("end", DressYourBest.DefaultBGColor, "End Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    },
                },
            }
            );
        }
    }
}

namespace HeavenStudio.Games
{
    /// This class handles the minigame logic.
    /// Minigame inherits directly from MonoBehaviour, and adds Heaven Studio specific methods to override.
    public class DressYourBest : Minigame
    {
        public readonly static Color DefaultBGColor = new(0.84f, 0.58f, 0.87f);
    }
}