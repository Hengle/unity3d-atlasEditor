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
	[SerializeField]
	private UFTAtlas uftAtlas;
	
	private bool isAtlasDirty=false;
	public Vector2 scrollPosition = Vector2.zero;
	public Vector2 atlasTexturesScrollPosition=Vector2.zero;
	public string ATLAS_FILE_MASK="atlas_";
	
	
	UFTAtlasMetadata atlasMetadata;
	private static string EDITOR_PREFS_KEY_ATLAS_WIDHT="uft.atlasEditor.width";
	private static string EDITOR_PREFS_KEY_ATLAS_HEIGHT="uft.atlasEditor.height";
	
	
	public static int DEFAULT_ATLAS_WIDTH=512;
	public static int DEFAULT_ATLAS_HEIGHT=512;
	
	
	
	[MenuItem ("Window/Free Atlas Editor")]
    static void ShowWindow () {    		
		EditorWindow.GetWindow <UFTAtlasEditor>("Atlas Editor",typeof(SceneView));				
    }
	
	void OnEnable(){
		initOnNewParameters ();
		
		registerListeners ();
		
	}

	public void initOnNewParameters ()
	{
		atlasMetadata=null;
		uftAtlas=UFTAtlas.CreateInstance<UFTAtlas>();
		//get free name
		string atlasName=UFTFileUtil.getFileName(ATLAS_FILE_MASK);
		uftAtlas.atlasName=atlasName;		
		readWidthHeightFromEditorPrefs();		
	}

	public void registerListeners ()
	{
		UFTAtlasEditorEventManager.onNeedToRepaint+=onNeedToRepaint;
		UFTAtlasEditorEventManager.onAtlasChange+=onAtlasChange;		
		UFTAtlasEditorEventManager.onAddNewEntry+=onAddNewEntry;
		UFTAtlasEditorEventManager.onRemoveEntry+=onRemoveEntry;
		UFTAtlasEditorEventManager.onStopDragging+=onStopDragging;
		UFTAtlasEditorEventManager.onStartDragging+=onStartDragging; 
		UFTAtlasEditorEventManager.onTextureSizeChanged+=onTextureSizeChanged;
		UFTAtlasEditorEventManager.onAtlasSizeChanged+=onAtlasSizeChanged;
		UFTAtlasEditorEventManager.onAtlasMetadataObjectChanged+=onAtlasMetadataObjectChanged;
	}
	
	

	public void onAtlasSizeChanged (int width, int height)
	{
		EditorPrefs.SetInt(EDITOR_PREFS_KEY_ATLAS_WIDHT,width);
		EditorPrefs.SetInt(EDITOR_PREFS_KEY_ATLAS_HEIGHT,height);
	}
	
	
	public void readWidthHeightFromEditorPrefs(){
		int width= EditorPrefs.GetInt(EDITOR_PREFS_KEY_ATLAS_WIDHT,DEFAULT_ATLAS_WIDTH);
		int height= EditorPrefs.GetInt(EDITOR_PREFS_KEY_ATLAS_HEIGHT,DEFAULT_ATLAS_HEIGHT);
		uftAtlas.atlasWidth=(UFTAtlasSize) width;
		uftAtlas.atlasHeight=(UFTAtlasSize)height;		
	}

	
	
	
	public void onAddNewEntry (UFTAtlasEntry uftAtlasEntry)
	{		
		Undo.RegisterUndo(uftAtlasEntry,"UFTAtlasEntry" + uftAtlasEntry.id);
	}

	public void onStopDragging (UFTAtlasEntry uftAtlasEntry)
	{
		//
	}

	public void onStartDragging (UFTAtlasEntry uftAtlasEntry)
	{
		registerSnapshotTargetState (uftAtlasEntry);
	}

	public void onRemoveEntry (UFTAtlasEntry uftAtlasEntry)
	{		
		registerSnapshotTargetState (uftAtlasEntry);
	}

	void registerSnapshotTargetState (UFTAtlasEntry uftAtlasEntry)
	{
		registerSnapshotForEntry (uftAtlasEntry);
		
	}

	void onTextureSizeChanged (UFTAtlasEntry uftAtlasEntry)
	{
		registerSnapshotForEntry (uftAtlasEntry);
	}

	void registerSnapshotForEntry (UFTAtlasEntry uftAtlasEntry)
	{
		registerAtlasSnapshot ();
		
		Undo.SetSnapshotTarget(uftAtlasEntry,"stop dragging uftAtlasEntry id="+uftAtlasEntry.id);
		Undo.CreateSnapshot();
		Undo.RegisterSnapshot();
	}
	
	
	public void onAtlasMetadataObjectChanged (UFTAtlasMetadata atlasMetadata)
	{		
		registerAtlasSnapshot ();
		initOnNewParameters();
		if (atlasMetadata!=null){
			//create UFTAtlas from metadata.
			uftAtlas=UFTAtlas.CreateInstance<UFTAtlas>();
			uftAtlas.readPropertiesFromMetadata(atlasMetadata);			
		}
		
	}

	public void registerAtlasSnapshot ()
	{
		//Register Undo for the previous state	
		Undo.SetSnapshotTarget(uftAtlas,"atlas");
		Undo.CreateSnapshot();
		Undo.RegisterSnapshot();
	}
	
		
	void Update(){
		Repaint();	
	}

	private void onAtlasChange ()
	{
		isAtlasDirty=true;
		Undo.CreateSnapshot();
        Undo.RegisterSnapshot();
	}
	
	
	private void onNeedToRepaint(){
		Repaint ();
	}	
	
	void OnGUI () {				
		if (isAtlasDirty){
			EditorUtility.SetDirty(uftAtlas);
			isAtlasDirty=false;
		}
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical (new GUILayoutOption[]{GUILayout.Width(200f)});
				EditorGUILayout.Separator();		
				EditorGUILayout.LabelField("Atlas:");
				string newName=EditorGUILayout.TextField("name",uftAtlas.atlasName);
				if (newName!=uftAtlas.atlasName){
					uftAtlas.atlasName=newName;
				}
		
				UFTAtlasMetadata newMeta= (UFTAtlasMetadata) EditorGUILayout.ObjectField((Object)atlasMetadata,typeof(UFTAtlasMetadata),false);
				if (newMeta!=atlasMetadata){					
					if (uftAtlas.atlasEntries.Count==0 || (uftAtlas.atlasEntries.Count>0 && EditorUtility.DisplayDialog("This action will remove all existing textures on your atlas", "Do you want to proceed?","Proceed","Cancel"))){
						if (UFTAtlasEditorEventManager.onAtlasMetadataObjectChanged!=null)
							UFTAtlasEditorEventManager.onAtlasMetadataObjectChanged(newMeta);
						atlasMetadata=newMeta;
					}				
				
				}
		
				EditorGUILayout.BeginHorizontal();
					
					if (GUILayout.Button("new")){
						if (uftAtlas.atlasEntries.Count==0 || (uftAtlas.atlasEntries.Count>0 && EditorUtility.DisplayDialog("This action will remove all existing textures on your atlas", "Do you want to proceed?","Proceed","Cancel"))){
							onClickNew();		
						}
					}
					if (GUILayout.Button("save"))
						onClickSave();
				EditorGUILayout.EndHorizontal();
				UFTAtlasSize newWidth = (UFTAtlasSize) EditorGUILayout.EnumPopup("width",uftAtlas.atlasWidth);
				UFTAtlasSize newHeight = (UFTAtlasSize) EditorGUILayout.EnumPopup("height",uftAtlas.atlasHeight);	
				
					
				
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Allign:");
		
				uftAtlas.borderSize=EditorGUILayout.IntField("border size",uftAtlas.borderSize);
				if (GUILayout.Button("allign") && uftAtlas!=null && uftAtlas.atlasEntries.Count>0)
					uftAtlas.arrangeEntriesUsingUnityPackager();
				
				
				
				if (newWidth!=uftAtlas.atlasWidth || newHeight!=uftAtlas.atlasHeight){
					uftAtlas.atlasWidth=newWidth;
					uftAtlas.atlasHeight=newHeight;
					
					if (UFTAtlasEditorEventManager.onAtlasSizeChanged!=null)
						UFTAtlasEditorEventManager.onAtlasSizeChanged((int)newWidth,(int)newHeight);
				}
		
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Atlas Textures:");
				if (uftAtlas.atlasEntries.Count>0){
					if (GUILayout.Button("trim alpha")){
						uftAtlas.trimAllEntries();				
					}					
				}
				atlasTexturesScrollPosition = EditorGUILayout.BeginScrollView(atlasTexturesScrollPosition);
				
				if (uftAtlas.atlasEntries.Count >0){
					List<string> names= uftAtlas.atlasEntries.ConvertAll(new System.Converter<UFTAtlasEntry,string>(uftAtlasEntryToString));
					names.Sort();
					foreach (string name in names) {
						EditorGUILayout.LabelField(name,GUILayout.MaxWidth(220f));
					}					
				}		
				EditorGUILayout.EndScrollView();
				EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
			
				
			EditorGUILayout.BeginVertical();	
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
					
					scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
						
							
					GUILayoutUtility.GetRect((int)uftAtlas.atlasWidth,(int)uftAtlas.atlasHeight);
					uftAtlas.OnGUI();
				
					EditorGUILayout.EndScrollView();
					EditorGUILayout.Separator();
				EditorGUILayout.EndHorizontal();				
			EditorGUILayout.EndVertical();
			
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
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
					string assetPath=AssetDatabase.GetAssetPath(texture);					
					uftAtlas.addNewEntry(texture,assetPath);
						
				}
				
			}
		}
		
		if (!addedSomething)
			Debug.Log("there was no any Texture2D in dropped content");
	}

	void onClickSave ()
	{
		string assetPath="Assets/"+uftAtlas.atlasName;
		
		string fullPath=Application.dataPath+"/"+uftAtlas.atlasName;
		bool isAnyAssetsExists=(File.Exists(@fullPath+".asset") || File.Exists(@fullPath+".png"));
	
		
		if (!isAnyAssetsExists || (EditorUtility.DisplayDialog("Some assets with following name "+uftAtlas.atlasName+" already exists", "Do you want to overwrite it?","Yes","No"))){		
			UFTAtlasMetadata metadata=uftAtlas.saveAtlasTextureAndGetMetadata(assetPath);
			AssetDatabase.CreateAsset(metadata,assetPath+".asset");
			atlasMetadata=metadata;
		}
	}

	void onClickNew ()
	{
		registerAtlasSnapshot ();		
		initOnNewParameters();
	}

	
	
	
	private bool isTextureCanvasContainsTexture (Texture2D texture)
	{
		return uftAtlas.atlasEntries.Find(toc => toc.texture == texture)!=null;
	}
	
	
	
	// this function used as converter in OnGUI to show list of the entry names
	private static string uftAtlasEntryToString(UFTAtlasEntry uftAtlasEntry){
		return uftAtlasEntry.textureName;
	}
	
}
