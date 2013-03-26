using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

public class UFTAtlasMigrateWindow : EditorWindow {
	public UFTAtlasMetadata atlasMetadataFrom;
	public UFTAtlasMetadata atlasMetadataTo;
	
	private static string EDITORPREFS_ATLASMIGRATION_FROM="uftAtlasEditor.atlasFrom";
	private static string EDITORPREFS_ATLASMIGRATION_TO="uftAtlasEditor.atlasTo";
	
	Dictionary<int,bool> checkedObjectsHash;
	bool DEFAULT_CHECKED_OBJECT_VALUE=true;
	
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

	void migrate ()
	{
		throw new System.NotImplementedException ();
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
				printHeader();
				EditorGUILayout.Separator();
				List<UFTObjectOnScene> objects=objectList[objectType];
				
				foreach (UFTObjectOnScene obj in objects){
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Separator();
					if (GUILayout.Button(""+obj.component.name,GUILayout.Width(250)))
						Selection.activeObject = obj.component.gameObject;
					
					//GUILayout.FlexibleSpace();
					EditorGUILayout.BeginVertical();
						foreach (FieldInfo field in obj.propertyList) {
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("["+field.Name+"]",GUILayout.Width(200));
													
								
								bool val= EditorGUILayout.Toggle(getValueFromFieldInfo(field, obj.component) ,isFieldChecked(field,obj.component));	
								if (val !=isFieldChecked(field,obj.component))
									setFieldChecked(field,obj.component,val);
								GUILayout.FlexibleSpace();
							EditorGUILayout.EndHorizontal();
						}
						
					EditorGUILayout.EndVertical();
				
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Separator();
				}
			}
			EditorGUILayout.EndScrollView();
			showSelectUnselectAllButtons();
			
			if (GUILayout.Button("migrate!"))
				migrate();
			
		}
	}

	void showSelectUnselectAllButtons ()
	{
		EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("unselect all"))
				changeValueForAllObjectsTo(false);
			if (GUILayout.Button("select all"))
				changeValueForAllObjectsTo(true);			
		EditorGUILayout.EndHorizontal();
	}

	void changeValueForAllObjectsTo (bool val)
	{		
		Dictionary<int,bool> newDict=new Dictionary<int, bool>();
		foreach(int key in checkedObjectsHash.Keys){
			newDict.Add(key,val);	
		}		
		checkedObjectsHash=newDict;		
	}
	
	
	
	
	
	void setFieldChecked(FieldInfo field, Component component, bool val){
		int hash=getCombinedHash(field,component);
		checkedObjectsHash[hash]=val;
	}
	
	bool isFieldChecked (FieldInfo field, Component component)
	{
		int hash=getCombinedHash(field,component);
		if (checkedObjectsHash.ContainsKey (hash)){
			return	(bool)checkedObjectsHash[hash];
		} else {
			checkedObjectsHash.Add(hash,DEFAULT_CHECKED_OBJECT_VALUE);
			return DEFAULT_CHECKED_OBJECT_VALUE;
		}
	}
	
	private int getCombinedHash(FieldInfo field, Component component){
		return (field.GetHashCode() +"]["+component.GetHashCode()).GetHashCode();
	}
	
	private void printHeader(){
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("objectName",GUILayout.Width(250));
			EditorGUILayout.LabelField("propertyType",GUILayout.Width(200));
			EditorGUILayout.LabelField("propertyNameValue");
		EditorGUILayout.EndHorizontal();					
	}
	
	private string getValueFromFieldInfo(FieldInfo fi, Component go){
		string result="";
		if ( fi.FieldType == typeof(UFTAtlasMetadata)){
			result=((UFTAtlasMetadata)fi.GetValue(go)).atlasName;	
		} else if ( fi.FieldType == typeof(UFTAtlasEntryMetadata)){
			result=((UFTAtlasEntryMetadata)fi.GetValue(go)).name;
		} else {
			throw new System.Exception("unsuported FieldInfo type exception ="+fi.FieldType);	
		}
		
		return result;
	}
	

	void updateObjectList ()
	{
		if (objectList == null)
			objectList = new Dictionary<System.Type, List<UFTObjectOnScene>>();
		
		objectList = UFTAtlasUtil.getObjectOnSceneByAtlas(atlasMetadataFrom,typeof(UFTAtlasEntryMetadata));
		checkedObjectsHash = new Dictionary<int, bool>();
	}

	
	
	
	
	
	
	public void checkIsAllPropertiesSet ()
	{
		//isValid=((atlasMetadataFrom != null) && (atlasMetadataTo != null) && (atlasMetadataTo != atlasMetadataFrom));
	}
	
	
	void OnWizardUpdate(){
		//checkIsAllPropertiesSet ();	
		
				
	}
	
}
