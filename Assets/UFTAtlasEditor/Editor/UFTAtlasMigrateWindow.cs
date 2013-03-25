using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class UFTAtlasMigrateWindow : EditorWindow {
	public UFTAtlasMetadata atlasMetadataFrom;
	public UFTAtlasMetadata atlasMetadataTo;
	
	private static string EDITORPREFS_ATLASMIGRATION_FROM="uftAtlasEditor.atlasFrom";
	private static string EDITORPREFS_ATLASMIGRATION_TO="uftAtlasEditor.atlasTo";
	
	
	Dictionary<System.Type,List<UFTObjectOnScene>>  objectList;
	
	
	private static string HELP_STRING = "atlas metadatas must be different and point to the objects." +
			"\nAll objects which use source metatadata will be updated" +
			"\nIf this objects will has entryMetadat this links will be changed according to names";
	
    [MenuItem ("Window/UFT Atlas Migration")]
    static void CreateWindow () {
        UFTAtlasMigrateWindow window=(UFTAtlasMigrateWindow) EditorWindow.GetWindow(typeof(UFTAtlasMigrateWindow));        
		
		string atlasFrom=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_FROM,null);
		if (atlasFrom != null){
			window.atlasMetadataFrom = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasFrom,typeof(UFTAtlasMetadata));
			if (window.atlasMetadataFrom !=null){
				window.updateObjectList();	
			}
		}
		string atlasTo=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_TO,null);
		if (atlasFrom != null)
			window.atlasMetadataTo = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasTo,typeof(UFTAtlasMetadata));
		
		window.checkIsAllPropertiesSet();
	}
	
	void OnWizardCreate(){
		UFTAtlasUtil.migrateAtlasMatchEntriesByName(atlasMetadataFrom,atlasMetadataTo);		
		
		EditorPrefs.SetString(EDITORPREFS_ATLASMIGRATION_TO,AssetDatabase.GetAssetPath(atlasMetadataTo));
		Debug.Log("done");
	}
	
	void OnGUI(){
		EditorGUILayout.LabelField(HELP_STRING,GUI.skin.GetStyle( "Box" ) );
		
		
		UFTAtlasMetadata newMeta=(UFTAtlasMetadata) EditorGUILayout.ObjectField("source atlas",atlasMetadataFrom,typeof(UFTAtlasMetadata),false);
		if (newMeta != atlasMetadataFrom ){
			atlasMetadataFrom= newMeta;
			EditorPrefs.SetString(EDITORPREFS_ATLASMIGRATION_FROM,AssetDatabase.GetAssetPath(atlasMetadataFrom));
			if (newMeta!=null){
				updateObjectList();					
			}
		}
		
		
		displayObjectListIfExist();				
		
		
		
	}
	
	Vector2 scrollPosition;
	
	void displayObjectListIfExist ()
	{
		
		if (objectList !=null && objectList.Count > 0){
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			foreach (System.Type objectType in objectList.Keys) {
				EditorGUILayout.LabelField(objectType.ToString()+":");
				List<UFTObjectOnScene> objects=objectList[objectType];
				
				foreach (UFTObjectOnScene obj in objects){
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Separator();
					if (GUILayout.Button(""+obj.component.name,GUILayout.Width(250)))
						Selection.activeObject = obj.component.gameObject;
					
					GUILayout.FlexibleSpace();
					EditorGUILayout.BeginVertical();
						foreach (FieldInfo field in obj.propertyList) {
							EditorGUILayout.Toggle(field.Name,true);	
						}
						
					EditorGUILayout.EndVertical();
				
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();
		
		}
	}
	
	

	void updateObjectList ()
	{
		if (objectList == null)
			objectList = new Dictionary<System.Type, List<UFTObjectOnScene>>();
		
		objectList = UFTAtlasUtil.getObjectOnSceneByAtlas(atlasMetadataFrom,typeof(UFTAtlasEntryMetadata));
	}
	
	
	
	
	
	public void checkIsAllPropertiesSet ()
	{
		//isValid=((atlasMetadataFrom != null) && (atlasMetadataTo != null) && (atlasMetadataTo != atlasMetadataFrom));
	}
	
	
	void OnWizardUpdate(){
		//checkIsAllPropertiesSet ();	
		
				
	}
	
}
