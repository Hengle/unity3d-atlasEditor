using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;


public class UFTAtlasUtil : MonoBehaviour {

	
	public static void migrateAtlasMatchEntriesByName(UFTAtlasMetadata atlasFrom, UFTAtlasMetadata atlasTo){	
		
		UFTSelectTextureFromAtlas[] objects=(UFTSelectTextureFromAtlas[]) GameObject.FindObjectsOfType(typeof(UFTSelectTextureFromAtlas));
		Debug.Log("heheh "+objects[0].name);
		
		
		/*
		 * 
	    Object[] objects = EditorUtility.CollectDependencies( new []{atlasFrom});
		for (int i = 0; i < objects.Length; i++) {
			Debug.Log("object "+objects[i].name + "    "+ objects[i].GetType());
			
		}
		*/
		
	}
	
}
