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
	private static string EDITORPREFS_ATLASMIGRATION_UPDATE_MATERIAL="uftAtlasEditor.updateMaterial";
	
	private Vector2 scrollPosition;
	private bool updateMaterial;
	Dictionary<int,bool> checkedObjectsHash;
	Dictionary<int,bool> checkedMaterialsHash;
	List<UFTMaterialWitTextureProps> materials;
	
	bool DEFAULT_CHECKED_OBJECT_VALUE=true;
	bool DEFAULT_CHECKED_MATERIAL_PROPERTY_VALUE=true;
	Dictionary<System.Type,List<UFTObjectOnScene>>  objectList;
	
	
	private static string HELP_STRING = "atlas metadatas must be different and point to the objects." +
			"\nAll objects which use source metatadata will be updated" +
			"\nIf this objects will has entryMetadata this links will be changed according to path";
	
    [MenuItem ("Window/UFT Atlas Migration")]
    static void CreateWindow () {
        UFTAtlasMigrateWindow window=(UFTAtlasMigrateWindow) EditorWindow.GetWindow(typeof(UFTAtlasMigrateWindow));        
		window.initialize();
			
	}
	
	void initialize(){
		string atlasFrom=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_FROM,null);
		if (atlasFrom != null){
			atlasMetadataFrom = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasFrom,typeof(UFTAtlasMetadata));
			if (atlasMetadataFrom !=null){
				updateObjectList();
				updateMaterialTextureList();		
			}
		}
		string atlasTo=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_TO,null);
		if (atlasFrom != null)
			atlasMetadataTo = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasTo,typeof(UFTAtlasMetadata));	
		updateMaterial = EditorPrefs.GetBool(EDITORPREFS_ATLASMIGRATION_UPDATE_MATERIAL,false);
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
			updateMaterialTextureList();
		}
		UFTAtlasMetadata newMetaTo=(UFTAtlasMetadata) EditorGUILayout.ObjectField("source atlas",atlasMetadataTo,typeof(UFTAtlasMetadata),false);
		if (newMetaTo != atlasMetadataTo ){
			atlasMetadataTo= newMetaTo;
			EditorPrefs.SetString(EDITORPREFS_ATLASMIGRATION_TO,AssetDatabase.GetAssetPath(atlasMetadataTo));
			if (isAllMetadataObjectsValid ()){
				updateObjectList();					
			}
		}
		
		if (isAllMetadataObjectsValid()){			
			displayUpdateLinksToTextureMaterial();
			displayObjectListIfExist();							
			if (GUILayout.Button("migrate!"))
				migrate();			
		}
		
	}

	bool isAllMetadataObjectsValid ()
	{
		return atlasMetadataFrom!=null && atlasMetadataTo !=null && atlasMetadataFrom != atlasMetadataTo;
	}
	
	

	void updateMaterialTextureList ()
	{
		materials=UFTMaterialUtil.getMaterialListByTexture(atlasMetadataFrom.texture);
		checkedMaterialsHash = new Dictionary<int, bool>();
	}
	
	void displayUpdateLinksToTextureMaterial ()
	{
		bool newUpdMaterialValue= EditorGUILayout.Toggle("update texture materials",updateMaterial);
		if (newUpdMaterialValue != updateMaterial){
			updateMaterial=newUpdMaterialValue;
			EditorPrefs.SetBool (EDITORPREFS_ATLASMIGRATION_UPDATE_MATERIAL,updateMaterial);			
		}
		if (updateMaterial && materials!=null ){			
			if (materials.Count == 0){
				EditorGUILayout.LabelField("non of material use " + atlasMetadataTo.name + " atlas texture");
			} else {
				EditorGUILayout.LabelField("Materials ["+materials.Count+"] :");
				
				foreach(UFTMaterialWitTextureProps mat in materials){
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(""+mat.material.name,GUILayout.Width(250),GUILayout.Width(250)))
						Selection.activeObject = mat.material;
					EditorGUILayout.Separator();
				
					foreach (string propName in mat.textureProperties) {
						bool newValue=EditorGUILayout.Toggle(""+propName, isPropertyMaterialChecked(mat, propName),GUILayout.Width(200));		
						if (newValue!=isPropertyMaterialChecked(mat,propName)){
							setPropertyMaterialChecked(mat,propName,newValue);
						}
					}
						
					GUILayout.FlexibleSpace();
					
					EditorGUILayout.EndVertical();
				}
				
			}
		}
	}
	void setPropertyMaterialChecked(UFTMaterialWitTextureProps mat, string propName, bool val){
		int hash=getCominedMaterialPropertyHash(mat,propName);
		checkedMaterialsHash[hash]=val;
	}
	
	
	bool isPropertyMaterialChecked (UFTMaterialWitTextureProps mat, string propName)
	{
		int hash=getCominedMaterialPropertyHash(mat,propName);
		if (checkedMaterialsHash.ContainsKey(hash)){
			return checkedMaterialsHash[hash];
		} else {
			checkedMaterialsHash.Add(hash,DEFAULT_CHECKED_MATERIAL_PROPERTY_VALUE);
			return DEFAULT_CHECKED_MATERIAL_PROPERTY_VALUE;
		}
	}
	
	private int getCominedMaterialPropertyHash(UFTMaterialWitTextureProps mat, string propName){
		return (mat.GetHashCode() +"]["+propName.GetHashCode()).GetHashCode();
		
	}	
	
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
								EditorGUILayout.LabelField(field.Name,GUILayout.Width(200));
													
								
								bool val= EditorGUILayout.Toggle(getNameFromFieldInfo(field, obj.component) ,isFieldChecked(field,obj.component),GUILayout.Width(200));	
								if (val !=isFieldChecked(field,obj.component))
									setFieldChecked(field,obj.component,val);
								EditorGUILayout.LabelField(field.FieldType.ToString());
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
			
		}
	}
	
	void migrate ()
	{
		Dictionary<string,UFTAtlasEntryMetadata> targetAtlasByMetaDictionary=getAtlasByMetaDictionary(atlasMetadataTo);
		foreach(KeyValuePair<System.Type,List<UFTObjectOnScene>> keyValue in objectList){
			foreach(UFTObjectOnScene obj in keyValue.Value){
				Component component=obj.component;
				foreach(FieldInfo fieldInfo in obj.propertyList){
					if (isFieldChecked(fieldInfo,component)){
						setNewFieldValue(fieldInfo,component,ref targetAtlasByMetaDictionary);	
					}
				}
				if (component is UFTOnAtlasMigrateInt)
					((UFTOnAtlasMigrateInt)component).onAtlasMigrate();				
			}
		}	
		
		if (updateMaterial)
			migrateMaterial();
	}

	void migrateMaterial()
	{
		throw new System.NotImplementedException ();
	}

	void setNewFieldValue (FieldInfo fieldInfo, Component component, ref Dictionary<string, UFTAtlasEntryMetadata> targetAtlasByMetaDictionary)
	{
		if ( fieldInfo.FieldType == typeof(UFTAtlasMetadata)){
			fieldInfo.SetValue(component,atlasMetadataTo);
			//result=((UFTAtlasMetadata)fieldInfo.GetValue(go)).atlasName;	
		} else if ( fieldInfo.FieldType == typeof(UFTAtlasEntryMetadata)){
			UFTAtlasEntryMetadata oldEntryMeta= (UFTAtlasEntryMetadata)fieldInfo.GetValue(component);			
			string path=oldEntryMeta.assetPath;
			fieldInfo.SetValue(component,targetAtlasByMetaDictionary[path]);
		} else {
			throw new System.Exception("unsuported FieldInfo type exception  fieldType="+fieldInfo.FieldType);	
		}		
	}
	
	

	private Dictionary<string, UFTAtlasEntryMetadata> getAtlasByMetaDictionary (UFTAtlasMetadata atlasMetadataTo)
	{
		Dictionary<string, UFTAtlasEntryMetadata> result= new Dictionary<string, UFTAtlasEntryMetadata>();
		foreach(UFTAtlasEntryMetadata entry in atlasMetadataTo.entries){
			result.Add(entry.assetPath,entry);	
		}
		return result;
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
		int hash=getCombinedFieldHash(field,component);
		checkedObjectsHash[hash]=val;
	}
	
	bool isFieldChecked (FieldInfo field, Component component)
	{
		int hash=getCombinedFieldHash(field,component);
		if (checkedObjectsHash.ContainsKey (hash)){
			return	(bool)checkedObjectsHash[hash];
		} else {
			checkedObjectsHash.Add(hash,DEFAULT_CHECKED_OBJECT_VALUE);
			return DEFAULT_CHECKED_OBJECT_VALUE;
		}
	}	
	
	private int getCombinedFieldHash(FieldInfo field, Component component){
		return (field.GetHashCode() +"]["+component.GetHashCode()).GetHashCode();
	}
	
	private void printHeader(){
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Object Name",GUILayout.Width(250));
			EditorGUILayout.LabelField("Property Name",GUILayout.Width(200));
			EditorGUILayout.LabelField("Property Value",GUILayout.Width(200));
			EditorGUILayout.LabelField("Property Type");
			GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();					
	}
	
	private string getNameFromFieldInfo(FieldInfo fi, Component go){
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

	
}
