using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

// The material has to be shared between game objects, and if it's
// created and stored in a variable on the SpriteContainer object,
// which lives in the hierarchy, then there's no need to create
// new instances of the material.
//
// Shared materials are one of the things that trigger Unity's
// dynamic batching.
// - http://docs.unity3d.com/Documentation/Manual/DrawCallBatching.html
public class Sprite : MonoBehaviour
{
	// There's no good way to tell if an editor property has changed
	// except to keep a copy of the value, and then in the Update()
	// method, check if the public var (which is exposed to the editor)
	// matches the private var. If it doesn't match, then in Update()
	// do what we gotta do when the editor property changes.
	public SpriteContainer spriteContainer;
	private SpriteContainer _spriteContainer = null;
	public int frameIndex = 0;
	public SpriteData[] spriteData;
	
	private MeshFilter _meshFilter;			// MeshFilter component added to GameObject by script
	private Mesh _mesh;						// mesh object created by script, and added to MeshFilter
	private bool _meshChanged = false;

	// Use this for initialization
	void Start ()
	{
		// add a MeshFilter object if it doesn't exist
		_meshFilter = gameObject.GetComponent<MeshFilter> ();
		if (_meshFilter == null) {
			_meshFilter = gameObject.AddComponent<MeshFilter> ();
		}
		
		// add a MeshRenderer if it doesn't exist
		if (renderer == null) {
			gameObject.AddComponent<MeshRenderer> ();
		}
		
		InitMeshAndSpriteData ();
		_spriteContainer = spriteContainer;
		
		// The important thing to remember is that Unity automatically batches draw calls if the sprites
		// are using the same material. I assumed that creating the material on the SpriteContainer and
		// then pointing all renderer materials to that instance would work. But I think that's why I'm
		// getting material memory leaks.
		// - http://answers.unity3d.com/questions/192561/renderermaterials-leaking-materials.html#answer-container-192915
		//
		// This prevents the "Cleaning up leaked objects..." message. I believe it's necessary because of
		// how we're instantiating/sharing the material from the SpriteContainer object.
		// My hypothesis: When the render is created/added to the GameObject, it already has a default material
		// assigned. When we set the material for that renderer to the material created on the SpriteContainer
		// object, the material that *was* on the renderer initially is no longer used, but also isn't
		// garbage collected immediately.
		// - http://answers.unity3d.com/questions/38960/cleaning-up-leaked-objects-in-scene-since-no-game.html
		Resources.UnloadUnusedAssets ();
	}

	void Update ()
	{
		if (spriteContainer != _spriteContainer && spriteContainer != null) {
			InitMeshAndSpriteData ();
			_spriteContainer = spriteContainer;
		} else if (spriteContainer == null) {
			Reset ();
		}

		// The frameIndex will only be changed in the editor.
		// To change the frame during runtime, use ShowFrame();
		if (!Application.isPlaying && spriteData != null) {
			if (frameIndex < 0) {
				frameIndex = 0;
			} else if (frameIndex >= spriteData.Length) {
				frameIndex = spriteData.Length - 1;
			}
		
			ShowFrame (frameIndex);
		}
		
		if (_meshChanged) {
			UpdateMesh ();
		}
	}
	
	public void ShowFrame (int index)
	{
		frameIndex = index;
		_meshChanged = true;
	}
	
	/*** Private ***/
	
	// Reset all public vars, and remove any dynamically added game objects.
	private void Reset ()
	{
		spriteContainer = null;
		_spriteContainer = null;
		frameIndex = 0;
		spriteData = null;
		
		_mesh = null;
		_meshChanged = false;
			
		if (Application.isPlaying) {
			_meshFilter.mesh = null;
			renderer.material = null;
		} else {
			_meshFilter.sharedMesh = null;
			renderer.sharedMaterial = null;
		}
	}
	
	private void InitMeshAndSpriteData ()
	{
		if (spriteContainer != null) {
			if (Application.isPlaying) {
				renderer.material = spriteContainer.material;
			} else {
				renderer.sharedMaterial = spriteContainer.material;
			}
			
			BuildSpriteData ();
			UpdateMesh ();
		}
	}
	
	// Build sprite data from atlas data
	private void BuildSpriteData ()
	{
		AtlasData[] data = spriteContainer.atlasData;
		spriteData = new SpriteData[spriteContainer.atlasData.Length];
		
		for (int i = 0; i < data.Length; i++) {
			spriteData [i] = new SpriteData ();
			spriteData [i].sprite = this;
			spriteData [i].index = i;
			spriteData [i].name = data [i].name;
			spriteData [i].size = data [i].size;
			spriteData [i].sheetPixelCoords = data [i].position;
			spriteData [i].UpdateVertices ();
			spriteData [i].UpdateUVs ();
		}
	}
	
	// http://docs.unity3d.com/Documentation/ScriptReference/Mesh.html
	private void UpdateMesh ()
	{
		if (spriteContainer != null && spriteData != null) {
			if (_mesh == null) {
				_mesh = new Mesh ();
				_mesh.name = spriteContainer.material.mainTexture.name;
				
				if (Application.isPlaying) {
					_meshFilter.mesh = _mesh;
				} else {
					_meshFilter.sharedMesh = _mesh;
				}
			} else {
				_mesh.Clear ();
			}
			
			_mesh.vertices = spriteData [frameIndex].vertices;
			_mesh.uv = spriteData [frameIndex].uvs;
			_mesh.triangles = spriteData [frameIndex].triangles;
			_mesh.normals = spriteData [frameIndex].normals;
			spriteData [frameIndex].RecalculateSize ();
			_mesh.RecalculateBounds ();
		}
	}
}
