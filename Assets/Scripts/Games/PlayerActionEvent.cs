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
        public delegate void ActionEventCallbackState(int state);

        public ActionEventCallbackState OnHit; //Function to trigger when an input has been done perfectly
        public ActionEventCallback OnMiss; //Function to trigger when an input has been missed
        public ActionEventCallback OnBlank; //Function to trigger when an input has been recorded while this is pending

        public float startBeat;
        public float timer;

        public bool canHit  = true;
        public bool enabled = true;

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

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat,timer);
            StateCheck(normalizedBeat);

            if (normalizedBeat > Minigame.LateTime()) Miss();


            if (IsCorrectInput())
            {
                if (state.perfect)
                {
                    Hit(1);
                }
                else if (state.early)
                {
                    Hit(0);
                }
                else if (state.late) 
                {
                    Hit(2);
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
            Hit(1);
        }

        //The state parameter is either 0 -> Early, 1 -> Perfect, 2 -> Late
        public void Hit(int state) 
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
            if (OnMiss != null && enabled)
            {
                OnMiss();
                CleanUp();
            }
        }

        public void Blank()
        {
            if(OnBlank != null && enabled)
            {
                OnBlank();
            }
        }

        public void CleanUp()
        {
            Destroy(this.gameObject);
        }

    }
}