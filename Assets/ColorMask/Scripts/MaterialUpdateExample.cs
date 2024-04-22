using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialUpdateExample : MonoBehaviour {

	public Material material;
	public List<Renderer> renderers = new List<Renderer>();

	//If running in the unity editor store the original ColorMask color values so they can be reset when the Game stops running
	#if UNITY_EDITOR
		private Color redColorReset;
		private Color greenColorReset;
		private Color blueColorReset;
		private Color alphaColorReset;
		
		void Start ()
		{
			if (material != null)
			{
				//Store the original ColorMask color values
				redColorReset = material.GetColor("_RedColor");
				greenColorReset = material.GetColor("_GreenColor");
				blueColorReset = material.GetColor("_BlueColor");
				alphaColorReset = material.GetColor("_AlphaColor");
			}
		}
		
		void OnDestroy ()
		{
			if (material != null)
			{
				//Reset to the original ColorMask color values
				material.SetColor("_RedColor", redColorReset);
				material.SetColor("_GreenColor", greenColorReset);
				material.SetColor("_BlueColor", blueColorReset);
				material.SetColor("_AlphaColor", alphaColorReset);
			}
		}
	#endif


	void Update ()
	{
		//Esc Exit - PC Standalone Build
		#if UNITY_STANDALONE_WIN
		if (Input.GetKey("escape"))
            Application.Quit();
		#endif
		
		if (material != null)
		{
			//Stores a value of time scaled
			float value = Time.time * 0.12f;

			//Calculates cyclical invert and fade values between zero and one
			float invert = Mathf.Pow (Mathf.Min(1f + Mathf.Sin(value * Mathf.PI), 1f), 7.5f);
			float fade = Mathf.Max(invert, Mathf.Pow (Mathf.Min(1f + Mathf.Sin(value * 2f * Mathf.PI), 1f), 7.5f) );

			//Converts value to a number between 0 and 1 which is converted to a color
			//Also applies fade alpha value to red and green
			Color redColor = ConvertValueToColor (value % 1f) * new Color(1f, 1f, 1f, fade);
			Color greenColor = ConvertValueToColor ( (value + 0.03125f) % 1f) * new Color(0.5f, 0.5f, 0.5f, fade);
			Color blueColor = ConvertValueToColor ( (value + 0.75f) % 1f);
			Color alphaColor = ConvertValueToColor ( (value + 0.78125f) % 1f) * new Color(0.5f, 0.5f, 0.5f, 1f);

			//Invert set red and alpha colors when invert value approaches zero
			redColor = Color.Lerp(new Color(1f - redColor.r, 1f - redColor.g, 1f - redColor.b, redColor.a), redColor, invert);
			alphaColor = Color.Lerp(new Color(1f - alphaColor.r, 1f - alphaColor.g, 1f - alphaColor.b, alphaColor.a), alphaColor, invert);

			//Apply the new colors to the ColorMask material colors
			material.SetColor("_RedColor", redColor);
			material.SetColor("_GreenColor", greenColor);
			material.SetColor("_BlueColor", blueColor);
			material.SetColor("_AlphaColor", alphaColor);
		}

		//If a static gameObject which uses a ColorMask shader affects another static gameObject(s)' Precomputed Realtime GI, then this code is necessary to see the updated GI
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				RendererExtensions.UpdateGIMaterials(renderers[i]);
			}
		}
	}

	//This converts a value between 0 and 1 to a color between Red, Yellow, Green, Cyan, Blue and Magenta
	Color ConvertValueToColor (float value)
	{
		List<Color> Rainbow = new List<Color>();
		Rainbow.Add(Color.red);
		Rainbow.Add(Color.yellow);
		Rainbow.Add(Color.green);
		Rainbow.Add(Color.cyan);
		Rainbow.Add(Color.blue);
		Rainbow.Add(Color.magenta);
		Rainbow.Add(Color.red);

		float RainbowCount = Rainbow.Count - 1;
		float idfloat = value * RainbowCount;
		int id = (int)idfloat;

		return Color.Lerp(Rainbow[id], Rainbow[id + 1], (idfloat - (float)id) % 1f);
	}
}
