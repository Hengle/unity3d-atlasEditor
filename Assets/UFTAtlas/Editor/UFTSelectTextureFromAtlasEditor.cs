using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(UFTSelectTextureFromAtlas))]
public class UFTSelectTextureFromAtlasEditor : Editor {
	SerializedProperty textureIndex;
	SerializedProperty atlasMetadata;
	
	private bool debug=false;
	
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
		
		UFTSelectTextureFromAtlas script=((UFTSelectTextureFromAtlas)target);
		if (!script.isUV2Empty()){			
			if (GUILayout.Button("restore original uv")){
				((UFTSelectTextureFromAtlas)target).restoreOriginalUVS();
			}
		}
		
		//debug
		debug=EditorGUILayout.Toggle("UV Debug mode" ,debug);
		
		if (debug){
			EditorGUILayout.LabelField("==========================================");
			EditorGUILayout.LabelField("uv2:");
			Vector2[] uv2=((UFTSelectTextureFromAtlas)target).gameObject.GetComponent<MeshFilter>().mesh.uv2;
			foreach(Vector2 v in uv2){
				EditorGUILayout.Vector2Field("",v);	
			}
			
			
			EditorGUILayout.LabelField("==========================================");
			EditorGUILayout.LabelField("uv:");
			Vector2[] uv=((UFTSelectTextureFromAtlas)target).gameObject.GetComponent<MeshFilter>().mesh.uv;
			foreach(Vector2 v in uv){
				EditorGUILayout.Vector2Field("",v);	
			}
		}
    }
	
}
