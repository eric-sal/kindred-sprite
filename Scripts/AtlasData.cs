using UnityEngine;
using System.Collections;

// Atlas data about a frame on a sprite sheet
[System.Serializable]
public class AtlasData {
    public string name = "";
    public Vector2 position = Vector2.zero;
    public Vector2 offset = Vector2.zero;
    public bool rotated = false;
    public Vector2 size = Vector2.zero;
    public Vector2 frameSize = Vector2.zero;
}
