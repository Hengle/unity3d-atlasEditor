using UnityEngine;
using System.Collections;

public class UFTSelectTextureFromAtlas : MonoBehaviour {
	
	public int textureIndex;	
	public UFTAtlasMetadata atlasMetadata;
	
	
	
	private Vector2[] originalUV;
	
	
	// Update is called once per frame
	public void updateUV(){
		
	getObjectMesh ();
			
		Rect rect=atlasMetadata.entries[textureIndex].uvRect;
		Mesh mesh=getObjectMesh();
		Vector2[] uvs=mesh.uv;
		if (originalUV==null){
			originalUV =uvs;	
			uvs=new Vector2[originalUV.Length];
		}
		
		for (int i=0; i<uvs.Length; i++){
			uvs[i].x=originalUV[i].x * rect.width + rect.x;
			uvs[i].y=originalUV[i].y * rect.height + rect.y;
			
		}
		mesh.uv=uvs;		
	}
	
	public void returnOriginal(){
		Mesh mesh=getObjectMesh();
		mesh.uv=originalUV;
	}

	public Mesh getObjectMesh ()
	{
		MeshFilter mf= GetComponent<MeshFilter>();
		Mesh mesh;
		if (Application.isEditor){
			mesh=mf.sharedMesh;
		} else {
			mesh=mf.mesh;	
		}
		return mesh;
	}
}
