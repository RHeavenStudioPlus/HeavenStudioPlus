using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// This is a custom button class that inherits from the default button. This allows us to have seperate events for left and right click, utilising all of the handy features of Button.
/// </summary>

public class UIButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEngine.Events.UnityEvent onRightClick = new UnityEngine.Events.UnityEvent();
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Invoke the left click event
            base.OnPointerClick(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Invoke the right click event
            onRightClick.Invoke();
        }
    }

    public bool isHovering = false;
    public bool isHolding = false;
    public bool wasHolding = false;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        base.OnPointerExit(eventData);
    }

    public void Update()
    {
        if (Input.GetMouseButton(1) && isHovering) isHolding = true;
        if (!Input.GetMouseButton(1)) isHolding = false;

        if (isHolding) DoStateTransition(Button.SelectionState.Pressed, false);
        if (isHolding != wasHolding) DoStateTransition(currentSelectionState, false);

        wasHolding = isHolding;
    }
}