using UnityEngine;
using System.Collections;
using UnityEditor;


public enum ATLAS_SIZE{
		_128=128,
		_256=256,
		_512=512,
		_1024=1024,
		_2056=2056,
		_4112=4112		
	}

public class FreeAtlasEditor : EditorWindow {
	[SerializeField]
	ATLAS_SIZE atlasWidth=ATLAS_SIZE._1024;
	
	[SerializeField]
	ATLAS_SIZE atlasHeight=ATLAS_SIZE._1024;
	
	[SerializeField]
	Texture2D atlasCanvasBG;
	static Color bgColor1=Color.white;
	static Color bgColor2=new Color(0.8f,0.8f,0.8f,1f);
	int bgCubeSize=8;
	
	
	
	public Vector2 scrollPosition = Vector2.zero;
	GUIContent atlasContent=new GUIContent("atlas");
	GUIStyle atlasStyle=GUIStyle.none;
	
	
	[MenuItem ("Window/Free Atlas Maker")]
    static void ShowWindow () {        
		EditorWindow.GetWindow <FreeAtlasEditor>();				
    }
	
	void OnEnable() {
		initParams ();	
	}

	void initParams ()
	{
		Debug.Log("im in init");		
		recreateAtlasBG ();
	}
	
	
	
	
	
	
	
	void OnGUI () {
		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal ();
				ATLAS_SIZE newWidth =(ATLAS_SIZE) EditorGUILayout.EnumPopup("atlas width",atlasWidth);
				ATLAS_SIZE newHeight =(ATLAS_SIZE) EditorGUILayout.EnumPopup("atlas height",atlasHeight);	
				
				if (newWidth!=atlasWidth || newHeight!=atlasHeight){
					atlasWidth=newWidth;
					atlasHeight=newHeight;
					recreateAtlasBG();		
				}
			EditorGUILayout.EndHorizontal();
		
			int width=(int)atlasWidth;
			int height=(int)atlasHeight;
		
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
				
				
				GUILayoutUtility.GetRect(width,height);
				//GUI.DrawTexture(new Rect(0,0,width,height),atlasBG);	
				EditorGUI.DrawPreviewTexture(new Rect(0,0,width,height),atlasCanvasBG);	
				//Rect r = GUILayoutUtility.GetAspectRect(1.0f,GUILayout.Width(width),GUILayout.Height(height));											
				//EditorGUI.DrawPreviewTexture(r,atlasBG,null,ScaleMode.StretchToFill);		
			EditorGUILayout.EndScrollView();
		
		EditorGUILayout.EndVertical();
		
		
		 if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform){
	        DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // show a drag-add icon on the mouse cursor
			if(Event.current.type==EventType.dragPerform)
				HandleDroppedObjects(DragAndDrop.objectReferences);
            
			DragAndDrop.AcceptDrag();
	        Event.current.Use();
	    }
		
	}

	void recreateAtlasBG ()
	{		
		atlasCanvasBG=createAtlasCanvasBGTexture((int)atlasWidth,(int)atlasHeight);		
	}

	void HandleDroppedObjects (Object[] objectReferences)
	{
		Debug.Log("income objects ="+objectReferences.Length);
	}
	
	
	
	
	
	
	
	
	//here we just generate Texture2d
	 Texture2D createAtlasCanvasBGTexture(int width, int height){
		Debug.Log("im here in atlas creation");
		Texture2D texture=new Texture2D(width,height);
		Color[] pixels=new Color[width*height];
		Color rowFirstColor=bgColor1;
		
		Color currentColor=bgColor1;
		for (int textHeight = 0; textHeight < height; textHeight+=bgCubeSize) {
			rowFirstColor=(rowFirstColor==bgColor1)?bgColor2:bgColor1;
			currentColor=rowFirstColor;
			for (int textWidth = 0; textWidth < width; textWidth+=bgCubeSize) {			
				int initPosition=textWidth+(textHeight*width);
				
				// paint pixels on this and nex bgCubeSize row
				for (int cubeWidth = 0; cubeWidth < bgCubeSize; cubeWidth++) {
					for (int cubeHeight = 0; cubeHeight < bgCubeSize; cubeHeight++) {
						//pixels[initPosition+k]=currentColor;	
						pixels[initPosition+cubeWidth+(width*cubeHeight)]=currentColor;	
					}					
				}
				currentColor=(currentColor==bgColor1)?bgColor2:bgColor1;
			}
		}
		texture.SetPixels(pixels);	
		texture.Apply();
		TextureUtil.saveTextureToFile(texture,"testTextrettt");
		return texture;
	}
	
}
