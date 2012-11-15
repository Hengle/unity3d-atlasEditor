using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class UFTAtlasEntryMetadata{
	[SerializeField]
	public string _name;
	
	[SerializeField]
	public string _assetPath;
	
	[SerializeField]
	public Rect _pixelRect;
	
	[SerializeField]
	public Rect _uvRect;
	
	[SerializeField]
	public bool _isTrimmed;
	
	public UFTAtlasEntryMetadata (string name, string assetPath, Rect pixelRect, Rect uvRect, bool isTrimmed)
	{
		this._name = name;
		this._assetPath = assetPath;
		this._pixelRect = pixelRect;
		this._uvRect = uvRect;
		this._isTrimmed = isTrimmed;
	}

	
	
	public string AssetPath {
		get {
			return this._assetPath;
		}		
	}

	public bool IsTrimmed {
		get {
			return this._isTrimmed;
		}		
	}

	public string Name {
		get {
			return this._name;
		}		
	}

	public Rect PixelRect {
		get {
			return this._pixelRect;
		}		
	}

	public Rect UvRect {
		get {
			return this._uvRect;
		}		
	}
}



public class UFTAtlasMetadata : ScriptableObject {
	
	public UFTAtlasEntryMetadata[] entries;
	
}
