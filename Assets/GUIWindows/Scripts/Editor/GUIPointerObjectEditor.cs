using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rellac.Windows
{
	[CustomEditor(typeof(GUIPointerObject))]
	[CanEditMultipleObjects]
	public class GUIPointerObjectEditor : Editor 
	{
		
		SerializedProperty onPointerUp;
		SerializedProperty onPointerDown;
		SerializedProperty onPointerEnter;
		SerializedProperty onPointerExit;
		
		void OnEnable()
		{
			onPointerUp = serializedObject.FindProperty("onPointerUp");
			onPointerDown = serializedObject.FindProperty("onPointerDown");
			onPointerEnter = serializedObject.FindProperty("onPointerEnter");
			onPointerExit = serializedObject.FindProperty("onPointerExit");
		}
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(onPointerUp);
			EditorGUILayout.PropertyField(onPointerDown);
			EditorGUILayout.PropertyField(onPointerEnter);
			EditorGUILayout.PropertyField(onPointerExit);
			serializedObject.ApplyModifiedProperties();
		}
	}
}