// Copyright (c) 2012 Eric Salczynski and Ramón Rocha
// This program (Kindred Sprite) is released under the MIT License.
// http://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]

/// <summary>
/// The defined animation behavior of a single set of frames.
/// </summary>
public class AnimationFrameset {
    public enum SelectType { Range, FrameIndex }

    /* *** Member Variables *** */

    public string name;         // AnimationFramesets can be fetched from an array by name or index.
    public float duration = 0;  // The number of seconds it takes to play all frames in the animation.
    public int startFrame;      // Index of the frame in the SpriteContainer().SpriteData to start our animation on.
    public int endFrame;        // Index of the frame to end our animation on.
    public bool startOnRandomFrame = false;	// Start the animation on any frame in our set between startFrame and endFrame inclusively.
    public bool pingPong = false;			// When ping ponging, one complete animation cycle is from the start frame to the end frame, and back down again.
    public bool looping = false;			// Keep playing animation until stopped.
    public int numberOfPlays = -1;			// How many times we should play the animation.
											// If looping == false, and numberOfPlays <= 1, then play the animation once.

    /* *** Properties *** */

    /// <summary>
    /// Gets the number of frames in this set.
    /// </summary>
    /// <value>
    /// The number of frames in this set.
    /// </value>
    public int numberOfFrames {
        get {
            int _numberOfFrames = -1;

            if (startFrame <= endFrame) {
                _numberOfFrames = endFrame - startFrame + 1;
            } else {
                _numberOfFrames = -1;
            }

            return _numberOfFrames;
        }
    }

    /// <summary>
    /// Gets the seconds per frame.
    /// A frame is only displayed on screen for X number of seconds, where X = the duration (in seconds) / the total number of frames in this set.
    /// </summary>
    /// <value>
    /// The duration per frame in seconds.
    /// </value>
    public float secondsPerFrame {
        get { return duration / numberOfFrames; }
    }
}
