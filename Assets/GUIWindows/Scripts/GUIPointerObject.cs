using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rellac.Windows
{
	/// <summary>
	/// Contains events referencing IPointerHandlers
	/// </summary>
	public class GUIPointerObject : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// Fires when a pointer up is detected
		/// </summary>
		[HideInInspector]
		[Tooltip("Fires when a pointer up is detected")]
		public UnityEvent onPointerUp = null;
		/// <summary>
		/// Fires when a pointer down is detected
		/// </summary>
		[HideInInspector]
		[Tooltip("Fires when a pointer down is detected")]
		public UnityEvent onPointerDown = null;
		/// <summary>
		/// Fires when a pointer enter is detected
		/// </summary>
		[HideInInspector]
		[Tooltip("Fires when a pointer enter is detected")]
		public UnityEvent onPointerEnter = null;
		/// <summary>
		/// Fires when a pointer exit is detected
		/// </summary>
		[HideInInspector]
		[Tooltip("Fires when a pointer exit is detected")]
		public UnityEvent onPointerExit = null;

		public void OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null)
			{
				onPointerUp.Invoke();
			}
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null)
			{
				onPointerDown.Invoke();
			}
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null)
			{
				onPointerEnter.Invoke();
			}
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null)
			{
				onPointerExit.Invoke();
			}
		}
	}
}