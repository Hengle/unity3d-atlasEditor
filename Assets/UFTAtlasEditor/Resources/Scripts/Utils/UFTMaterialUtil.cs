using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

internal enum ShaderPropertyType
{
    Color,
    Vector,
    Float,
    Range,
    TexEnv
}


public class UFTMaterialUtil {
	
	public static List<Texture> getTextures(Material material){
		Shader shader=material.shader;			
		List<Texture> materials=new List<Texture>();
		for (int i = 0; i < ShaderUtil.GetPropertyCount(shader) ; i++) {
			Debug.Log("type=="+ShaderUtil.GetPropertyType(shader,i));
			if (ShaderUtil.GetPropertyType(shader,i) == ShaderUtil.ShaderPropertyType.TexEnv){
				materials.Add(material.GetTexture(ShaderUtil.GetPropertyName(shader,i)));
			}			
		}
		return materials;
	}	
}
