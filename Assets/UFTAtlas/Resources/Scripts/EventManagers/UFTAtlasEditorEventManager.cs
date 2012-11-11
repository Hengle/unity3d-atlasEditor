using UnityEngine;
using System.Collections;

public delegate void DragInProgress();
public delegate void StopDragging();
public delegate void AtlasSizeChanged(int width, int height);
public delegate void TextureOnCanvasClick(UFTAtlasEntry textureOnCanvas);
public delegate void NeedToRepaint();
public delegate void AtlasChange();


public class UFTAtlasEditorEventManager {
	public static DragInProgress onDragInProgress;
	public static StopDragging onStopDragging;
	public static AtlasSizeChanged onAtlasSizeChanged;
	public static TextureOnCanvasClick onTextureOnCanvasClick;
	public static NeedToRepaint onNeedToRepaint; 
	public static AtlasChange onAtlasChange;	
}
