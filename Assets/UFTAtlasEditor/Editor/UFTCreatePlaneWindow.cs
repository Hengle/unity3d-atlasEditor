using UnityEngine;
using System.Collections;
using UnityEditor;

public class UFTCreatePlaneWindow : ScriptableWizard {
	public string meshName="plane";
	public int width=256;
	public int height=128;
	public Material material;
	public UFTAtlasMetadata atlasMetadata;
	
	
	[MenuItem ("Window/UFT SimplePlane")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<UFTCreatePlaneWindow>("Window/UFT SimplePlane", "Create");        
    }
	
	 void OnWizardCreate () {
        GameObject go = UFTMeshUtil.createPlane(width,height);
		go.AddComponent<UFTSelectTextureFromAtlas>().atlasMetadata=atlasMetadata;
		go.renderer.material=material;
		
		AssetDatabase.CreateAsset(go.GetComponent<MeshFilter>().sharedMesh, AssetDatabase.GenerateUniqueAssetPath("Assets/"+meshName+".asset") );
        AssetDatabase.SaveAssets();
		
    } 
}
