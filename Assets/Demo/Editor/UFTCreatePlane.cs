using UnityEngine;
using System.Collections;
using UnityEditor;

public class UFTCreatePlane : ScriptableWizard {
	public int width=256;
	public int height=128;
	
	[MenuItem ("GameObject/Create Plane")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<UFTCreatePlane>("Create Plane", "Create");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }
	
	 void OnWizardCreate () {
        GameObject go = MeshUtil.createPlane(width,height);
		go.AddComponent<UFTSelectTextureFromAtlas>();
		
		AssetDatabase.CreateAsset(go.GetComponent<MeshFilter>().sharedMesh, "Assets/plane.asset" );
        AssetDatabase.SaveAssets();
		
    } 
}
