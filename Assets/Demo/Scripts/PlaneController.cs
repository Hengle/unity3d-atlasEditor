using UnityEngine;
using System.Collections;


public enum SCREEN_SIDE_ENUM{
	left,
	right,
	top,
	down
}

public class PlaneController : MonoBehaviour {
	public static int zMin=400;
	public static int zMax=1000;
	public Vector3 direction;
	public float speed=50;
	public float speedMin=10;
	public float speedMax=30;
	
	private static int OFFSET_X=128; //half of input texture width
	private static int OFFSET_Y=64; //half of input texture height
	// Use this for initialization
	void Start () {
		updatePositionAndDirection();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(direction*Time.deltaTime*speed,Space.World);
	}
	
	public void updatePositionAndDirection(){
		
		if (Camera.main==null)
			return;
		
		int z=Random.Range(zMin,zMax);
		SCREEN_SIDE_ENUM side=(SCREEN_SIDE_ENUM) Random.Range(0,4);
		int x=0;
		int y=0;
		int offsetX=0;
		int offsetY=0;
		switch (side){
		case SCREEN_SIDE_ENUM.left:
			y=Random.Range(OFFSET_Y,Screen.height-OFFSET_Y);				
			offsetX=-OFFSET_X;
			direction=Vector3.right;
			break;
		
		case SCREEN_SIDE_ENUM.right:
			x=Screen.width;
			offsetX=OFFSET_X;
			y=Random.Range(OFFSET_Y,Screen.height-OFFSET_Y);
			direction=Vector3.left;
			break;
		
		case SCREEN_SIDE_ENUM.top:
			y=Screen.height;
			x=Random.Range(OFFSET_X,Screen.width-OFFSET_X);				
			offsetY=OFFSET_Y;
			direction=Vector3.down;
			break;
		case SCREEN_SIDE_ENUM.down:
			y=0;
			offsetY=-OFFSET_Y;
			x=Random.Range(OFFSET_X,Screen.width+OFFSET_X);
			direction=Vector3.up;
			break;
		}
		speed=z/Random.Range(speedMin,speedMax);
		Vector3 position= Camera.main.ScreenToWorldPoint(new Vector3(x,y,z));
	
		position.x+=offsetX;
		position.y+=offsetY;
		transform.position=position;
		
	}
	
	
	void OnBecameInvisible(){
		updatePositionAndDirection();	
	}
}
