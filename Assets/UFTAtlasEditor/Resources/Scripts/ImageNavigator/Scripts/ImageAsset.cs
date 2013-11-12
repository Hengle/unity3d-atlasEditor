using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;



public class ImageAsset {
	public Texture2D texture;
	public string name;
	public string path;
	public bool showDetails = false;
	public bool selected;
	public long sizeInBytes;
	public int width;
	public int height;
	public string widthHeight;

	Color32 colorCache;
	bool doCollapse;
	static GUIStyle collapseStyle;




	public ImageAsset(Texture2D texture){
		this.texture = texture;
		width = texture.width;
		height = texture.height;
		widthHeight = width+"x"+height;
		path = AssetDatabase.GetAssetPath(texture);
		sizeInBytes = new FileInfo(path).Length;
		name = Path.GetFileName(path);
		rectDict = new Dictionary<EventType, Rect>();
	}

	bool newShowDetails;

	Rect selectRect;
	bool isMouse;
	bool isMousUp;
	bool returnStatus;
	Dictionary<EventType, Rect> rectDict;

	public bool OnGUI(ref bool repaint ){
		returnStatus = false;
		repaint = false;
		if (selected && rectDict.ContainsKey(Event.current.type)){

			colorCache = GUI.backgroundColor;
			GUI.color = GUI.skin.settings.selectionColor;
			GUI.DrawTexture (rectDict[Event.current.type],EditorGUIUtility.whiteTexture);
			GUI.color = colorCache;

		}


		EditorGUILayout.BeginVertical();

		isMouse = Event.current.isMouse;
		isMousUp = (Event.current.type == EventType.MouseUp);
		newShowDetails = EditorGUILayout.Foldout(showDetails,  name);

		if (newShowDetails != showDetails){
			showDetails = newShowDetails;
			repaint = true;
		} else { 
			if ( (Event.current.type == EventType.used) && isMouse && isMousUp){
			    returnStatus =  true; // clicked on item (maybe) =)
				repaint = true;
			}
		}


		if ( showDetails ){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.TextArea(path);
			EditorGUILayout.TextField("size (bytes)", ""+sizeInBytes);
			EditorGUILayout.TextField("WIDTH x HEIGHT", widthHeight);

			EditorGUILayout.EndVertical();
			if (GUILayout.Button(texture,GUILayout.Width(50),GUILayout.Height(50)))
				Selection.activeObject = texture;

			EditorGUILayout.EndHorizontal();
		}


		EditorGUILayout.EndVertical();




		if (selected){
			if (!rectDict.ContainsKey(Event.current.type))
				rectDict.Add(Event.current.type,new Rect());
			rectDict[Event.current.type]= GUILayoutUtility.GetLastRect();
		}


		return returnStatus;
	}
}

