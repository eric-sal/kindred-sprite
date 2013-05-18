using UnityEngine;
using System.Collections;

/// <summary>
/// An animated sprite instance.
/// Inherits from Sprite.
/// </summary>
public class AnimatedSprite : Sprite {

    /* *** Member Variables *** */

    public SpriteAnimation spriteAnimation;
    public bool playOnStart = false;            // Start playing the animationn as soon as the Start() method is called.
    public bool destroyAfterFinished = false;   // After the animation completes playing, destroy the GameObject.

    private AnimationFrameset _currentFrameset;
    private bool _isPlaying = false;
    private float _frameTime = 0;    // The amount of time for which we've been displaying the current frame.
    private int _step = 1;           // The direction we're moving through the current frameset. Foward (1) or backward (-1).
    private int _currentFrame = -1;  // The index of the current frame in the current frameset.
    private int _timesPlayed = 0;    // The number of times we've played the complete animation. The limit is set on the AnimationFrameset in the Unity editor.

    /* *** Constructors *** */

    public override void Start() {
        if (playOnStart) {
            Play();
        }

        base.Start();
    }

    /* *** MonoBehaviour Methods *** */

    public override void Update() {
        if (_currentFrameset != null && _isPlaying) {
            // A frame is only displayed for X number of seconds, where X = AnimationFrameset().secondsPerFrame.
            if (_frameTime >= _currentFrameset.secondsPerFrame) {
                base.ShowFrame(_currentFrame);
                _currentFrame = NextFrame();  // Fetch the next frame for this frameset

                _isPlaying = _currentFrame >= 0; // A frame index of -1 means we're done playing this frameset.
                _frameTime = 0;

                if (!_isPlaying && destroyAfterFinished) {
                    Destroy(gameObject);
                }
            } else {
                // The current frame hasn't been on screen long enough
                _frameTime += Time.deltaTime;
            }
        }

        base.Update();
    }

    /* *** Public Methods *** */

    /// <summary>
    /// Play this instance's animation.
    /// Since no frameset is specified, just play the first frameset in the SpriteAnimation.
    /// </summary>
    public void Play() {
        Play(spriteAnimation.framesets[0]);
    }

    /// <summary>
    /// Play the specified frameset reference by name.
    /// </summary>
    /// <param name='framesetName'>
    /// Name of the frameset to play. The name for a frameset is specified on the SpriteAnimation instance in the Unity editor.
    /// </param>
    public void Play(string framesetName) {
        Play(spriteAnimation.GetFrameset(framesetName));
    }

    /// <summary>
    /// Play the specified frameset referenced by index.
    /// </summary>
    /// <param name='frameset'>
    /// Index of the frameset to play.
    /// </param>
    public void Play(AnimationFrameset frameset) {
        _currentFrameset = frameset;
        if (_currentFrameset != null) {
            ResetAnimation();
        }
    }

    /// <summary>
    /// Set _isPlaying to false so that the current animation stops playing in the next Update() call.
    /// </summary>
    public void Stop() {
        _isPlaying = false;
    }

    /// <summary>
    /// Stop the animation, and show the specified frame.
    /// </summary>
    /// <param name='index'>
    /// Index of the frame to show.
    /// </param>
    public override void ShowFrame(int index) {
        Stop();
        base.ShowFrame(index);
    }

    /* *** Private Methods *** */

    /// <summary>
    /// Based on the current frame we're displaying, get the next frame.
    /// The next frame will depend on whether or not the animation should ping pong, and/or if the animation is looping.
    /// </summary>
    /// <returns>
    /// An integer index to the next frame.
    /// </returns>
    private int NextFrame() {
        int _nextFrame = _currentFrame + _step;  // The next frame is +/- 1 frame, depending...

        if (_currentFrameset.pingPong) {
            // If we're ping-ponging, the animation should go from the start frame to the end frame, and back down again.

            if (_nextFrame < _currentFrameset.startFrame) {
                // When ping ponging, one complete animation cycle is from the start frame to the end frame, and back down again.
                _timesPlayed++;
            }

            if (_nextFrame > _currentFrameset.endFrame ||
             _nextFrame < _currentFrameset.startFrame && _currentFrameset.looping ||
             _nextFrame < _currentFrameset.startFrame && _currentFrameset.numberOfPlays > 0 && _timesPlayed < _currentFrameset.numberOfPlays) {
                // Reverse the direction of our step if:
                // 1. We've reached the end frame OR
                // 2. We've reached the start frame, and we're looping OR
                // 3. We've reached the start frame, we're looping, and we haven't reached the maximum number of plays for this animation

                _step *= -1;
                _currentFrame += _step;
            } else if (_nextFrame < _currentFrameset.startFrame) {
                // Once we get back down to the start frame, the next frame index is -1, so we know to stop playing the animation.
                _currentFrame = -1;
            } else {
                _currentFrame = _nextFrame;
            }
        } else {
            // We're not ping ponging...

            if (_nextFrame > _currentFrameset.endFrame) {
                // ...so one complete animation cycle is from the start frame to the end frame.
                _timesPlayed++;

                if (_currentFrameset.looping ||
                 _currentFrameset.numberOfPlays > 0 && _timesPlayed < _currentFrameset.numberOfPlays) {
                    // Start the animation over from the beginning if:
                    // 1. We're looping OR
                    // 2. We haven't reached the maximum number of plays for this animation

                    _currentFrame = _currentFrameset.startFrame;
                } else {
                    // Otherwise, we're done playing the animation.
                    _currentFrame = -1;
                }
            } else {
                _currentFrame = _nextFrame;
            }
        }

        return _currentFrame;
    }

    /// <summary>
    /// Reset the frameset to be played from the beginning.
    /// </summary>
    private void ResetAnimation() {
        _step = 1;
        _timesPlayed = 0;
        _frameTime = 0;
        _isPlaying = true;

        if (_currentFrameset.startOnRandomFrame) {
            _currentFrame = Random.Range(_currentFrameset.startFrame, _currentFrameset.endFrame);
        } else {
            _currentFrame = _currentFrameset.startFrame;
        }
    }
}
