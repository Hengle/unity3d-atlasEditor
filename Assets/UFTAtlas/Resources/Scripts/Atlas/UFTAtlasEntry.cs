using UnityEngine;
using System.Collections;
using UnityEditor;


public class UFTAtlasEntry{	
	
	public UFTAtlas uftAtlas;
	public Rect canvasRect;
	public Texture2D texture;
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	public UFTTextureState textureState=UFTTextureState.passive;
	
	public long blinkTimeout=5000000; //interval in ticks There are 10,000 ticks in a millisecond
	public Color blinkColor1=Color.red;
	public Color blinkColor2=Color.yellow;
	private Color currentBlinkColor;
	private long? controlBlinkTime=null;
	public bool isSizeInvalid=false;
	
	
	
	
	
	public UFTAtlasEntry (Rect canvasRect, Texture2D texture, UFTAtlas uftAtlas)
	{
		this.canvasRect = canvasRect;
		this.texture = texture;
		UFTAtlasEditorEventManager.onAtlasSizeChanged+=onAtlasSizeChanged;
		this.uftAtlas=uftAtlas;
	}
	
	public void draw(){
		
		if (Event.current.type == EventType.MouseUp){
			textureState=UFTTextureState.passive;
			isDragging = false;			
			if (UFTAtlasEditorEventManager.onStopDragging!=null)
				UFTAtlasEditorEventManager.onStopDragging();
		} else if (Event.current.type == EventType.MouseDown && canvasRect.Contains (Event.current.mousePosition)){
			textureState=UFTTextureState.onDrag;
			isDragging = true;						
			textureState=UFTTextureState.onDrag;
			mouseStartPosition=Event.current.mousePosition;	
			if (UFTAtlasEditorEventManager.onTextureOnCanvasClick!=null)
				UFTAtlasEditorEventManager.onTextureOnCanvasClick(this);
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
				
				if (canvasRect.xMax > (int)uftAtlas.atlasWidth){
					canvasRect.x=	(int)uftAtlas.atlasWidth-texture.width;
				}
				
				if (canvasRect.yMax > (int)uftAtlas.atlasHeight){
					canvasRect.y=	(int)uftAtlas.atlasHeight-texture.height;
				}
				
				
				mouseStartPosition=Event.current.mousePosition;
				
			}
			if (UFTAtlasEditorEventManager.onDragInProgress!=null)
				UFTAtlasEditorEventManager.onDragInProgress();
			
			//if dragging lets color it to drag color border
			GUI.color=UFTTextureUtil.borderColorDict[UFTTextureState.onDrag];
			
			
			
		}
		
		if (isSizeInvalid){
			long currentTime=System.DateTime.Now.Ticks;
			
			if (controlBlinkTime==null){
				controlBlinkTime=currentTime+blinkTimeout;
				currentBlinkColor=blinkColor1;
			}
			
			if (controlBlinkTime <= currentTime){
				
				controlBlinkTime=currentTime+blinkTimeout;
				currentBlinkColor=(currentBlinkColor==blinkColor1)?blinkColor2:blinkColor1;
			}
			GUI.color=currentBlinkColor;
			
		}
		
		
		EditorGUI.DrawPreviewTexture(canvasRect,texture);			
		


		if (isDragging || isSizeInvalid)
			GUI.color=color;

	}
	
	
	private void onAtlasSizeChanged(int width, int height){
		if (texture.width>width || texture.height>height){
			isSizeInvalid=true;	
		}else{
			isSizeInvalid=false;
		}
		controlBlinkTime=null;
	}
	
}