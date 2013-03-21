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

	private Transform _transform = null;
	private SpriteData[] _spriteData;
	private MeshFilter _meshFilter;			// MeshFilter component added to GameObject by script
	private Mesh _mesh;						// mesh object created by script, and added to MeshFilter
	private bool _meshChanged = false;

	public Vector2 position {
		get {
			return new Vector2 (_transform.position.x, _transform.position.y);
		}

		set {
			_transform.position = new Vector3 (value.x, value.y, _transform.position.z);
		}
	}

	void Awake ()
	{
		_transform = transform;
		_meshFilter = gameObject.GetComponent<MeshFilter> ();
		InitMeshAndSpriteData ();

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
		//
		// This is too resource intensive, and probably shouldn't be run on every Sprite instance.
		// If necessary, it would be best to move to a scene-controller-like game object. Ideally,
		// we could figure out how to stop the material leaks.
		//Resources.UnloadUnusedAssets ();
	}

	void Update ()
	{
		if (!Application.isPlaying) {
			if (spriteContainer != _spriteContainer && spriteContainer != null) {
				InitMeshAndSpriteData ();
				_spriteContainer = spriteContainer;
			} else if (spriteContainer == null) {
				Reset ();
			}

			// The frameIndex will only be changed in the editor.
			// To change the frame during runtime, use ShowFrame();
			if (_spriteData != null) {
				if (frameIndex < 0) {
					frameIndex = 0;
				} else if (frameIndex >= _spriteData.Length) {
					frameIndex = _spriteData.Length - 1;
				}

				ShowFrame (frameIndex);
			}
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

	// Reset all public vars, and remove any dynamically added game objects.
	public void Reset ()
	{
		spriteContainer = null;
		_spriteContainer = null;
		frameIndex = 0;
		_spriteData = null;

		_mesh = null;
		_meshChanged = false;

		_meshFilter.sharedMesh = null;
		renderer.sharedMaterial = null;
	}

	/*** Private ***/

	private void InitMeshAndSpriteData ()
	{
		if (spriteContainer != null) {
			if (_mesh == null) {
				_mesh = new Mesh ();
				_mesh.name = spriteContainer.name;

				if (Application.isPlaying) {
					_meshFilter.mesh = _mesh;
					renderer.material = spriteContainer.material;
				} else {
					_meshFilter.sharedMesh = _mesh;
					renderer.sharedMaterial = spriteContainer.material;
				}
			} else {
				_mesh = (Application.isPlaying) ? _meshFilter.mesh : _meshFilter.sharedMesh;
			}

			if (_spriteData == null || _spriteData.Length == 0) {
				_spriteData = spriteContainer.spriteData;
			}

			UpdateMesh ();
		}
	}

	// http://docs.unity3d.com/Documentation/ScriptReference/Mesh.html
	private void UpdateMesh ()
	{
		if (_mesh != null && _spriteData != null && _spriteData.Length > 0) {
			_mesh.Clear ();
			_mesh.vertices = _spriteData [frameIndex].vertices;
			_mesh.uv = _spriteData [frameIndex].uvs;
			_mesh.triangles = _spriteData [frameIndex].triangles;
			_mesh.normals = _spriteData [frameIndex].normals;
			_transform.localScale = _spriteData [frameIndex].RecalculateSize ();
			_mesh.RecalculateBounds ();
		}
	}
}
