using UnityEngine;
using System.Collections;

public class MovedGUITexture : MonoBehaviour {
	public float speed;
	public UFTAtlasEntryMetadata entryMetadata;
	public UFTAtlasMetadata atlasMetadata;	
	
	int directionX;
	int directionY;
	
	int width;
	int height;
	int x;
	int y;
	
	int rigthLimit;
	int leftLimit;
	int topLimit;
	int downLimit;
	
	
	float startTime;
	Vector2 widthHeight;
	Vector2 currentPoint;	
	void OnGUI () {
		int cx;
		int cy;
		if (directionX !=0){
			cx = (int)( x + directionX * speed * (Time.time - startTime));
			cy = y;
		} else {
			cy = (int)( y + directionY * speed * (Time.time - startTime));
			cx = x;
		}
		GUI.DrawTextureWithTexCoords(new Rect(cx,cy,width,height),atlasMetadata.texture,entryMetadata.uvRect);
		if (cx>rigthLimit || cx < leftLimit || cy < topLimit || cy > downLimit)
			reset ();
	}
	
		
	
	public void reset(){		
		resetDirection ();
		resetSpeed();
		float ratio = Random.Range(0.5f,1f);
		width =(int)( entryMetadata.pixelRect.width * ratio);
		height =(int)( entryMetadata.pixelRect.height * ratio);
		if (directionX != 0){
			x = directionX * Screen.width * -1;
			x = x < 0 ? 0 : x;
			y = Random.Range(0,Screen.height);
		} else {
			y = directionY * Screen.height * -1;
			y = y < 0 ? 0 : y;
			x = Random.Range(0, Screen.width);
		}
		leftLimit = -width;
		rigthLimit = Screen.width + width;
		topLimit = -height;
		downLimit = Screen.height + height;
		startTime = Time.time;
	}
	
	void resetSpeed(){
		speed = Random.Range(20,60);
	}
	
	public void resetDirection ()
	{
		switch (Random.Range(0,4)){
		case 0:
			directionX =-1;
			directionY = 0;			
			break;
		case 1:
			directionX = 1;
			directionY = 0;
			break;
		case 2:
			directionX = 0;
			directionY = 1;
			break;
		case 3:
			directionX = 0;
			directionY =-1;
			break;			
		}
	}
}
