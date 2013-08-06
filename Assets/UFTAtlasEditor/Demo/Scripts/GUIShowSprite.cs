using UnityEngine;
using System.Collections;

public class GUIShowSprite : MonoBehaviour {
	public UFTAtlasMetadata atlasMeta;
	
	public float timeoutInSec = 1.0f;
	public int offset = 50;
	UFTAtlasEntryMetadata spriteMeta1;
	UFTAtlasEntryMetadata spriteMeta2;
	Rect positionRect1;
	Rect positionRect2;
	float counter;
	
	void Start(){
		initNewSprite();
	}
	
	void FixedUpdate(){
		counter+=Time.deltaTime;
		if (counter>timeoutInSec)
			initNewSprite();
	}

	void initNewSprite()
	{
		counter = 0;		
		initRandomSpriteAndPosition(ref spriteMeta1,ref positionRect1);
		initRandomSpriteAndPosition(ref spriteMeta2,ref positionRect2, false);
	}
	
	void initRandomSpriteAndPosition(ref UFTAtlasEntryMetadata spriteMeta, ref Rect positionRect, bool alignToplef=true){
		int id=Random.Range(0,atlasMeta.entries.Length-1);
		spriteMeta = atlasMeta.entries[id];
		if (alignToplef){
			positionRect = new Rect(offset,
									offset,
									spriteMeta.pixelRect.width,
									spriteMeta.pixelRect.height);
		} else {
			positionRect = new Rect(Screen.width  - spriteMeta.pixelRect.width - offset,
									Screen.height - spriteMeta.pixelRect.height - offset,
									spriteMeta.pixelRect.width,
									spriteMeta.pixelRect.height);
		}
	}
	
	
	// Update is called once per frame
	void OnGUI () {
		GUI.DrawTextureWithTexCoords(positionRect1,atlasMeta.texture,spriteMeta1.uvRect);
		GUI.DrawTextureWithTexCoords(positionRect2,atlasMeta.texture,spriteMeta2.uvRect);
		
	}
}
