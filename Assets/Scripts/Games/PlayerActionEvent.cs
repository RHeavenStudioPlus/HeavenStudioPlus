using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using HeavenStudio.Util;
using Starpelly;

namespace HeavenStudio.Games
{

    public class PlayerActionEvent : PlayerActionObject
    {
        public delegate void ActionEventCallback();
        public delegate void ActionEventCallbackState(float state);

        public delegate void ActionEventCallbackSelf(PlayerActionEvent evt);

        public ActionEventCallbackState OnHit; //Function to trigger when an input has been done perfectly
        public ActionEventCallback OnMiss; //Function to trigger when an input has been missed
        public ActionEventCallback OnBlank; //Function to trigger when an input has been recorded while this is pending

        public ActionEventCallbackSelf OnDestroy; //Function to trigger whenever this event gets destroyed. /!\ Shouldn't be used for a minigame! Use OnMiss instead /!\

        public float startBeat;
        public float timer;

        public bool canHit  = true; //Indicates if you can still hit the cue or not. If set to false, it'll guarantee a miss
        public bool enabled = true; //Indicates if the PlayerActionEvent is enabled. If set to false, it'll not trigger any events and destroy itself AFTER the event

        public bool autoplayOnly = false; //Indicates if the input event only triggers when it's autoplay. If set to true, NO Miss or Blank events will be triggered when you're not autoplaying.

        public bool noAutoplay = false; //Indicates if this PlayerActionEvent is recognized by the autoplay. /!\ Overrides autoPlayOnly /!\

        // I don't know why we would ever need the noAutoplay setting, but we never know!

        public InputType inputType;

        public void setHitCallback(ActionEventCallbackState OnHit)
        { 
            this.OnHit = OnHit;
        }

        public void setMissCallback(ActionEventCallback OnMiss)
        {
            this.OnMiss = OnMiss;
        }

        public void Enable()  { enabled = true; }
        public void Disable() { enabled = false; }

        public void CanHit(bool canHit)
        {
            this.canHit = canHit;
        }

        public void Update()
        {
            if(!Conductor.instance.NotStopped()){CleanUp();} // If the song is stopped entirely in the editor, destroy itself as we don't want duplicates

            if (noAutoplay && autoplayOnly) autoplayOnly = false;
            if (noAutoplay && triggersAutoplay){ triggersAutoplay = false; }

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat,timer);
            StateCheck(normalizedBeat);

            if (normalizedBeat > Minigame.LateTime()) Miss();


            if (IsCorrectInput() && !autoplayOnly)
            {
                if (state.perfect)
                {
                    Hit(0f);
                }
                else if (state.early)
                {
                    Hit(-1f);
                }
                else if (state.late) 
                {
                    Hit(1f);
                }
                else
                {
                    Blank();
                }
            }
        } 

        public bool IsCorrectInput()
        {
            // This one is a mouthful but it's an evil good to detect the correct input
            // Forgive me for those input type names
            return (
                        //General inputs, both down and up
                        (PlayerInput.Pressed()              && inputType == InputType.STANDARD_DOWN)        ||
                        (PlayerInput.AltPressed()           && inputType == InputType.STANDARD_ALT_DOWN)    ||
                        (PlayerInput.GetAnyDirectionDown()  && inputType == InputType.DIRECTION_DOWN)       ||
                        (PlayerInput.PressedUp()            && inputType == InputType.STANDARD_UP)          ||
                        (PlayerInput.AltPressedUp()         && inputType == InputType.STANDARD_ALT_UP)      ||
                        (PlayerInput.GetAnyDirectionUp()    && inputType == InputType.DIRECTION_UP)         ||
                        //Specific directional inputs
                        (PlayerInput.GetSpecificDirectionDown(PlayerInput.DOWN)     && inputType == InputType.DIRECTION_DOWN_DOWN)  ||
                        (PlayerInput.GetSpecificDirectionDown(PlayerInput.UP)       && inputType == InputType.DIRECTION_UP_DOWN)    ||
                        (PlayerInput.GetSpecificDirectionDown(PlayerInput.LEFT)     && inputType == InputType.DIRECTION_LEFT_DOWN)  ||
                        (PlayerInput.GetSpecificDirectionDown(PlayerInput.RIGHT)    && inputType == InputType.DIRECTION_RIGHT_DOWN) ||

                        (PlayerInput.GetSpecificDirectionUp(PlayerInput.DOWN)       && inputType == InputType.DIRECTION_DOWN_UP)    ||
                        (PlayerInput.GetSpecificDirectionUp(PlayerInput.UP)         && inputType == InputType.DIRECTION_UP_UP)      ||
                        (PlayerInput.GetSpecificDirectionUp(PlayerInput.LEFT)       && inputType == InputType.DIRECTION_LEFT_UP)    ||
                        (PlayerInput.GetSpecificDirectionUp(PlayerInput.RIGHT)      && inputType == InputType.DIRECTION_RIGHT_UP)
                   );
        }


        //For the Autoplay
        public override void OnAce()
        {
            Hit(0f);
        }

        //The state parameter is either -1 -> Early, 0 -> Perfect, 1 -> Late
        public void Hit(float state) 
        {
            if (OnHit != null && enabled)
            {
                if(canHit)
                {
                    OnHit(state);
                    CleanUp();
                } else
                {
                    OnBlank();
                }
                
            }
    
        }

        public void Miss()
        {
            if (OnMiss != null && enabled && !autoplayOnly)
            {
                OnMiss();
            }

            CleanUp();
        }

        public void Blank()
        {
            if(OnBlank != null && enabled && !autoplayOnly)
            {
                OnBlank();
            }
        }

        public void CleanUp()
        {
            OnDestroy(this);
            Destroy(this.gameObject);
        }

    }
}