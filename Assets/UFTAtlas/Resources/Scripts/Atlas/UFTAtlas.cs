using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class UFTAtlas : ScriptableObject {

	[SerializeField]
	public UFTAtlasSize atlasWidth=UFTAtlasSize._512;
	
	[SerializeField]
	public UFTAtlasSize atlasHeight=UFTAtlasSize._512;
	
	[SerializeField]
	public List<UFTAtlasEntry> texturesOnCanvas;	

	Texture2D atlasCanvasBG;
	
	public static Texture2D borderTexture;
	
	private static int atlasTileCubeFactor=16; // it equals of the cube size on bg
	private Rect atlasBGTexCoord=new Rect(0,0,1,1);
	
	
	


	private UFTAtlasEntry clickedTextureOnCanvas;
	private bool recreateTexturesPositions=false;
	
	
	void OnEnable() {
		hideFlags = HideFlags.HideAndDontSave;
		initParams ();	
	}
	//uncoment it if you will have a problem with repaint
	/*
	
	*/
	
	
	
	public void OnGUI(){
		int width=(int)atlasWidth;
		int height=(int)atlasHeight;
		
		
		//check if in previous frame we clicked on texture, if we did, move this texture to the last index in collection
		if (recreateTexturesPositions){
			texturesOnCanvas.Remove(clickedTextureOnCanvas);
			texturesOnCanvas.Add(clickedTextureOnCanvas);
			recreateTexturesPositions=false;	
		}
		
		
		// check fi user pressed button delete, in this case we will remove last element in the list
		
		if ((Event.current.type==EventType.keyDown) && (Event.current.keyCode == KeyCode.Delete) && (texturesOnCanvas!=null) && (texturesOnCanvas.Count >0)){			
			removeLatestEntryFromList ();
		}
		
		
		
		Rect canvasRect = new Rect (0, 0, width, height);
		GUI.DrawTextureWithTexCoords(canvasRect,UFTTextureUtil.atlasCanvasBGTile,atlasBGTexCoord,false);
	
	
		if(texturesOnCanvas!=null){
			foreach(UFTAtlasEntry toc in  texturesOnCanvas){					
				toc.draw();
				
			}	
	
			// draw ellow border if mouse under the canvasw
			
			if (canvasRect.Contains (Event.current.mousePosition)){
				
				Color color=GUI.color;
				GUI.color=UFTTextureUtil.borderColorDict[UFTTextureState.showBorder];
					
				foreach(UFTAtlasEntry toc in  texturesOnCanvas){						
					GUI.Box(toc.canvasRect,GUIContent.none,UFTTextureUtil.borderStyle);
				}	
				GUI.color=color;
			}
		
		}
		
	}
	
	
	public void addNewEntry(Texture2D texture){		
		Rect rect=new Rect(0,0,texture.width,texture.height);
		UFTAtlasEntry uftAtlasEntry=UFTAtlasEntry.CreateInstance<UFTAtlasEntry>();
		uftAtlasEntry.canvasRect=rect;
		uftAtlasEntry.texture=texture;
		uftAtlasEntry.uftAtlas=this;
		texturesOnCanvas.Add( uftAtlasEntry);
		if (UFTAtlasEditorEventManager.onAddNewEntry!=null)
			UFTAtlasEditorEventManager.onAddNewEntry(uftAtlasEntry);
		sendEventAtlasChanged();
	}
	
	
	
	void initParams ()
	{	
		
		
		borderTexture=UFTTextureUtil.createOnePxBorderTexture();
		
		
		if(texturesOnCanvas==null)
			texturesOnCanvas=new List<UFTAtlasEntry>();
		
		//init listeners
		UFTAtlasEditorEventManager.onDragInProgress+=onDragInProgressListener;
		UFTAtlasEditorEventManager.onStopDragging+=onStopDraggingListener;
		UFTAtlasEditorEventManager.onStartDragging+=onStartDraggingListener;
		UFTAtlasEditorEventManager.onAtlasSizeChanged+=onAtlasSizeChanged;
		onAtlasSizeChanged((int)atlasWidth,(int)atlasHeight);
	}
	
	
	
	private void onDragInProgressListener(){
		Repaint();	
	}
	
	private void onStopDraggingListener(UFTAtlasEntry uftAtlasEntry){		
		Repaint();			
	}

	void removeLatestEntryFromList ()
	{		
		UFTAtlasEntry latestEntry= texturesOnCanvas[texturesOnCanvas.Count-1];
		UFTAtlasEditorEventManager.onRemoveEntry(latestEntry);			
		texturesOnCanvas.Remove(latestEntry);
		sendEventAtlasChanged();
	}
	
	private void Repaint(){
		if (UFTAtlasEditorEventManager.onNeedToRepaint!=null)
			UFTAtlasEditorEventManager.onNeedToRepaint();
	}
	
	
	private void sendEventAtlasChanged(){
		if (UFTAtlasEditorEventManager.onAtlasChange!=null)
			UFTAtlasEditorEventManager.onAtlasChange();
	}
	
	// we cant move this element to the last position in the list, because in paralel iterator can use list
	// because of that we will just store this value, and in nex frame in OnGUI function
	// we will move this object to the last position
	private void onStartDraggingListener (UFTAtlasEntry textureOnCanvas)
	{		
		clickedTextureOnCanvas=textureOnCanvas;		
		recreateTexturesPositions=true;
	}
	
	
	// in case if atlas size is changed, we need to send event with new size
	void onAtlasSizeChanged (int atlasWidth, int atlasHeight)
	{	
		atlasBGTexCoord=new Rect(0,0,atlasWidth/atlasTileCubeFactor,atlasHeight/atlasTileCubeFactor);				
	}
}
