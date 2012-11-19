using UnityEngine;
using System.Collections;
using UnityEditor;

public class UFTCreatePlane : ScriptableWizard {
	public string meshName="plane";
	public int width=256;
	public int height=128;
	public Material material;
	public UFTAtlasMetadata atlasMetadata;
	
	
	[MenuItem ("GameObject/Create Plane")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<UFTCreatePlane>("UFT Create Plane", "Create");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }
	
	 void OnWizardCreate () {
        GameObject go = MeshUtil.createPlane(width,height);
		go.AddComponent<UFTSelectTextureFromAtlas>().atlasMetadata=atlasMetadata;
		go.renderer.material=material;
		
		AssetDatabase.CreateAsset(go.GetComponent<MeshFilter>().sharedMesh, AssetDatabase.GenerateUniqueAssetPath("Assets/"+meshName+".asset") );
        AssetDatabase.SaveAssets();
		
    } 
}
