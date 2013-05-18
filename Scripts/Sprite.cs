using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

/// <summary>
/// Simple sprite class.
/// </summary>
public class Sprite : MonoBehaviour {

    /* *** Member Variables *** */

    [OnChange ("UpdateSpriteContainer", typeof(SpriteContainer))] public SpriteContainer spriteContainer;
    [OnChange ("UpdateFrameIndex")] public int frameIndex = 0;  // Index of the frame to display from spriteContainer.SpriteData
    [OnChange ("UpdateDepth")] public float depth = 0;          // z-depth of sprite

    private Transform _transform = null;
    private SpriteData[] _spriteData;
    private MeshFilter _meshFilter;     // MeshFilter component added to GameObject by script
    private Mesh _mesh;                 // mesh object created by script, and added to MeshFilter
    private bool _meshChanged = false;  // We've changed the mesh by changing the frame index

    /* *** Constructors *** */

    public virtual void Start() {
        _transform = transform;
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        InitMeshAndSpriteData();
    }

    /* *** MonoBehaviour Methods *** */

    public virtual void Update() {
        if (_meshChanged) {
            UpdateMesh();
        }
    }

    public virtual void Reset() {
        spriteContainer = null;
        frameIndex = 0;
        _spriteData = null;

        _mesh = null;
        _meshChanged = false;

        _meshFilter.sharedMesh = null;
        renderer.sharedMaterial = null;
    }

    /* *** Public Methods *** */

    /// <summary>
    /// Change the frame index, and mark the mesh as changed for the next Update() call.
    /// </summary>
    /// <param name='index'>
    /// Index of the frame to show
    /// </param>
    public virtual void ShowFrame(int index) {
        frameIndex = index;
        _meshChanged = true;
    }

    /// <summary>
    /// Callback for the OnChange event from the Unity editor.
    /// </summary>
    /// <param name='newVal'>
    /// New SpriteContainer
    /// </param>
    public void UpdateSpriteContainer(SpriteContainer newVal) {
        spriteContainer = newVal;

        if (spriteContainer != null) {
            InitMeshAndSpriteData();
        } else {
            Reset();
        }

        // I'm not entirely sure what this does, but it seems it's necessary to
        // properly update the non-standard properties that have the "OnChange" attribute.
        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }

    /// <summary>
    /// Callback for the OnChange event from the Unity editor.
    /// </summary>
    /// <param name='newVal'>
    /// New frameIndex
    /// </param>
    public void UpdateFrameIndex(int newVal) {
        if (newVal < 0) {
            frameIndex = 0;
        } else if (newVal >= _spriteData.Length) {
            frameIndex = _spriteData.Length - 1;
        } else {
            frameIndex = newVal;
        }

        // We've changed the frame index, so call UpdateMesh() to display it
        UpdateMesh();

        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }

    /// <summary>
    /// Callback for the OnChange event from the Unity editor.
    /// </summary>
    /// <param name='newVal'>
    /// New z-index depth of the sprite
    /// </param>
    public void UpdateDepth(float newVal) {
        depth = newVal;
        spriteContainer.UpdateVertices(depth);

        // We've changed the sprite's verts, so call UpdateMesh() to display it
        UpdateMesh();

        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }

    /* *** Private Methods *** */

    /// <summary>
    /// Create a new mesh (if necessary), fetch the sprite data from our SpriteContainer, and update the mesh.
    /// </summary>
    private void InitMeshAndSpriteData() {
        if (spriteContainer != null) {
            if (_mesh == null) {
                _mesh = new Mesh();
                _mesh.name = spriteContainer.name;

                // Remember, sharing the material is what makes dynamic batching of draw calls happen in Unity.
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

    /// <summary>
    /// Updates the mesh based on the verts, triangles, UVs from the sprite data
    /// http://docs.unity3d.com/Documentation/ScriptReference/Mesh.html
    /// </summary>
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
