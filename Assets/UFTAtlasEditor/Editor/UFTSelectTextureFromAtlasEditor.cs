using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(UFTSelectTextureFromAtlas))]
public class UFTSelectTextureFromAtlasEditor : Editor {
	
	
	SerializedProperty atlasEntryMetadata;
	
	private bool debug=false;
	
	void OnEnable () {
    	atlasEntryMetadata = serializedObject.FindProperty("atlasEntryMetadataInst");
    }
	
	public override void OnInspectorGUI(){
		
		UFTSelectTextureFromAtlas targetObj=(UFTSelectTextureFromAtlas)target;
		targetObj.atlasMetadata =(UFTAtlasMetadata) EditorGUILayout.ObjectField(targetObj.atlasMetadata,typeof(UFTAtlasMetadata),false);
		
		if (targetObj.atlasMetadata!=null){			
			int newValue=EditorGUILayout.IntSlider(targetObj.textureIndex ,0,targetObj.atlasMetadata.entries.Length-1);								
			if (newValue != targetObj.textureIndex){
				targetObj.textureIndex=newValue;
				if (atlasEntryMetadata !=null)
					serializedObject.Update();
			}
		}
		
		GUI.enabled=false;
		if (atlasEntryMetadata !=null)
			EditorGUILayout.PropertyField(atlasEntryMetadata,true);
		GUI.enabled=true;
		
				
		if (!targetObj.isUV2Empty()){			
			if (GUILayout.Button("restore original uv")){
				targetObj.restoreOriginalUVS();
			}
		}
		
		//debug
		debug=EditorGUILayout.Toggle("UV Debug mode" ,debug);
		
		if (debug){
			EditorGUILayout.LabelField("==========================================");
			EditorGUILayout.LabelField("uv (actual uv):");
			Vector2[] uv=UFTMeshUtil.getObjectMesh(targetObj.gameObject).uv;
			foreach(Vector2 v in uv){
				EditorGUILayout.Vector2Field("",v);	
			}
			
			EditorGUILayout.LabelField("==========================================");
			EditorGUILayout.LabelField("uv2 (original uv):");
			Vector2[] uv2=UFTMeshUtil.getObjectMesh(targetObj.gameObject).uv2;
			foreach(Vector2 v in uv2){
				EditorGUILayout.Vector2Field("",v);	
			}			
		}
    }
	
}
