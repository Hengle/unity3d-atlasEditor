using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(UFTSelectTextureFromAtlas))]
public class UFTSelectTextureFromAtlasEditor : Editor {
	SerializedProperty textureIndex;
	SerializedProperty atlasMetadata;
	
	void OnEnable () {
        // Setup the SerializedProperties
        textureIndex = serializedObject.FindProperty ("textureIndex");  
		atlasMetadata= serializedObject.FindProperty("atlasMetadata");
    }
	
	public override void OnInspectorGUI(){
		
		UFTSelectTextureFromAtlas planeObject=((UFTSelectTextureFromAtlas) target);
		if (planeObject.atlasMetadata!=null){
			
			EditorGUILayout.IntSlider(textureIndex,0,planeObject.atlasMetadata.entries.Length-1);		
		}
		
		EditorGUILayout.PropertyField(atlasMetadata);
		serializedObject.ApplyModifiedProperties ();
		
		if (GUI.changed){
			((UFTSelectTextureFromAtlas)target).updateUV();			
		}
    }
	
}
