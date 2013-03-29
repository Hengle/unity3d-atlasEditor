using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public class UFTMaterialUtil {	
	public static Dictionary<string,Texture> getTextures(Material material){		
		string assetPath=AssetDatabase.GetAssetPath(material);		
		if (String.IsNullOrEmpty(assetPath))
			return null;
		
		Shader shader=material.shader;			
		Dictionary<string,Texture> textures=new Dictionary<string, Texture>();
		
		for (int i = 0; i < ShaderUtil.GetPropertyCount(shader) ; i++) {						
			if (ShaderUtil.GetPropertyType(shader,i) == ShaderUtil.ShaderPropertyType.TexEnv){				
				string propertyName=ShaderUtil.GetPropertyName(shader,i);				
				textures.Add(propertyName, material.GetTexture(propertyName));													
			}			
		}		
		return textures;
	}
	
	public static List<UFTMaterialWitTextureProps> getMaterialListByTexture (Texture texture)
	{
		
		List<UFTMaterialWitTextureProps> result=new List<UFTMaterialWitTextureProps>();
		Material[] allMaterials=(Material[])Resources.FindObjectsOfTypeAll(typeof(Material));
		
		foreach(Material mat in allMaterials){
	
			Dictionary<string,Texture> textures=UFTMaterialUtil.getTextures(mat);
			if (textures !=null && textures.Count>0){
				List<string> properties=new List<string>();
				foreach(KeyValuePair<string,Texture> keyValue in textures){				
					if (AssetDatabase.Equals(texture,keyValue.Value))
						properties.Add(keyValue.Key);								
				}
				if (properties.Count > 0){
					result.Add(new UFTMaterialWitTextureProps(mat,properties));
				}				
			}
		}
		return result;
	}
}
