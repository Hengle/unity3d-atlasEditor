using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;


public class ImageNavigator {

	public List<ImageAsset> images;
	public GenericMenu contextMenu;
	Vector2 scrollPosition;
	int clickedId;
	int previousClickedId = 0;
	bool repaint;
	bool doRepaint;

	SortField sortField;
	SortField newSortField;
	int sort = 0;
	bool ascending = true;
	string asc = "↓";
	string desc="↑";

	Action Repaint;

	public void initialize(Action Repaint){
		this.Repaint = Repaint;
		resortImages();
	}



	enum SortField{
		NAME,
		PATH,
		SIZE,
		WIDTH,
		HEIGHT
	}

	public void OnGUI(){
		if (doRepaint)
			Repaint();

		doRepaint = false;
		clickedId =-1;
		if (images!=null){
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("sort by:");			
			newSortField =(SortField) EditorGUILayout.EnumPopup( sortField);
			if (newSortField!=sortField){
				sortField = newSortField;
				resortImages();
			}

			if ( GUILayout.Button(ascending ? asc: desc)){
				ascending=!ascending;
				resortImages();
			}

			EditorGUILayout.EndHorizontal();


			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < images.Count; i++) {
				if (images[i].OnGUI(ref repaint)){
					clickedId = i;
				}
				if (repaint)
					doRepaint = true;
			}
			EditorGUILayout.EndScrollView();
			if (Event.current.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){
				contextMenu.ShowAsContext();
			}



			if (clickedId!= -1)
				onImageSelect(clickedId);

		}

	}


	public void resortImages(){
		List<ImageAsset> images2 =  new List<ImageAsset>();
		if (images ==null)
			return;
		switch (sortField){

		case SortField.HEIGHT:
			images2 = images.OrderBy(o=>o.height).ToList();
			break;
		case SortField.NAME:
			images2 = images.OrderBy(o=>o.name).ToList();
			break;
		case SortField.PATH:
			images2 = images.OrderBy(o=>o.path).ToList();
			break;
		case SortField.SIZE:
			images2 = images.OrderBy(o=>o.sizeInBytes).ToList();
			break;
		case SortField.WIDTH:
			images2 = images.OrderBy(o=>o.width).ToList();
			break;
		default:
			Debug.LogError("unknonw type here");
			break;
		}

		if (!ascending)
			images2.Reverse();

		images = images2;

	}

	void onImageSelect(int clickedId){
		if (Event.current.control || Event.current.command){
			images[clickedId].selected = !images[clickedId].selected;
		} else if  (Event.current.shift){
			selectWithShift(clickedId,previousClickedId);
		} else {
			resetAllButOne(clickedId);
		}
	}

	void selectWithShift(int activeId, int previousId){
		int from = Mathf.Min(activeId, previousId);
		int to = Mathf.Max(activeId, previousId);
		for (int i = 0; i < images.Count; i++) {
			images[i].selected = i>=from && i <=to;
		}
	}

	void resetAllButOne(int activeId){
		for (int i = 0; i < images.Count; i++) {
			images[i].selected = false;
		}
		images[activeId].selected = !images[activeId].selected;
		previousClickedId= clickedId;
	}
}
