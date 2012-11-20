using UnityEngine;
using System.Collections;



public class PlaneCreateController : MonoBehaviour {
	public UFTAtlasMetadata metadata;
	public float newObjectTimeout=1f;
	public Material material;

	IEnumerator  GeneratePlanes ()
	{
		int i=0;
		foreach(UFTAtlasEntryMetadata entryMeta in metadata.entries){
			//GameObject gameObject=new GameObject();
			GameObject gameObject=MeshUtil.createPlane(entryMeta.pixelRect.width,entryMeta.pixelRect.height);			
			
			gameObject.renderer.material=material;
			gameObject.AddComponent<PlaneController>();
			UFTSelectTextureFromAtlas textAtlasScript= gameObject.AddComponent<UFTSelectTextureFromAtlas>();
			textAtlasScript.atlasMetadata=metadata;
			textAtlasScript.textureIndex=i++;
			textAtlasScript.updateUV();
			yield return new WaitForSeconds(newObjectTimeout);
		}
	}
		
	void Start () {
		StartCoroutine( GeneratePlanes ());
	}
}
