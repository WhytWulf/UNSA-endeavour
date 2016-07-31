using UnityEngine;
using System.Collections;

public class UIAnimation {

	private int TileX = 0;
	private int TileY = 0;

	public float FPS = 30.0f;

	private Texture2D texture;
	private Vector2 TexOffset = Vector2.zero;
	private Vector2 TexSize = Vector2.zero;

	public float Percentage = 0.0f;

	private float PUT = 0.100f;
	public float PercentageUpdatePeriod { get { return this.PUT; } set { this.PUT = value; } }
	private float NPUT = 0.0f;

	public UIAnimation(Texture2D tex, int TilesX, int TilesY)
	{
		this.texture = tex;
		this.TileX = TilesX;
		this.TileY = TilesY;
	}

	public void Update()
	{
		// Calculate index
		int index = Mathf.RoundToInt(Time.time * this.FPS);

		// repeat when exhausting all frames
		index = index % (this.TileX * this.TileY);
		
		// Size of every tile
		this.TexSize = new Vector2((1.0f / this.TileX), (1.0f / this.TileY));
		
		// split into horizontal and vertical index
		int uIndex = index % this.TileX;
		int vIndex = index / this.TileX;
		
		// build offset
		// v coordinate is the bottom of the image in opengl so we need to invert.
		this.TexOffset = new Vector2((uIndex * this.TexSize.x), (1.0f - this.TexSize.y - vIndex * this.TexSize.y));
		
		// Calculate the percentage
		int totalFrames = (this.TileX * this.TileY);
		float percentage;
		percentage = (1.0f * index) / (1.0f * totalFrames);
		percentage = percentage * 100;
		
		// Set the percentage for usage outside of this functions scope
		if (Time.time >= this.NPUT)
		{
			this.Percentage = Mathf.Round(percentage);
			this.NPUT += this.PUT;
		}
	}

	public Rect Draw()
	{
		// Check if we have a texture
		if (!this.texture)
			return new Rect(0,0,0,0);
		
		// Draw the texture
		Rect texCoords = new Rect(this.TexOffset.x, this.TexOffset.y, this.TexSize.x, this.TexSize.y);
		bool alpha = true;

		GUIStyle style = new GUIStyle();
		style.stretchWidth = false;
		style.stretchHeight = false;

		// Use the space for the animation
		Rect rect = GUILayoutUtility.GetRect((this.texture.width / this.TileX), (this.texture.height / this.TileY), style);

		GUI.DrawTextureWithTexCoords(rect, this.texture, texCoords, alpha);

		return rect;
	}

	public void Draw(Rect rect)
	{
		// Check if we have a texture
		if (!this.texture)
			return;

		// Draw the texture
		Rect texCoords = new Rect(this.TexOffset.x, this.TexOffset.y, this.TexSize.x, this.TexSize.y);
		bool alpha = true;
		
		GUI.DrawTextureWithTexCoords(rect, this.texture, texCoords, alpha);
	}
}
