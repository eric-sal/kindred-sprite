using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using XmlExtensions;

// We want to see the changes to the SpriteContainer while developing
[ExecuteInEditMode]

// Import sprite sheet and atlas data
public class SpriteContainer : MonoBehaviour
{
	public Texture texture;
	public AtlasData[] atlasData = null;
	public int borderPadding = 0; 				// padding around edges of sprite sheet image in pixels
	public TextAsset atlasDataFile = null;
	private TextAsset _atlasDataFile = null;
	public bool reloadData = false;

	private SpriteData[] _spriteData;
	private int _atlasDataFileSize = 0;
	private Material _material;
	private MeshRenderer _renderer;

	private XmlNode _subTexture = null;

	public SpriteData[] spriteData {
		get {
			return _spriteData;
		}
	}

	public Material material {
		get {
			if (_material == null) {
				_material = new Material (Shader.Find ("Sprite"));
			}

			if (texture != null) {
				_material.mainTexture = texture;
			}

			return _material;
		}
	}

	public void Awake ()
	{
		if (!Application.isPlaying) {
			if (atlasDataFile != null) {
				ImportAtlasData ();

				_atlasDataFileSize = atlasDataFile.bytes.Length;
			}

			_atlasDataFile = atlasDataFile;
		}
	}

	public void Update ()
	{
		if (!Application.isPlaying) {
			if (atlasDataFile == null || texture == null) {
				Reset ();
			}

			// Only reload the data file if it's changed, or we're forcing a reload.
			// We'll assume the data file has changed if the file is different from the cached file,
			// or if the filesize of the atlas data file has changed.
			if (reloadData || _atlasDataFile != atlasDataFile || (atlasDataFile != null && _atlasDataFileSize != atlasDataFile.bytes.Length)) {
				if (atlasDataFile != null) {
					ImportAtlasData ();
					reloadData = false;

					_atlasDataFileSize = atlasDataFile.bytes.Length;
				}

				_atlasDataFile = atlasDataFile;
			}
		}
	}

	private void Reset ()
	{
		if (atlasDataFile == null) {
			atlasData = null;
			_atlasDataFile = null;
			_atlasDataFileSize = 0;
		}
	}

	// Read and parse atlas data from XML file
	private void ImportAtlasData ()
	{
		XmlDocument xml = new XmlDocument ();
		xml.LoadXml (atlasDataFile.text);
		XmlNode frames = xml.DocumentElement.SelectSingleNode ("dict/key");
		List<AtlasData> data = new List<AtlasData> ();

		if (frames != null && frames.InnerText == "frames") {
			XmlNodeList subTextureNames = xml.DocumentElement.SelectNodes ("dict/dict/key");
			XmlNodeList subTextures = xml.DocumentElement.SelectNodes ("dict/dict/dict");
			try {
				for (int si = 0; si < subTextures.Count; si++) {
					_subTexture = subTextures [si];
					AtlasData ad = new AtlasData ();

					bool rotated = _subTexture.GetBool ("rotated");
					Rect frame = _subTexture.GetRect ("frame");
					Rect colorRect = _subTexture.GetRect ("sourceColorRect");
					Vector2 sourceSize = _subTexture.GetVector2 ("sourceSize");

					try {
						ad.name = subTextureNames [si].InnerText.Split ('.') [0];
					} catch (System.Exception) {
						ad.name = subTextureNames [si].InnerText;
					}
					ad.position = new Vector2 (frame.xMin, frame.yMin);
					ad.rotated = rotated;
					ad.size = new Vector2 (colorRect.width, colorRect.height);
					ad.frameSize = sourceSize;
					ad.offset = new Vector2 (colorRect.xMin, colorRect.yMin);

					data.Add (ad);
				}
			} catch (System.Exception ERR) {
				Debug.LogError ("Atlas Import error!");
				Debug.LogError (ERR.Message);
			}
		}

		atlasData = data.ToArray ();
		BuildSpriteData ();
	}

	// Build sprite data from atlas data
	private void BuildSpriteData ()
	{
		_spriteData = new SpriteData[atlasData.Length];

		for (int i = 0; i < atlasData.Length; i++) {
			spriteData [i] = new SpriteData ();
			spriteData [i].index = i;
			spriteData [i].name = atlasData [i].name;
			spriteData [i].size = atlasData [i].size;
			spriteData [i].sheetPixelCoords = atlasData [i].position;
			spriteData [i].texture = texture;
			spriteData [i].UpdateVertices ();
			spriteData [i].UpdateUVs ();
		}
	}
}
