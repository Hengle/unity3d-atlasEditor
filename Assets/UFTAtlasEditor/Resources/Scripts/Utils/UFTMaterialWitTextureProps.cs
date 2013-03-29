using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UFTMaterialWitTextureProps{
	public Material material;
	public List<string> textureProperties;
	
	public UFTMaterialWitTextureProps (Material material, List<string> textureProperties)
	{
		this.material = material;
		this.textureProperties = textureProperties;
	}

}
