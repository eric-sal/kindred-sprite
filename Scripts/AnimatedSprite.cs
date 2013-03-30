using UnityEngine;
using System.Collections;

public class AnimatedSprite : Sprite
{
	public SpriteAnimation spriteAnimation;
	public bool playOnStart = false;
	public bool destroyAfterFinished = false;

	private AnimationFrameset _currentFrameset;
	private int _currentFrame = -1;
	private bool _isPlaying = false;
	private float _frameTime = 0;

	public override void Awake ()
	{
		base.Awake ();
	}

	public override void Start ()
	{
		if (playOnStart) {
			Play ();
		}

		base.Start ();
	}

	public override void Update ()
	{
		if (_currentFrameset != null && _isPlaying) {
			if (_frameTime >= _currentFrameset.durationPerFrame) {
				base.ShowFrame (_currentFrame);
				_currentFrame = _currentFrameset.nextFrame;

				_isPlaying = _currentFrame >= 0;
				_frameTime = 0;

				if (!_isPlaying && destroyAfterFinished) {
					Destroy (gameObject);
				}
			} else {
				_frameTime += Time.deltaTime;
			}
		}

		base.Update ();
	}

	public void Play ()
	{
		Play (spriteAnimation.framesets [0]);
	}

	public void Play (string framesetName)
	{
		Play (spriteAnimation.GetFrameset (framesetName));
	}

	public void Play (AnimationFrameset frameset)
	{
		_currentFrameset = frameset;
		if (_currentFrameset != null) {
			_currentFrameset.Reset ();

			if (_currentFrameset.startOnRandomFrame) {
				_currentFrame = Random.Range (_currentFrameset.startFrame, _currentFrameset.endFrame);
			} else {
				_currentFrame = _currentFrameset.startFrame;
			}

			_frameTime = 0;
			_isPlaying = true;
		}
	}

	public void Stop ()
	{
		_isPlaying = false;
	}

	public override void ShowFrame (int index)
	{
		Stop ();
		base.ShowFrame (index);
	}
}
