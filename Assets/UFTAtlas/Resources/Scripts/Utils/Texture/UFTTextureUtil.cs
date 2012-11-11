using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public class UFTTextureUtil : MonoBehaviour {
	public static Color bgColor1=Color.white;
	public static Color bgColor2=new Color(0.8f,0.8f,0.8f,1f);
	
	
	private static GUIStyle _borderStyle;  

	public static GUIStyle borderStyle {
		get {
			if (_borderStyle == null){
				_borderStyle=new GUIStyle();
				_borderStyle.normal.background=UFTTextureUtil.createOnePxBorderTexture();
				_borderStyle.border=new RectOffset(1,1,1,1);
				_borderStyle.alignment=TextAnchor.MiddleCenter;
			}			
			return _borderStyle;
		}
		set {
			_borderStyle = value;
		}
	}
	
	private static Texture2D _atlasCanvasBGTile;

	public static Texture2D atlasCanvasBGTile {
		get {
			if (_atlasCanvasBGTile==null)
				_atlasCanvasBGTile=getAtlasCanvasBGTile();
			return _atlasCanvasBGTile;
		}
		set {
			_atlasCanvasBGTile = value;
		}
	}	
	
	

	public static Dictionary<UFTTextureState,Color> borderColorDict=new Dictionary<UFTTextureState, Color>(){
		{UFTTextureState.passive, GUI.color},
		{UFTTextureState.onDrag, Color.green},
		{UFTTextureState.showBorder, Color.yellow}		
	};
	
	
	
	
	
	
	
	
	
	
	public static Texture2D createTexture(int width, int height, Color color){
		Texture2D tex = new Texture2D(width,height,TextureFormat.ARGB32,false);
		Color32[] pixels=tex.GetPixels32();
		for (int i=0;i<pixels.Length;i++){
			pixels[i]=color;	
		}			
		tex.SetPixels32(pixels);
		tex.Apply(false);		
	    return tex;
	}
	
	public static Texture2D createTexture(int width, int height, Color32[] colors){
		Texture2D tex = new Texture2D(width,height,TextureFormat.ARGB32,false);				
		tex.SetPixels32(colors);
		tex.Apply(false);		
	    return tex;
	}
	
	
	public static void saveTextureToFile(Texture2D texture, string fileName){
			
		string directoryPath=Application.dataPath +"/data";
		byte[] byteArray=texture.EncodeToPNG();	
		if (!Directory.Exists(directoryPath))
			Directory.CreateDirectory(directoryPath);
		
		FileStream file=File.Open(directoryPath+"/" +fileName + ".png",FileMode.Create);
		BinaryWriter bw=new BinaryWriter(file);
		bw.Write(byteArray);
		file.Close();
	}

	
	public static Texture2D createOnePxBorderTexture(){
		string assetPath="Assets/UFTAtlas/Editor/Texture/onePxBorder.png";
		
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Mesh));
		if (texture==null){
			texture=new Texture2D(3,3);
			Color[] c=new Color[9];
			for (int i=0;i<9;i++){
				c[i]=new Color(1,1,1,1);	
			}
			c[4]=new Color(1,1,1,0); //center alpha is empty
			texture.SetPixels(c);
			texture.Apply();
			
			
			
			
			//save to files an then import
			 byte[] bytes = texture.EncodeToPNG();
		    if (bytes != null)
		      File.WriteAllBytes(assetPath, bytes);
		    Object.DestroyImmediate((Object) texture);
		    AssetDatabase.ImportAsset(assetPath);
		    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		    textureImporter.textureFormat=TextureImporterFormat.ARGB32;
		    textureImporter.textureType=TextureImporterType.Advanced;
			textureImporter.mipmapEnabled=false;
			textureImporter.wrapMode=TextureWrapMode.Clamp;
			textureImporter.filterMode=FilterMode.Point;
			textureImporter.npotScale=TextureImporterNPOTScale.None;
		    AssetDatabase.ImportAsset(assetPath);
		    texture= (Texture2D) AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D));			
			
			
			
			
			
		}
		return texture;
	}
	
	
	
	public static Texture2D getAtlasCanvasBGTile(){
		int squareWidth=1;
		int textureWidth=2;
		
		string assetPath="Assets/UFTAtlas/Editor/Texture/AtlasCanvasBGTile.png";
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Mesh));
		if (texture==null){
			texture=createAtlasCanvasBGTexture(textureWidth,textureWidth,bgColor1,bgColor2, squareWidth);
			byte[] bytes = texture.EncodeToPNG();
		    if (bytes != null)
		      File.WriteAllBytes(assetPath, bytes);
			 AssetDatabase.ImportAsset(assetPath);
		    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		     textureImporter.textureFormat=TextureImporterFormat.ARGB32;
		    textureImporter.textureType=TextureImporterType.Advanced;
			textureImporter.mipmapEnabled=false;
			textureImporter.wrapMode=TextureWrapMode.Repeat;
			textureImporter.filterMode=FilterMode.Point;
			textureImporter.npotScale=TextureImporterNPOTScale.None;
		    AssetDatabase.ImportAsset(assetPath);
		    texture= (Texture2D) AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D));
		}
		return texture;
	}
	
	
	
	//here we just generate Texture2d
	 static Texture2D createAtlasCanvasBGTexture(int width, int height, Color bgColor1,Color bgColor2, int squareWidth){
	
		
		Texture2D texture=new Texture2D(width,height);
		
			
		Color[] pixels=new Color[width*height];
		Color rowFirstColor=bgColor1;
		
		Color currentColor=bgColor1;
		for (int textHeight = 0; textHeight < height; textHeight+=squareWidth) {
			rowFirstColor=(rowFirstColor==bgColor1)?bgColor2:bgColor1;
			currentColor=rowFirstColor;
			for (int textWidth = 0; textWidth < width; textWidth+=squareWidth) {			
				int initPosition=textWidth+(textHeight*width);
				
				// paint pixels on this and nex squareWidth row
				for (int cubeWidth = 0; cubeWidth < squareWidth; cubeWidth++) {
					for (int cubeHeight = 0; cubeHeight < squareWidth; cubeHeight++) {						
						pixels[initPosition+cubeWidth+(width*cubeHeight)]=currentColor;	
					}					
				}
				currentColor=(currentColor==bgColor1)?bgColor2:bgColor1;
			}
		}
		texture.SetPixels(pixels);	
		texture.Apply();
		
		return texture;
	}
}
