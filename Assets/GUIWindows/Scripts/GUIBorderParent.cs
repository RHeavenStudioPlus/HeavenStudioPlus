using UnityEngine;

namespace Rellac.Windows
{
	/// <summary>
	/// Contains a reference to all GUIWindowHandle objects below this Transform for ease of referencing
	/// </summary>
	public class GUIBorderParent : MonoBehaviour
	{
		private GUIWindowHandle[] handles;
		// Use this for initialization
		void Start()
		{
			handles = GetComponentsInChildren<GUIWindowHandle>();
		}

		/// <summary>
		/// Toggle interactivity of handles
		/// </summary>
		/// <param name="input">is interactive</param>
		public void SetIsLocked(bool input)
		{
			for (int i = 0; i < handles.Length; i++)
			{
				handles[i].SetIsLocked(input);
			}
		}
	}
}