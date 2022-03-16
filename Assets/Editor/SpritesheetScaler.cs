using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 
public class SpritesheetScaler : EditorWindow {
 
   Object source;
   int multiplier = 1;
 
   // Creates a new option in "Windows"
   [MenuItem ("Window/Scale spritesheet pivots and slices")]
   static void Init () {
     // Get existing open window or if none, make a new one:
     SpritesheetScaler window = (SpritesheetScaler)EditorWindow.GetWindow (typeof (SpritesheetScaler));
     window.Show();
   }
 
   void OnGUI () {
     GUILayout.BeginHorizontal ();
     GUILayout.Label ("Source texture:", EditorStyles.boldLabel);
     source = EditorGUILayout.ObjectField(source, typeof(Texture2D), false, GUILayout.Width(220));
     GUILayout.EndHorizontal ();
 
     GUILayout.BeginHorizontal ();
     GUILayout.Label ("Multiplier:", EditorStyles.boldLabel);
     multiplier = EditorGUILayout.IntField(multiplier, GUILayout.Width(220));
     GUILayout.EndHorizontal ();
   
     GUILayout.Space (25f);
     if (GUILayout.Button ("Scale pivots and slices")) {
       ScalePivotsAndSlices();
     }
   }
 
   void ScalePivotsAndSlices()
   {
     if (!source || (multiplier <= 0)) {
       Debug.Log("Missing one object");
       return;
     }
   
     if (source.GetType () != typeof(Texture2D)) {
       Debug.Log (source + "needs to be Texture2D!");
       return;
     }
   
     string sourcePath = AssetDatabase.GetAssetPath(source);
     TextureImporter ti1 = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
     bool wasReadable = ti1.isReadable;
     ti1.isReadable = true;

     ti1.spritePixelsPerUnit *= multiplier;
 
     List <SpriteMetaData> newData = new List <SpriteMetaData> ();
   
     Debug.Log ("Amount of slices found: " + ti1.spritesheet.Length);
   
     for (int i = 0; i < ti1.spritesheet.Length; i++) {
       SpriteMetaData d = ti1.spritesheet[i];
       d.rect = ScaleRect(d.rect, multiplier);
       d.border *= multiplier;
       newData.Add(d);
     }

     ti1.spritesheet = newData.ToArray();

     ti1.isReadable = wasReadable;
   
     AssetDatabase.ImportAsset(sourcePath, ImportAssetOptions.ForceUpdate);
   }

   Rect ScaleRect(Rect source, int mult)
   {
       var newRect = new Rect();
       newRect.Set(source.x * mult, source.y * mult, source.width * mult, source.height * mult);
       return newRect;
   }
}
