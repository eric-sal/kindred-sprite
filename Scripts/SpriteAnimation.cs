// Copyright (c) 2012 Eric Salczynski and Ramón Rocha
// This program (Kindred Sprite) is released under the MIT License.
// http://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

/// <summary>
/// A container class used to define an animation's behavior.
/// Works in conjunction with a SpriteContainer object to animate the Sprite.
///
/// A SpriteAnimation may contain one or more AnimationFramesets.
/// Ex:
///  "WalkRight" and "WalkLeft" for a player OR
///  "Enemy1Walk" and "Enemy2Walk" for two different enemies that share the same SpriteContainer.
/// </summary>
public class SpriteAnimation : MonoBehaviour {
    public AnimationFrameset[] framesets;    // An array of framesets defined in the Unity editor

    /// <summary>
    /// Get the AnimationFrameset object named `framesetName` from the array of defined framesets.
    /// </summary>
    /// <returns>
    /// An AnimationFrameset object.
    /// </returns>
    /// <param name='framesetName'>
    /// The name of the AnimationFrameset to get.
    /// </param>
    public AnimationFrameset GetFrameset(string framesetName) {
        AnimationFrameset frameset = null;

        if (framesetName == "") {
            return null;
        }

        for (int f = 0; f < framesets.Length; f++) {
            if (framesets[f].name.ToLower() == framesetName.ToLower()) {    // Case insensitive comparison
                frameset = framesets[f];
                break;
            }
        }

        return frameset;
    }
}
