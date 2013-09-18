using UnityEngine;
using System.Collections;

public class OnGuiPlaneController : MonoBehaviour {
	public UFTAtlasMetadata metadata;
	public float newObjectTimeout=0.5f;
	
	IEnumerator  GeneratePlanes ()
	{
		int i=0;
		foreach(UFTAtlasEntryMetadata entryMeta in metadata.entries){
			//GameObject gameObject=new GameObject();
			GameObject gameObject=new GameObject();
			MovedGUITexture mt = gameObject.AddComponent<MovedGUITexture>();
			mt.atlasMetadata = metadata;
			mt.entryMetadata = entryMeta;
			mt.reset();			
			yield return new WaitForSeconds(newObjectTimeout);
		}
	}
		
	void Start () {
		StartCoroutine( GeneratePlanes ());
	}
}
