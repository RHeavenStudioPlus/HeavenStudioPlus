using UnityEngine;

namespace Rellac.Windows
{
	/// <summary>
	/// Simple script to destroy the target GameObject when window is closed
	/// </summary>
	public class GUIWindow : MonoBehaviour
	{
		/// <summary>
		/// Close window by destroying this GameObject
		/// </summary>
		public void CloseWindow()
		{
			Destroy(gameObject);
		}
	}
}