using UnityEngine;
using System.Collections;
using UnityEditor;

public class UFTAtlasMigrateWizard : ScriptableWizard {
	public UFTAtlasMetadata atlasMetadataFrom;
	public UFTAtlasMetadata atlasMetadataTo;
	
    [MenuItem ("Window/UFT Atlas Migration")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<UFTAtlasMigrateWizard>("Migrate all objects to new atlas", "Migrate!");        
    }
	
	void OnWizardCreate(){
		
	}
	
	void OnWizardUpdate(){
		isValid=((atlasMetadataFrom != null) && (atlasMetadataTo != null) && (atlasMetadataTo != atlasMetadataFrom));	
		helpString = "atlas metadatas must be different and point to the objects." +
			"\nAll objects which use source metatadata will be updated" +
			"\nIf this objects will has entryMetadat this links will be changed according to names";
				
	}
	
}
