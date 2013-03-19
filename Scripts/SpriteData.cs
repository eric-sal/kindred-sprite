using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteData
{
	public string name = "";
	public float width;			// pixel size divided by in-game scale value - used to scale the mesh's vertices
	public float height;
	public int index;

	private Vector2 _size;							// size of frame in pixels
	private Vector2 _sheetPixelCoords;				// coords of frame on actual image in pixels
	private Texture _texture;
	private Vector2[] _uvs = new Vector2[4];		// 4 coords for each sprite - upper-left, lower-left, lower-right, upper-right
	private Vector3[] _vertices = new Vector3[4];	// ditto
	private int[] _triangles = new int[6];			// define the triangles of the mesh using the vertex indices - we're winding clockwise
	private Vector3[] _normals = new Vector3[4];

	public Vector2 size {
		get {
			return _size;
		}

		set {
			_size = value;
		}
	}

	public Vector2 sheetPixelCoords {
		set {
			_sheetPixelCoords = value;
		}
	}

	public Texture texture {
		set {
			_texture = value;
		}
	}

	public Vector2[] uvs {
		get {
			return _uvs;
		}
	}

	public Vector3[] vertices {
		get {
			return _vertices;
		}
	}

	public int[] triangles {
		get {
			return _triangles;
		}
	}

	public Vector3[] normals {
		get {
			return _normals;
		}
	}

	public void UpdateVertices ()
	{
		// for a centered pivot point
		_vertices [0] = new Vector3 (-0.5f, -0.5f, 0);	// lower-left
		_vertices [1] = new Vector3 (0.5f, -0.5f, 0);	// lower-right
		_vertices [2] = new Vector3 (-0.5f, 0.5f, 0);	// upper-left
		_vertices [3] = new Vector3 (0.5f, 0.5f, 0);		// upper-right

		// also update the triangles - Clockwise winding
		_triangles [0] = 0;		//	2				2 ___ 3
		_triangles [1] = 2;		//  |\		Verts:	 |\  |
		_triangles [2] = 1;		// 0|_\1			0|_\|1

		_triangles [3] = 2;		//	3__ 4
		_triangles [4] = 3;		//   \ |
		_triangles [5] = 1;		//    \|5

		// and finally, update the normals. Since we know we're in XY 2D space,
		// We can just make the normals face forward, and save some computing time.
		_normals [0] = Vector3.forward;
		_normals [1] = Vector3.forward;
		_normals [2] = Vector3.forward;
		_normals [3] = Vector3.forward;
	}

	public void UpdateUVs ()
	{
		if (_texture != null) {
			Vector2 lowerLeftUV = PixelCoordToUVCoord (_sheetPixelCoords.x, _sheetPixelCoords.y);
			Vector2 uvDimensions = PixelSpaceToUVSpace (_size.x, _size.y);
			Vector2 yOffset = new Vector2 (0, _size.y / (float)_texture.height);

			_uvs [0] = lowerLeftUV - yOffset;									// lower-left
			_uvs [1] = lowerLeftUV - yOffset + uvDimensions.x * Vector2.right;	// lower-right
			_uvs [2] = lowerLeftUV - yOffset + uvDimensions.y * Vector2.up;		// upper-left
			_uvs [3] = lowerLeftUV - yOffset + uvDimensions;					// upper-right
		}
	}

	// By creating our mesh vertices as a 1x1 unit square, we can adjust the GameObject's
	// scale to exactly match the pixel height/width of the sprite.
	// Unity recommends using a 1 unit = 1 meter scale for the best physics emulation
	// results, but I believe that if we also adjust our gravity, we should be fine.
	public Vector3 RecalculateSize ()
	{
		return new Vector3 (_size.x, _size.y, 0);
	}


	// Converts pixel-space values to UV-space scalar values
	// according to the currently assigned material.
	private Vector2 PixelSpaceToUVSpace (float x, float y)
	{
		return new Vector2 (x / ((float)_texture.width), y / ((float)_texture.height));
	}

	// Converts pixel coordinates to UV coordinates according to
	// the currently assigned material.
	private Vector2 PixelCoordToUVCoord (float x, float y)
	{
		Vector2 p = PixelSpaceToUVSpace (x, y);
		p.y = 1.0f - p.y;
		return p;
	}
}
