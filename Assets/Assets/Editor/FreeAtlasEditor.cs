using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/*
 * Thanks AngryAnt for a greate drag'n'drop example at htis page http://angryant.com/2009/09/18/gui-drag-drop/
 * 
 */


public enum ATLAS_SIZE{
		_128=128,
		_256=256,
		_512=512,
		_1024=1024,
		_2056=2056,
		_4112=4112		
	}


public class TextureOnCanvas{
	public Rect rect;
	public Texture2D texture;
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	
	
	public TextureOnCanvas (Rect rect, Texture2D texture)
	{
		this.rect = rect;
		this.texture = texture;
	}
	
	public void drag(){
		if (Event.current.type == EventType.MouseUp){
			isDragging = false;
			
		} else if (Event.current.type == EventType.MouseDown && rect.Contains (Event.current.mousePosition)){
			isDragging = true;			
			
			mouseStartPosition=Event.current.mousePosition;
			
			Event.current.Use();	
			
		}
		
		if (isDragging){
			
			Vector2 currentOffset=Event.current.mousePosition-mouseStartPosition;
			
			rect.x+=currentOffset.x;
			rect.y+=currentOffset.y;
			
			mouseStartPosition=Event.current.mousePosition;
			
		}
		
	}

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
	
	
	private List<TextureOnCanvas> texturesOnCanvas;
	
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
		recreateAtlasBG ();
		texturesOnCanvas=new List<TextureOnCanvas>();
	}
	
	
	
	
	
	
	
	void OnGUI () {
		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal (GUILayout.MinHeight(150f));
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
				
				EditorGUI.DrawPreviewTexture(new Rect(0,0,width,height),atlasCanvasBG);	
				
				foreach(TextureOnCanvas toc in  texturesOnCanvas){
					
					toc.drag();
					EditorGUI.DrawPreviewTexture(toc.rect,toc.texture);	
					Repaint();
				}	
				
		
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
		bool addedSomething=false;
		foreach (Object item in objectReferences) {
			if(typeof(Texture2D)==item.GetType()){
				Texture2D texture=(Texture2D)item;
				addedSomething=true;
				
				
				
				if (isTextureCanvasContainsTexture (texture)){
					Debug.Log("one of texture is already on Canvas");
				} else {
					addTextureToCanvas(texture);	
				}
				
			}
		}
		
		if (!addedSomething)
			Debug.Log("there was no any Texture2D in dropped content");
	}

	void addTextureToCanvas (Texture2D texture)
	{
		Rect rect=new Rect(0,0,texture.width,texture.height);
		
		texturesOnCanvas.Add(new TextureOnCanvas(rect, texture));
		//texturesOnCanvas.Add(texture);
	}
	
	
	
	private bool isTextureCanvasContainsTexture (Texture2D texture)
	{
		return texturesOnCanvas.Find(toc => toc.texture == texture)!=null;
	}
	
	
	
	
	
	
	
	
	//here we just generate Texture2d
	 Texture2D createAtlasCanvasBGTexture(int width, int height){
		
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
