using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

[System.Serializable]
public class AnimationFrameset
{
	public enum SelectType { Range, FrameIndex }

	public string name;
	public float duration = 0;		// The number of seconds it takes to play all frames in the animation.
	public int startFrame;
	public int endFrame;
	public bool startOnRandomFrame = false;
	public bool pingPong = false;
	[OnChange ("UpdateLooping")] public bool looping = false;			// Keep playing animation until stopped.
	[OnChange ("UpdateNumberOfPlays")] public int numberOfPlays = -1;	// How many times we should play the animation.
																		// If looping == false, and numberOfPlays <= 1, then play the animation once.

	private int _step = 1;
	private int _currentFrame = -1;
	private int _timesPlayed = 0;

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

	public float durationPerFrame {
		get {
			return duration / numberOfFrames;
		}
	}

	public bool limitNumberOfPlays {
		get {
			return numberOfPlays > 0;
		}
	}

	// A frame index of -1 means we're done playing this frameset.
	public int nextFrame {
		get {
			int _nextFrame = _currentFrame + _step;

			if (pingPong) {
				// If we're ping-ponging, the animation should go from the
				// start frame to the end frame, and back down again.
				// And if we're looping, keep the ping-pong going.
				if (_nextFrame < startFrame) {
					_timesPlayed++;
				}

				if (_nextFrame > endFrame || (_nextFrame < startFrame && looping) || (_nextFrame < startFrame && numberOfPlays > 0 && _timesPlayed < numberOfPlays)) {
					_step *= -1;
					_currentFrame += _step;
				} else if (_nextFrame < startFrame) {
					_currentFrame = -1;
				} else {
					_currentFrame = _nextFrame;
				}
			} else {
				if (_nextFrame > endFrame) {
					_timesPlayed++;

					if (looping || (numberOfPlays > 0 && _timesPlayed < numberOfPlays)) {
						_currentFrame = startFrame;
					} else {
						_currentFrame = -1;
					}
				} else {
					_currentFrame = _nextFrame;
				}
			}

			return _currentFrame;
		}
	}

	// Reset the frameset to be played from the beginning.
	public void Reset ()
	{
		_step = 1;
		_timesPlayed = 0;
		_currentFrame = startFrame;
	}

	public void UpdateLooping (bool newVal)
	{
		looping = newVal;
		if (looping) {
			// If we're looping, the number of plays is infinite.
			numberOfPlays = -1;
		}
	}

	public void UpdateNumberOfPlays (int newVal)
	{
		numberOfPlays = newVal;
		if (numberOfPlays > 0) {
			// If we set the number of plays, we're not looping.
			looping = false;
		}
	}
}
