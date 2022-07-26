using UnityEngine;
using UnityEngine.Events;

namespace Rellac.Windows
{
	/// <summary>
	/// Script to handle moving windows
	/// </summary>
	public class GUIWindowMover : GUIPointerObject
	{
		/// <summary>
		/// Window to move
		/// </summary>
		[Tooltip("Window to move")]
		[SerializeField] private RectTransform parentWindow = null;
		/// <summary>
		/// Mover is locked and unusable
		/// </summary>
		[Tooltip("Mover is locked and unusable")]
		[SerializeField] private bool isLocked = false;
		/// <summary>
		/// Fires when a window has been moved
		/// </summary>
		[Tooltip("Fires when a window has been moved")]
		[SerializeField] private UnityEvent onWindowMoved = null;

		private Vector2 mouseOffset;
		private bool isGrabbed = false;

		void Start()
		{
			onPointerDown.AddListener(SetIsGrabbed);
		}

		void Update()
		{
			if (!isGrabbed || isLocked) return;

			parentWindow.position = (Vector2)GUIWindowUtils.MousePosition() + mouseOffset;
			if (Input.GetMouseButtonUp(0))
			{
				isGrabbed = false;
				if (onWindowMoved != null)
				{
					onWindowMoved.Invoke();
				}
			}
		}

		/// <summary>
		/// Toggle interactivity of handle
		/// </summary>
		/// <param name="input">is interactive</param>
		public void SetIsLocked(bool input)
		{
			isLocked = input;
			isGrabbed = false;
		}

		/// <summary>
		/// Trigger that window has started to be moved
		/// </summary>
		public void SetIsGrabbed()
		{
			mouseOffset = parentWindow.position - GUIWindowUtils.MousePosition();
			isGrabbed = true;
			parentWindow.SetAsLastSibling();
		}
	}
}