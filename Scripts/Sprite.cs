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
public class Sprite : MonoBehaviour {
    [OnChange ("UpdateSpriteContainer")] public SpriteContainer spriteContainer;
    [OnChange ("UpdateFrameIndex")] public int frameIndex = 0;
    [OnChange ("UpdateDepth")] public float depth = 0;        // z-depth of sprite

    private Transform _transform = null;
    private SpriteData[] _spriteData;
    private MeshFilter _meshFilter;     // MeshFilter component added to GameObject by script
    private Mesh _mesh;                 // mesh object created by script, and added to MeshFilter
    private bool _meshChanged = false;

    public Vector2 position {
        get { return new Vector2(_transform.position.x, _transform.position.y); }
        set { _transform.position = new Vector3(value.x, value.y, _transform.position.z); }
    }

    public virtual void Awake() {
        _transform = transform;
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        InitMeshAndSpriteData();

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
        //Resources.UnloadUnusedAssets();
    }

    public virtual void Start() { }

    public virtual void Update() {
        if (_meshChanged) {
            UpdateMesh();
        }

        // I'm not entirely sure what this does, but it seems it's necessary to
        // properly update the properties that have the "OnChange" attribute.
        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }

    public virtual void ShowFrame(int index) {
        frameIndex = index;
        _meshChanged = true;
    }

    // Reset all public vars, and remove any dynamically added game objects.
    public virtual void Reset() {
        spriteContainer = null;
        frameIndex = 0;
        _spriteData = null;

        _mesh = null;
        _meshChanged = false;

        _meshFilter.sharedMesh = null;
        renderer.sharedMaterial = null;
    }

    public void UpdateSpriteContainer(SpriteContainer newVal) {
        spriteContainer = newVal;

        if (spriteContainer != null) {
            InitMeshAndSpriteData();
        } else {
            Reset();
        }
    }

    public void UpdateFrameIndex(int newVal) {
        if (newVal < 0) {
            frameIndex = 0;
        } else if (newVal >= _spriteData.Length) {
            frameIndex = _spriteData.Length - 1;
        } else {
            frameIndex = newVal;
        }

        UpdateMesh();
    }

    public void UpdateDepth(float newVal) {
        depth = newVal;
        spriteContainer.UpdateVertices(depth);
        UpdateMesh();
    }

    /*** Private ***/

    private void InitMeshAndSpriteData() {
        if (spriteContainer != null) {
            if (_mesh == null) {
                _mesh = new Mesh();
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

            UpdateDepth(depth);
            UpdateMesh();
        }
    }

    // http://docs.unity3d.com/Documentation/ScriptReference/Mesh.html
    private void UpdateMesh() {
        if (_mesh != null && _spriteData != null && _spriteData.Length > 0) {
            _mesh.Clear();
            _mesh.vertices = spriteContainer.vertices;
            _mesh.triangles = spriteContainer.triangles;
            _mesh.normals = spriteContainer.normals;
            _mesh.uv = _spriteData[frameIndex].uvs;
            _transform.localScale = _spriteData[frameIndex].RecalculateSize();
            _mesh.RecalculateBounds();
        }

        _meshChanged = false;
    }
}
