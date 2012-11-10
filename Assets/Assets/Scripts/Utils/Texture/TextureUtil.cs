using UnityEngine;
using System.Collections;
using System.IO;

public class TextureUtil : MonoBehaviour {

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
	/*
	 function SaveTextureToFile( texture: Texture2D,fileName)
 {
    var bytes=texture.EncodeToPNG();
    var file = new File.Open(Application.dataPath + "/"+fileName,FileMode.Create);
    var binary= new BinaryWriter(file);
    binary.Write(bytes);
    file.Close();
 }
 
 */
}
