using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;



public class UFTAtlasUtil  {

	
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
	
	public static Dictionary<System.Type,List<UFTObjectOnScene>> getObjectOnSceneByAtlas(UFTAtlasMetadata atlasMetadata, System.Type additionalType=null){
		Dictionary<System.Type,List<UFTObjectOnScene>> result = new Dictionary<System.Type, List<UFTObjectOnScene>>();
		
		HashSet<System.Type> unwantedTypes=new HashSet<System.Type>();
		
		Object[] objects = GameObject.FindObjectsOfType( typeof( Component ) );
		foreach( Component component in objects )
		{
			if (unwantedTypes.Contains(component.GetType()))
				continue;
			
			UFTObjectOnScene objectOnScene=new UFTObjectOnScene(component);
			foreach (FieldInfo prop in component.GetType().GetFields(BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic)) {				
				if (prop.FieldType == atlasMetadata.GetType() || prop.FieldType == additionalType ){
					objectOnScene.addProperty(prop);
				}
			} 
			
			if (objectOnScene.propertyList.Count == 0 ){
				unwantedTypes.Add(component.GetType());	
			} else {
				List<UFTObjectOnScene> list;
				if ( !result.ContainsKey(component.GetType())){
					list = new List<UFTObjectOnScene>();
					Dictionary<System.Type,List<UFTObjectOnScene>> dict = new Dictionary<System.Type, List<UFTObjectOnScene>>();
					result.Add(component.GetType(),list);
				} else {
					list = result[component.GetType()];
				}
				list.Add(objectOnScene);
			}
			
		}
		
		return result;
	}
	
	
}
