using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


/*
 * Thanks AngryAnt for a greate drag'n'drop example at this page http://angryant.com/2009/09/18/gui-drag-drop/
 * 
 */



public class UFTAtlasEditor : EditorWindow {

	private UFTAtlas uftAtlas;
	public Vector2 scrollPosition = Vector2.zero;
	
	

	
	[MenuItem ("Window/Free Atlas Maker")]
    static void ShowWindow () {    		
		EditorWindow.GetWindow <UFTAtlasEditor>();				
    }
	
	void OnEnable(){
		uftAtlas=UFTAtlas.CreateInstance<UFTAtlas>();
		UFTAtlasEditorEventManager.onNeedToRepaint+=onNeedToRepaint;
	}
	
	private void onNeedToRepaint(){
		Repaint ();
	}	
	
	void OnGUI () {
		
	
		
		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal (GUILayout.MinHeight(100f));
				UFTAtlasSize newWidth =(UFTAtlasSize) EditorGUILayout.EnumPopup("atlas width",uftAtlas.atlasWidth);
				UFTAtlasSize newHeight =(UFTAtlasSize) EditorGUILayout.EnumPopup("atlas height",uftAtlas.atlasHeight);	
				
				if (newWidth!=uftAtlas.atlasWidth || newHeight!=uftAtlas.atlasHeight){
					uftAtlas.atlasWidth=newWidth;
					uftAtlas.atlasHeight=newHeight;
					
					if (UFTAtlasEditorEventManager.onAtlasSizeChanged!=null)
						UFTAtlasEditorEventManager.onAtlasSizeChanged((int)newWidth,(int)newHeight);
				}
			EditorGUILayout.EndHorizontal();
		
			
		
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
				
				
			GUILayoutUtility.GetRect((int)uftAtlas.atlasWidth,(int)uftAtlas.atlasHeight);
			uftAtlas.OnGUI();
		
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
					uftAtlas.addNewEntry(texture);
						
				}
				
			}
		}
		
		if (!addedSomething)
			Debug.Log("there was no any Texture2D in dropped content");
	}

	
	
	
	private bool isTextureCanvasContainsTexture (Texture2D texture)
	{
		return uftAtlas.texturesOnCanvas.Find(toc => toc.texture == texture)!=null;
	}
	
}
