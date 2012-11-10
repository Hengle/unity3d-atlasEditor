using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/*
 * Thanks AngryAnt for a greate drag'n'drop example at this page http://angryant.com/2009/09/18/gui-drag-drop/
 * 
 */



public class UFTAtlasEditorConfig{
		
	private static GUIStyle _borderStyle;  
	
	public static GUIStyle borderStyle {
		get {
			if (_borderStyle == null){
				_borderStyle=new GUIStyle();
				_borderStyle.normal.background=FreeAtlasEditor.borderTexture;
				_borderStyle.border=new RectOffset(1,1,1,1);
				_borderStyle.alignment=TextAnchor.MiddleCenter;
			}			
			return _borderStyle;
		}
		set {
			_borderStyle = value;
		}
	}
}




public enum ATLAS_SIZE{
		_128=128,
		_256=256,
		_512=512,
		_1024=1024,
		_2056=2056,
		_4112=4112		
	}


public class TextureOnCanvas{	
	public Rect canvasRect;
	public Texture2D texture;
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	
	
	public TextureOnCanvas (Rect canvasRect, Texture2D texture)
	{
		this.canvasRect = canvasRect;
		this.texture = texture;
	}
	
	public void draw(){
		
		
		if (Event.current.type == EventType.MouseUp){
			isDragging = false;
			if (FreeAtlasEditor.stopDragging!=null)
				FreeAtlasEditor.stopDragging();
		} else if (Event.current.type == EventType.MouseDown && canvasRect.Contains (Event.current.mousePosition)){
			isDragging = true;						
			mouseStartPosition=Event.current.mousePosition;							
			Event.current.Use();	
			
		}
		
		if (isDragging){ 
			
			Vector2 currentOffset=Event.current.mousePosition-mouseStartPosition;
			
			if (Event.current.type == EventType.Repaint){
				canvasRect.x+=currentOffset.x;
				canvasRect.y+=currentOffset.y;
				
				if (canvasRect.x < 0){
					canvasRect.x=0;					
				} 
				
				if (canvasRect.y <0){
					canvasRect.y=0;					
				}
				
				if (canvasRect.xMax > (int)FreeAtlasEditor.atlasWidth){
					canvasRect.x=	(int)FreeAtlasEditor.atlasWidth-texture.width;
				}
				
				if (canvasRect.yMax > (int)FreeAtlasEditor.atlasHeight){
					canvasRect.y=	(int)FreeAtlasEditor.atlasHeight-texture.height;
				}
				
				
				mouseStartPosition=Event.current.mousePosition;
				
			}
			if (FreeAtlasEditor.dragInProgress!=null)
				FreeAtlasEditor.dragInProgress();
		}
		
		EditorGUI.DrawPreviewTexture(canvasRect,texture);	
		
		
		
		GUI.Box(canvasRect,GUIContent.none,UFTAtlasEditorConfig.borderStyle);
		
	}

}


public delegate void DragInProgress();
public delegate void StopDragging();

public class FreeAtlasEditor : EditorWindow {
	[SerializeField]
	public static ATLAS_SIZE atlasWidth=ATLAS_SIZE._512;
	
	[SerializeField]
	public static ATLAS_SIZE atlasHeight=ATLAS_SIZE._512;
	
	[SerializeField]
	Texture2D atlasCanvasBG;
	
	public static Texture2D borderTexture;
	static Color bgColor1=Color.white;
	static Color bgColor2=new Color(0.8f,0.8f,0.8f,1f);
	static int bgCubeSize=8;
	
	
	private List<TextureOnCanvas> texturesOnCanvas;
	
	public Vector2 scrollPosition = Vector2.zero;
	
	public static DragInProgress dragInProgress;
	public static StopDragging stopDragging;
	
	[MenuItem ("Window/Free Atlas Maker")]
    static void ShowWindow () {    
		Init();
		EditorWindow.GetWindow <FreeAtlasEditor>();				
    }
	
	void OnEnable() {
		initParams ();	
	}
	
	static void Init(){
		createAtlasCanvasBGTexture((int)atlasWidth,(int)atlasHeight);
		borderTexture=createOnePxBorderTexture();
	}
	
	
	void initParams ()
	{	
		
		recreateAtlasBG ();
		texturesOnCanvas=new List<TextureOnCanvas>();
		dragInProgress+=onDragInProgress;
		stopDragging+=onStopDragging;
		
	}
	
	
	private void onDragInProgress(){
		Repaint();	
	}
	
	private void onStopDragging(){
		Repaint();	
	}
	void Update(){
		Repaint();	
	}
	
	
	void OnGUI () {
		
		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal (GUILayout.MinHeight(100f));
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
				
				if (atlasCanvasBG!=null)
					EditorGUI.DrawPreviewTexture(new Rect(0,0,width,height),atlasCanvasBG);	
				
				if(texturesOnCanvas!=null){
					foreach(TextureOnCanvas toc in  texturesOnCanvas){
						
						toc.draw();
						
					}	
			
				/*	
				foreach(TextureOnCanvas toc in  texturesOnCanvas){
						Color color=GUI.color;
						GUI.color=Color.yellow;
						Graphics.DrawTexture(toc.canvasRect, borderTexture,5,5,5,5);
						GUI.color=color;
					}	
				*/
			
			
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
	
	
	
	
	
	static Texture2D createOnePxBorderTexture(){
		string assetPath="Assets/Assets/Editor/Texture/onePxBorder.png";
		
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Mesh));
		if (texture==null){
			texture=new Texture2D(3,3);
			Color[] c=new Color[9];
			for (int i=0;i<9;i++){
				c[i]=new Color(1,1,1,1);	
			}
			c[4]=new Color(1,1,1,0); //center alpha is empty
			texture.SetPixels(c);
			texture.Apply();
			
			
			
			
			//save to files an then import
			 byte[] bytes = texture.EncodeToPNG();
		    if (bytes != null)
		      File.WriteAllBytes(assetPath, bytes);
		    Object.DestroyImmediate((Object) texture);
		    AssetDatabase.ImportAsset(assetPath);
		    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		    textureImporter.textureFormat=TextureImporterFormat.ARGB32;//   set_textureFormat((TextureImporterFormat) -3);
		    textureImporter.textureType=TextureImporterType.Advanced;// set_textureType((TextureImporterType) 2);
			textureImporter.mipmapEnabled=false;
			textureImporter.wrapMode=TextureWrapMode.Clamp;
			textureImporter.filterMode=FilterMode.Point;
			textureImporter.npotScale=TextureImporterNPOTScale.None;
		    AssetDatabase.ImportAsset(assetPath);
		    texture= (Texture2D) AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D));			
			
			
			
			
			
		}
		return texture;
	}
	
	
	
	
	//here we just generate Texture2d
	 static Texture2D createAtlasCanvasBGTexture(int width, int height){
		string assetPath="Assets/Assets/Editor/Texture/AtlasCanvasBG.asset";
		
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Mesh));
		if (texture==null){
			texture=new Texture2D(width,height,TextureFormat.ARGB32,false);	
			AssetDatabase.CreateAsset(texture,assetPath);
			AssetDatabase.SaveAssets();
		}
		
		
		texture=new Texture2D(width,height);
		
			
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
		//TextureUtil.saveTextureToFile(texture,"testTextrettt");
		
		
		return texture;
	}
	
}
