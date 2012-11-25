using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class UFTAtlasEntry:ScriptableObject{	
	
	[SerializeField]
	public Texture2D texture;
	
	[SerializeField]
	public Rect canvasRect;
	
	[SerializeField]
	public string assetPath;
	
	[SerializeField]
	public string textureName;
	
	
	[SerializeField]
	public bool isTrimmed=false;
	
	
	
	
	public static int _idCounter;
	
	[SerializeField]
	public int? _id;

	public int? id {
		get {
			if (this._id==null){
				_idCounter++;
				this._id=_idCounter;
			}
			return this._id;
		}		
	}	
	
	
	public Rect uvRect {
		get {
			float x = (float)canvasRect.x / (float) uftAtlas.atlasWidth;
			
			float width = (float)canvasRect.width / (float) uftAtlas.atlasWidth;
			float height= (float)canvasRect.height / (float) uftAtlas.atlasHeight;
			
			float y = 1-height - (float)canvasRect.y / (float) uftAtlas.atlasHeight;
			return new Rect(x,y,width,height);
		}
	}
	
	private UFTAtlas _uftAtlas;
	
	public UFTAtlas uftAtlas{
		set{
			this._uftAtlas=value;
			checkSize((int)uftAtlas.atlasWidth,(int)uftAtlas.atlasHeight);
		}
		get{
			return this._uftAtlas;	
		}
	}
	
	
	
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	public UFTTextureState textureState=UFTTextureState.passive;
	
	public long blinkTimeout=5000000; //interval in ticks There are 10,000 ticks in a millisecond
	public Color blinkColor1=Color.red;
	public Color blinkColor2=Color.yellow;
	private Color currentBlinkColor;
	private long? controlBlinkTime=null;
	public bool isSizeInvalid=false;
	private bool stopDragging = false;
	
	public void OnEnable(){
		hideFlags = HideFlags.HideAndDontSave;
		UFTAtlasEditorEventManager.onAtlasSizeChanged+=onAtlasSizeChanged;		
	}

	
	//i'm not sure about it, but i think we need to destroy texture on close if texture is clipped, because it doesn't store in assets
	public void OnDestroy() {
		if (isTrimmed){
			DestroyImmediate(texture,true);	
		}
	}
	
	
	public void OnGUI(){		
		if (Event.current.type == EventType.MouseUp || (isDragging && stopDragging)){
			textureState=UFTTextureState.passive;
			if (isDragging){
				isDragging = false;			
				if (UFTAtlasEditorEventManager.onStopDragging!=null)
					UFTAtlasEditorEventManager.onStopDragging(this);
				if (UFTAtlasEditorEventManager.onAtlasChange!=null)
					UFTAtlasEditorEventManager.onAtlasChange();
			}
		} else if (Event.current.type == EventType.MouseDown && canvasRect.Contains (Event.current.mousePosition)){			
			textureState=UFTTextureState.onDrag;
			isDragging = true;						
			textureState=UFTTextureState.onDrag;
			mouseStartPosition=Event.current.mousePosition;	
			if (UFTAtlasEditorEventManager.onStartDragging!=null)
				UFTAtlasEditorEventManager.onStartDragging(this);
			Event.current.Use();	
			stopDragging=false;
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
			
			
			// check is mouse under texture, if not, initialize dropping
			if (Event.current.type==EventType.Repaint && !canvasRect.Contains (Event.current.mousePosition))
				stopDragging=true;			
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
			if (UFTAtlasEditorEventManager.onNeedToRepaint!=null)
				UFTAtlasEditorEventManager.onNeedToRepaint();
		}
		
		GUI.DrawTexture(canvasRect,texture,ScaleMode.ScaleToFit,true);
		//EditorGUI.DrawPreviewTexture(canvasRect,texture);			
		


		if (isDragging || isSizeInvalid)
			GUI.color=color;

	}
	
	
	// this is event handler, it's called whenever atlas size is changed
	private void onAtlasSizeChanged(int width, int height){
		checkSize (width, height);
	}
	
	
	//this function change status to size invalid if texture size is greater than width, or heigh
	public void checkSize (int width, int height)
	{
		if (texture==null)
			return;
		if (texture.width>width || texture.height>height){
			isSizeInvalid=true;	
		}else{
			isSizeInvalid=false;
		}
		controlBlinkTime=null;
	}
	
	
	//return true if texture has been trimmed, or false if not
	public bool trimTexture ()
	{
		Texture2D newTexture=UFTTextureUtil.trimTextureAlpha (texture);
		if (newTexture!=texture){
			canvasRect.width=newTexture.width;
			canvasRect.height=newTexture.height;
			isTrimmed=true;
			texture=newTexture;
			if (UFTAtlasEditorEventManager.onTextureSizeChanged!=null)
				UFTAtlasEditorEventManager.onTextureSizeChanged(this);			
			return true;			
		}
		return false;
	}
	
	public void readPropertiesFromMetadata(UFTAtlasEntryMetadata metadata){
		Texture2D texture= UFTTextureUtil.importTexture(metadata.assetPath);	
		if (texture==null){
			throw new TextureDoesNotExistsException(metadata.assetPath);	
		} else {
			this.texture=texture;
			this.canvasRect=metadata.pixelRect;
			this.textureName=metadata.name;
			this.isTrimmed=metadata.isTrimmed;
			if (this.isTrimmed){
				trimTexture();
			}						
		}
	}
	
	public UFTAtlasEntryMetadata getMetadata(){
		return new UFTAtlasEntryMetadata(textureName,assetPath,canvasRect,uvRect,isTrimmed);	
	}
	
}
