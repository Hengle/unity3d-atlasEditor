using UnityEngine;
using System.Collections;


// In this script we will store original uv's to mesh.uv2
// and then we will apply texture from attlas, we will multiply original uv2 to atlas position

public class UFTSelectTextureFromAtlas : MonoBehaviour, UFTOnAtlasMigrateInt {	
	private int _textureIndex;	

	public int textureIndex {
		get {
			return this._textureIndex;
		}
		set {
			_textureIndex = value;
			if (atlasMetadata != null)
				atlasEntryMetadataInst=atlasMetadata.entries[textureIndex];
			this.updateUV();
		}
	}

	public UFTAtlasMetadata atlasMetadata;
	public UFTAtlasEntryMetadata atlasEntryMetadataInst;
	
	
	public void Reset(){
		storeOriginalUV();
	}
	
	public void storeOriginalUV ()
	{
		Mesh mesh=UFTMeshUtil.getObjectMesh(gameObject);		
		if (isUV2Empty()){
			mesh.uv2=(Vector2[]) mesh.uv.Clone();		
		} else {
			mesh.uv=(Vector2[]) mesh.uv2.Clone();	
		}
	}	
	
	/// <summary>
	/// Updates mesh uv, we will take original mesh.uv from uv2 coordinates and then multiply to atlas position
	/// </summary>		
	public void updateUV(){
		storeOriginalUV ();
		Rect rect=atlasMetadata.entries[_textureIndex].uvRect;
		Mesh mesh=UFTMeshUtil.getObjectMesh(gameObject);
		Vector2[] uvs=new Vector2[mesh.uv2.Length];
		for (int i=0; i<uvs.Length; i++){
			uvs[i].x = mesh.uv2[i].x * rect.width + rect.x;
			uvs[i].y = mesh.uv2[i].y * rect.height + rect.y;			
		}
		mesh.uv=uvs;		
	}	

	public bool isUV2Empty ()
	{
		Mesh mesh=UFTMeshUtil.getObjectMesh(gameObject);
		return mesh.uv2.Length==0 || mesh.uv2==null;
	}
	
	
	
	
	public void restoreOriginalUVS(){
		Mesh mesh=UFTMeshUtil.getObjectMesh(gameObject);
		if (isUV2Empty ())
			throw new System.Exception("mesh.uv2 of your object is wrong, can't restore original values");
		
		mesh.uv=(Vector2[]) mesh.uv2.Clone();
		
	}

	#region UFTOnAtlasMigrateInt implementation
	public void onAtlasMigrate ()
	{
		// clalculate index (because migration just change public field atlasMetadata and atlasEntryMetadata)
		for (int i = 0; i < atlasMetadata.entries.Length; i++) {
			if (atlasMetadata.entries[i] == atlasEntryMetadataInst){
				textureIndex=i;
				break;
			}
		}		
	}
	#endregion
}
