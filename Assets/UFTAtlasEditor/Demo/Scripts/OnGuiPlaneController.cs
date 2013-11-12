using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnGuiPlaneController : MonoBehaviour {
	public UFTAtlasMetadata metadata;
	public float newObjectTimeout=0.5f;
	public bool pause=false;
	
	void Awake(){
		useGUILayout = false;
		movedList=new List<MovedGUITexture>();
	}
	
	List<MovedGUITexture> movedList;
	IEnumerator  GeneratePlanes ()
	{
		int i=0;
		foreach(UFTAtlasEntryMetadata entryMeta in metadata.entries){
			//GameObject gameObject=new GameObject();			
			MovedGUITexture mt = new MovedGUITexture();
			mt.atlasMetadata = metadata;
			mt.entryMetadata = entryMeta;
			mt.ogpc = this;
			mt.reset();			
			movedList.Add(mt);
			yield return new WaitForSeconds(newObjectTimeout);
		}
	}
		
	void Start () {
		StartCoroutine( GeneratePlanes ());
	}
	
	void OnGUI(){
		for (int i = 0; i < movedList.Count; i++) {
			movedList[i].OnGUI();
		}
	}
}
