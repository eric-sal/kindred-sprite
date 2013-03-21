using UnityEngine;
using System.Collections;

// Inherit from ScriptableObject to get the proper serialization.
// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
// http://answers.unity3d.com/questions/190350/what-is-the-purpose-of-scriptableobject-versus-nor.html#answer-190445
[System.Serializable]
public class SpriteData : ScriptableObject
{
	public string name = "";
	public Vector2 size;							// size of frame in pixels
	public Vector2 sheetPixelCoords;				// coords of frame on actual image in pixels
	public Texture texture;
	public Vector2[] uvs = new Vector2[4];		// 4 coords for each sprite - upper-left, lower-left, lower-right, upper-right
	public Vector3[] vertices = new Vector3[4];	// ditto
	public int[] triangles = new int[6];			// define the triangles of the mesh using the vertex indices - we're winding clockwise
	public Vector3[] normals = new Vector3[4];

	public void UpdateVertices ()
	{
		// for a centered pivot point
		vertices [0] = new Vector3 (-0.5f, -0.5f, 0);	// lower-left
		vertices [1] = new Vector3 (0.5f, -0.5f, 0);	// lower-right
		vertices [2] = new Vector3 (-0.5f, 0.5f, 0);	// upper-left
		vertices [3] = new Vector3 (0.5f, 0.5f, 0);		// upper-right

		// also update the triangles - Clockwise winding
		triangles [0] = 0;		//	2				2 ___ 3
		triangles [1] = 2;		//  |\		Verts:	 |\  |
		triangles [2] = 1;		// 0|_\1			0|_\|1

		triangles [3] = 2;		//	3__ 4
		triangles [4] = 3;		//   \ |
		triangles [5] = 1;		//    \|5

		// and finally, update the normals. Since we know we're in XY 2D space,
		// We can just make the normals face forward, and save some computing time.
		normals [0] = Vector3.forward;
		normals [1] = Vector3.forward;
		normals [2] = Vector3.forward;
		normals [3] = Vector3.forward;
	}

	public void UpdateUVs ()
	{
		if (texture != null) {
			Vector2 lowerLeftUV = PixelCoordToUVCoord (sheetPixelCoords.x, sheetPixelCoords.y);
			Vector2 uvDimensions = PixelSpaceToUVSpace (size.x, size.y);
			Vector2 yOffset = new Vector2 (0, size.y / (float)texture.height);

			uvs [0] = lowerLeftUV - yOffset;									// lower-left
			uvs [1] = lowerLeftUV - yOffset + uvDimensions.x * Vector2.right;	// lower-right
			uvs [2] = lowerLeftUV - yOffset + uvDimensions.y * Vector2.up;		// upper-left
			uvs [3] = lowerLeftUV - yOffset + uvDimensions;						// upper-right
		}
	}

	// By creating our mesh vertices as a 1x1 unit square, we can adjust the GameObject's
	// scale to exactly match the pixel height/width of the sprite.
	// Unity recommends using a 1 unit = 1 meter scale for the best physics emulation
	// results, but I believe that if we also adjust our gravity, we should be fine.
	public Vector3 RecalculateSize ()
	{
		return new Vector3 (size.x, size.y, 0);
	}


	// Converts pixel-space values to UV-space scalar values
	// according to the currently assigned material.
	public Vector2 PixelSpaceToUVSpace (float x, float y)
	{
		return new Vector2 (x / ((float)texture.width), y / ((float)texture.height));
	}

	// Converts pixel coordinates to UV coordinates according to
	// the currently assigned material.
	public Vector2 PixelCoordToUVCoord (float x, float y)
	{
		Vector2 p = PixelSpaceToUVSpace (x, y);
		p.y = 1.0f - p.y;
		return p;
	}
}
