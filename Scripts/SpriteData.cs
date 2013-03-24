using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteData
{
	public string name = "";
	public Vector2 size;						// size of frame in pixels
	public Vector2 sheetPixelCoords;			// coords of frame on actual image in pixels
	public Texture texture;
	public Vector2[] uvs = new Vector2[4];		// 4 coords for each sprite - upper-left, lower-left, lower-right, upper-right

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
		return new Vector3 (size.x, size.y, 1);
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
