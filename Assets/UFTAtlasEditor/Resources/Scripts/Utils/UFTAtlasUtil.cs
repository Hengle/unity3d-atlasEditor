using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public class UFTAtlasUtil : MonoBehaviour {

	
	public static void migrateAtlasMatchEntriesByName(UFTAtlasMetadata atlasFrom, UFTAtlasMetadata atlasTo){
		Object[] objects= Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour));
		for (int i = 0; i < objects.Length; i++) {
			PropertyInfo[] props= objects[i].GetType().GetProperties();
			for (int j = 0; j < props.Length; j++) {
				if (props[j].GetType().Equals(typeof(UFTAtlasMetadata))){
					Debug.Log("object "+objects[i].name+" has UFTAtlasMetadataProperty propName="+props[j].Name);	
				} else if (props[j].GetType().Equals(typeof(UFTAtlasEntryMetadata))) {
					Debug.Log("object "+objects[i].name+" has UFTAtlasEntryMetadata propName="+props[j].Name);
				}
			}
		}	
		
		
	}
	
}