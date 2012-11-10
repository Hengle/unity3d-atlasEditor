using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/*
 * Thanks AngryAnt for a greate drag'n'drop example at this page http://angryant.com/2009/09/18/gui-drag-drop/
 * 
 */



public enum AtlasSize{
		_128=128,
		_256=256,
		_512=512,
		_1024=1024,
		_2056=2056,
		_4112=4112		
	}

public enum TextureState{
		passive,
		onDrag,
		showBorder,
		invalidPosition
		
}



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
	
	
	private static Texture2D _atlasCanvasBGTile;

	public static Texture2D atlasCanvasBGTile {
		get {
			if (_atlasCanvasBGTile==null)
				_atlasCanvasBGTile=FreeAtlasEditor.getAtlasCanvasBGTile();
			return _atlasCanvasBGTile;
		}
		set {
			_atlasCanvasBGTile = value;
		}
	}	
	
	
	public static Dictionary<TextureState,Color> borderColorDict=new Dictionary<TextureState, Color>(){
		{TextureState.passive, GUI.color},
		{TextureState.onDrag, Color.green},
		{TextureState.showBorder, Color.yellow},
		{TextureState.invalidPosition, Color.red},
	};
}










public class TextureOnCanvas{	
	public Rect canvasRect;
	public Texture2D texture;
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	public TextureState textureState=TextureState.passive;
	
	
	
	public TextureOnCanvas (Rect canvasRect, Texture2D texture)
	{
		this.canvasRect = canvasRect;
		this.texture = texture;
	}
	
	public void draw(){
		
		if (Event.current.type == EventType.MouseUp){
			textureState=TextureState.passive;
			isDragging = false;
			textureState=TextureState.passive;
			if (FreeAtlasEditor.stopDragging!=null)
				FreeAtlasEditor.stopDragging();
		} else if (Event.current.type == EventType.MouseDown && canvasRect.Contains (Event.current.mousePosition)){
			textureState=TextureState.onDrag;
			isDragging = true;						
			textureState=TextureState.onDrag;
			mouseStartPosition=Event.current.mousePosition;							
			Event.current.Use();				
		}
		Color color=GUI.color;
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
			
			//if dragging lets color it to drag color border
			
			GUI.color=UFTAtlasEditorConfig.borderColorDict[TextureState.onDrag];
			
			
		}
		
		EditorGUI.DrawPreviewTexture(canvasRect,texture);			
		


		if (isDragging)
			GUI.color=color;

	}

}


public delegate void DragInProgress();
public delegate void StopDragging();

public class FreeAtlasEditor : EditorWindow {
	[SerializeField]
	public static AtlasSize atlasWidth=AtlasSize._512;
	
	[SerializeField]
	public static AtlasSize atlasHeight=AtlasSize._512;
	
	[SerializeField]
	Texture2D atlasCanvasBG;
	
	public static Texture2D borderTexture;
	static Color bgColor1=Color.white;
	static Color bgColor2=new Color(0.8f,0.8f,0.8f,1f);
	
	
	private static int atlasTileCubeFactor=16; // it equals of the cube size on bg
	private Rect atlasBGTexCoord=new Rect(0,0,1,1);
	
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
//		createAtlasCanvasBGTexture((int)atlasWidth,(int)atlasHeight);
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
				AtlasSize newWidth =(AtlasSize) EditorGUILayout.EnumPopup("atlas width",atlasWidth);
				AtlasSize newHeight =(AtlasSize) EditorGUILayout.EnumPopup("atlas height",atlasHeight);	
				
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
			
			Rect canvasRect = new Rect (0, 0, width, height);
			//if (atlasCanvasBG!=null)
			//	EditorGUI.DrawPreviewTexture(canvasRect,atlasCanvasBG);	
			GUI.DrawTextureWithTexCoords(canvasRect,UFTAtlasEditorConfig.atlasCanvasBGTile,atlasBGTexCoord,false);
		
		
			if(texturesOnCanvas!=null){
				foreach(TextureOnCanvas toc in  texturesOnCanvas){					
					toc.draw();
					
				}	
		
			
			
				// draw ellow border if mouse under the canvasw
				
				if (canvasRect.Contains (Event.current.mousePosition)){
					Color color=GUI.color;
					GUI.color=UFTAtlasEditorConfig.borderColorDict[TextureState.showBorder];
						
					foreach(TextureOnCanvas toc in  texturesOnCanvas){						
						GUI.Box(toc.canvasRect,GUIContent.none,UFTAtlasEditorConfig.borderStyle);
					}	
					GUI.color=color;
				}
			
			}
		
		/*
				//draw textures borders (in front of all textures, to prevent overlap)
				// draw "wrong texture" border everytime if exists, 
				// otherwise only if mouse in focus on atlas
				bool mouseInAtlasCanvas=canvasRect.Contains (Event.current.mousePosition);
				foreach(TextureOnCanvas toc in  texturesOnCanvas){
					if (toc.textureState == TextureState.invalidPosition || mouseInAtlasCanvas){
						EditorGUI.RectField
					}
				}
		*/
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
		atlasBGTexCoord=new Rect(0,0,(int)atlasWidth/atlasTileCubeFactor,(int)atlasHeight/atlasTileCubeFactor);		
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
	
	
	
	public static Texture2D getAtlasCanvasBGTile(){
		int squareWidth=1;
		int textureWidth=2;
		
		string assetPath="Assets/Assets/Editor/Texture/AtlasCanvasBGTile.png";
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Mesh));
		if (texture==null){
			texture=createAtlasCanvasBGTexture(textureWidth,textureWidth,squareWidth);
			byte[] bytes = texture.EncodeToPNG();
		    if (bytes != null)
		      File.WriteAllBytes(assetPath, bytes);
			 AssetDatabase.ImportAsset(assetPath);
		    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		     textureImporter.textureFormat=TextureImporterFormat.ARGB32;//   set_textureFormat((TextureImporterFormat) -3);
		    textureImporter.textureType=TextureImporterType.Advanced;// set_textureType((TextureImporterType) 2);
			textureImporter.mipmapEnabled=false;
			textureImporter.wrapMode=TextureWrapMode.Repeat;
			textureImporter.filterMode=FilterMode.Point;
			textureImporter.npotScale=TextureImporterNPOTScale.None;
		    AssetDatabase.ImportAsset(assetPath);
		    texture= (Texture2D) AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D));
		}
		return texture;
	}
	
	
	
	//here we just generate Texture2d
	 static Texture2D createAtlasCanvasBGTexture(int width, int height, int squareWidth){
	
		
		Texture2D texture=new Texture2D(width,height);
		
			
		Color[] pixels=new Color[width*height];
		Color rowFirstColor=bgColor1;
		
		Color currentColor=bgColor1;
		for (int textHeight = 0; textHeight < height; textHeight+=squareWidth) {
			rowFirstColor=(rowFirstColor==bgColor1)?bgColor2:bgColor1;
			currentColor=rowFirstColor;
			for (int textWidth = 0; textWidth < width; textWidth+=squareWidth) {			
				int initPosition=textWidth+(textHeight*width);
				
				// paint pixels on this and nex squareWidth row
				for (int cubeWidth = 0; cubeWidth < squareWidth; cubeWidth++) {
					for (int cubeHeight = 0; cubeHeight < squareWidth; cubeHeight++) {
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
