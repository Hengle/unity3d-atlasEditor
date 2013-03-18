using UnityEngine;
using System.Collections;
using UnityEditor;

public class UFTAtlasMigrateWizard : ScriptableWizard {
	public UFTAtlasMetadata atlasMetadataFrom;
	public UFTAtlasMetadata atlasMetadataTo;
	
	private static string EDITORPREFS_ATLASMIGRATION_FROM="uftAtlasEditor.atlasFrom";
	private static string EDITORPREFS_ATLASMIGRATION_TO="uftAtlasEditor.atlasTo";
	
	
    [MenuItem ("Window/UFT Atlas Migration")]
    static void CreateWizard () {
        UFTAtlasMigrateWizard wizard= ScriptableWizard.DisplayWizard<UFTAtlasMigrateWizard>("Migrate all objects to new atlas", "Migrate!");        
		
		string atlasFrom=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_FROM,null);
		if (atlasFrom != null)
			wizard.atlasMetadataFrom = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasFrom,typeof(UFTAtlasMetadata));
		
		string atlasTo=EditorPrefs.GetString(EDITORPREFS_ATLASMIGRATION_TO,null);
		if (atlasFrom != null)
			wizard.atlasMetadataTo = (UFTAtlasMetadata) AssetDatabase.LoadAssetAtPath(atlasTo,typeof(UFTAtlasMetadata));
    }
	
	void OnWizardCreate(){
		UFTAtlasUtil.migrateAtlasMatchEntriesByName(atlasMetadataFrom,atlasMetadataTo);		
		EditorPrefs.SetString(EDITORPREFS_ATLASMIGRATION_FROM,AssetDatabase.GetAssetPath(atlasMetadataFrom));
		EditorPrefs.SetString(EDITORPREFS_ATLASMIGRATION_TO,AssetDatabase.GetAssetPath(atlasMetadataTo));
		Debug.Log("done");
	}
	
	void OnWizardUpdate(){
		isValid=((atlasMetadataFrom != null) && (atlasMetadataTo != null) && (atlasMetadataTo != atlasMetadataFrom));	
		helpString = "atlas metadatas must be different and point to the objects." +
			"\nAll objects which use source metatadata will be updated" +
			"\nIf this objects will has entryMetadat this links will be changed according to names";
				
	}
	
}
