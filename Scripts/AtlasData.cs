// Copyright (c) 2012 Eric Salczynski and Ramón Rocha
// This program (Kindred Sprite) is released under the MIT License.
// http://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections;

[System.Serializable]

/// <summary>
/// Atlas data about a frame on a sprite sheet
/// </summary>
public class AtlasData {
    public string name = "";
    public Vector2 position = Vector2.zero;
    public Vector2 offset = Vector2.zero;
    public bool rotated = false;
    public Vector2 size = Vector2.zero;
    public Vector2 frameSize = Vector2.zero;
}
