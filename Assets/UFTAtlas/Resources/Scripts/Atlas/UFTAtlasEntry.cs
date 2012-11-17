using UnityEngine;
using System.Collections;
using UnityEditor;
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
			float x = canvasRect.x == 0 ? 0 : (float) uftAtlas.atlasWidth / (float)canvasRect.x;
			float y = canvasRect.y == 0 ? 0 : (float) uftAtlas.atlasHeight / (float)canvasRect.y;
			float width = (float) uftAtlas.atlasWidth / (float)canvasRect.width;
			float height= (float) uftAtlas.atlasHeight / (float)canvasRect.height;
			return new Rect(x,y,width,height);
		}
	}
	
	
	public UFTAtlas uftAtlas;		
	private bool isDragging=false;
	private Vector2 mouseStartPosition;
	public UFTTextureState textureState=UFTTextureState.passive;
	
	public long blinkTimeout=5000000; //interval in ticks There are 10,000 ticks in a millisecond
	public Color blinkColor1=Color.red;
	public Color blinkColor2=Color.yellow;
	private Color currentBlinkColor;
	private long? controlBlinkTime=null;
	public bool isSizeInvalid=false;
	
	
	public void OnEnable(){
		hideFlags = HideFlags.HideAndDontSave;			
	}

	
	public UFTAtlasEntry (Rect canvasRect, Texture2D texture, UFTAtlas uftAtlas)
	{
		this.canvasRect = canvasRect;
		this.texture = texture;
		UFTAtlasEditorEventManager.onAtlasSizeChanged+=onAtlasSizeChanged;
		this.uftAtlas=uftAtlas;
	}
	
	public void OnGUI(){		
		if (Event.current.type == EventType.MouseUp){
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
			if (UFTAtlasEditorEventManager.onNeedToRepaint!=null)
				UFTAtlasEditorEventManager.onNeedToRepaint();
		}
		
		GUI.DrawTexture(canvasRect,texture,ScaleMode.ScaleToFit,true);
		//EditorGUI.DrawPreviewTexture(canvasRect,texture);			
		


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
	
	public void readPropertiesFromMetadata(UFTAtlasEntryMetadata metadata){
		Texture2D texture=(Texture2D) AssetDatabase.LoadAssetAtPath(metadata.assetPath,typeof(Texture2D));	
		if (texture==null){
			throw new TextureDoesNotExistsException(metadata.assetPath);	
		} else {
			this.texture=texture;
			this.canvasRect=metadata.pixelRect;
			this.name=metadata.name;
			this.isTrimmed=metadata.isTrimmed;
			if (this.isTrimmed){
				this.texture=UFTTextureUtil.trimTextureAlpha(this.texture);					
			}						
		}
	}
	
}
