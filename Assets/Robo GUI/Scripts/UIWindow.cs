using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindow {

	public enum Anchor
	{
		TopLeft,
		TopMiddle,
		TopRight,
		Left,
		Middle,
		Right,
		BottomLeft,
		BottomMiddle,
		BottomRight
	}

	public delegate void OnWindowToggle(bool toggle);
	public delegate void OnWindowOpen();
	public delegate void OnWindowClose();

	// STATIC
	private static Dictionary<int, UIWindow> mWindows = new Dictionary<int, UIWindow>();

	// Store the currently drawing window, the variable is nulled once done drawing
	private static UIWindow mCurrent;
	public static UIWindow Current { get { return mCurrent; } set {} }

	// This contains the window id of the lastly hovered window
	private static int MouseOverWindowId = -1;

	// We need an object to call coroutines from
	private static GameObject mCoroutines;

	public static UIWindow Create(Rect rect, int WindowId, GUI.WindowFunction callback)
	{
		// Initialize an object
		return new UIWindow(rect, WindowId, callback);
	}

	public static UIWindow Get(int WindowId)
	{
		if (mWindows.ContainsKey(WindowId))
			return mWindows[WindowId];

		return null;
	}

	public static void OnGUI()
	{
		// Call the draw for all the windows
		foreach (KeyValuePair<int, UIWindow> window in mWindows)
		{
			// If we have the auto draw set to true
			if (window.Value.AutoDraw)
				window.Value.Draw();
		}

		// After all the windows have been drawn
		// set which one of them has the mouse over
		// this is important to be done after all the windows have been drawn
		foreach (KeyValuePair<int, UIWindow> window in mWindows)
			window.Value.mIsMouseOver = (window.Value.WindowId == MouseOverWindowId);
	}

	private static void OnShowWindow(int WindowId)
	{
		UIWindow window = mWindows[WindowId];

		// Check for delegates
		if (window.OnOpen != null)
			window.OnOpen();
	}

	private static void OnHideWindow(int WindowId)
	{
		UIWindow window = mWindows[WindowId];

		// Check for delegates
		if (window.OnClose != null)
			window.OnClose();
	}

	public static Vector2 GetAnchorPosition(Anchor anch)
	{
		Vector2 position = Vector2.zero;

		// Prepare the position for this anchor
		switch (anch)
		{
			case Anchor.TopLeft: 		position = Vector2.zero; 											break;
			case Anchor.TopMiddle: 		position = new Vector2(Screen.width / 2.0f, 0.0f); 					break;
			case Anchor.TopRight: 		position = new Vector2(Screen.width, 0.0f); 						break;
			case Anchor.Left: 			position = new Vector2(0.0f, Screen.height / 2.0f); 				break;
			case Anchor.Middle: 		position = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f); 	break;
			case Anchor.Right: 			position = new Vector2(Screen.width, Screen.height / 2.0f); 		break;
			case Anchor.BottomLeft: 	position = new Vector2(0.0f, Screen.height); 						break;
			case Anchor.BottomMiddle: 	position = new Vector2(Screen.width / 2.0f, Screen.height); 		break;
			case Anchor.BottomRight: 	position = new Vector2(Screen.width, Screen.height); 				break;
		}

		return position;
	}

	public static UIWindowCoroutines GetCoroutinesObject()
	{
		if (mCoroutines == null)
		{
			mCoroutines = new GameObject("_UIWindowCoroutines");
			mCoroutines.AddComponent<UIWindowCoroutines>();
		}
		
		return mCoroutines.GetComponent("UIWindowCoroutines") as UIWindowCoroutines;
	}

	public static void DrawTitle(Rect rect, string text)
	{
		// Convert the text to upper case
		text = text.ToUpper();

		// Get the title text size
		Vector2 titleSize = GUI.skin.GetStyle("WindowTitle").CalcSize(new GUIContent(text));

		// Draw the title background
		GUI.DrawTexture(new Rect((rect.x + ((rect.width - (titleSize.x + 26.0f)) / 2)), 58.0f, (titleSize.x + 26.0f), 38.0f), GUI.skin.GetStyle("WindowTitleBackground").normal.background);
		
		// Draw the title text
		UIElements.TextWithShadow(
			new Rect((rect.x + ((rect.width - titleSize.x) / 2)), 66.0f, titleSize.x, titleSize.y),
			text,
			GUI.skin.GetStyle("WindowTitle"),
			GUI.skin.GetStyle("WindowTitle").normal.textColor,
			new Color(0.0f, 0.0f, 0.0f, 0.5f),
			new Vector2(0.0f, 2.0f)
		);

		// Draw the title overlay
		GUI.DrawTexture(new Rect((rect.x + ((rect.width - (titleSize.x + 14.0f)) / 2)), 65.0f, (titleSize.x + 14.0f), 21.0f), GUI.skin.GetStyle("WindowTitleOverlay").normal.background);
	}

	public static void DrawBackground(Rect rect, bool withHeader)
	{
		string StylePrefix = (withHeader ? "WindowBackground" : "WindowBackground2");

		Texture2D topleft = GUI.skin.GetStyle(StylePrefix + "BodyTopLeft").normal.background;
		Texture2D topright = GUI.skin.GetStyle(StylePrefix + "BodyTopRight").normal.background;
		Texture2D bottomleft = GUI.skin.GetStyle(StylePrefix + "BodyBottomLeft").normal.background;
		Texture2D bottomright = GUI.skin.GetStyle(StylePrefix + "BodyBottomRight").normal.background;
		Texture2D horizontalrepeat = GUI.skin.GetStyle(StylePrefix + "HorizontalRepeat").normal.background;
		Texture2D verticalrepeat = GUI.skin.GetStyle(StylePrefix + "VerticalRepeat").normal.background;
		Texture2D middlerepeat = GUI.skin.GetStyle("WindowBackgroundMiddleRepeat").normal.background;
		Texture2D headerleft = GUI.skin.GetStyle("WindowBackgroundHeaderLeft").normal.background;
		Texture2D headerright = GUI.skin.GetStyle("WindowBackgroundHeaderRight").normal.background;
		Texture2D headerrepeat = GUI.skin.GetStyle("WindowBackgroundHeaderRepeat").normal.background;

		float topheight = topleft.height;
		float bottomheight = bottomleft.height;

		// Begin a group to contain the background
		GUI.BeginGroup(rect);

		// Draw the top left body background
		GUI.DrawTexture(new Rect(0, 0, topleft.width, topheight), topleft);

		// Draw the top right body background
		GUI.DrawTexture(new Rect((rect.width - topright.width), 0, topright.width, topheight), topright);

		// Draw the bottom left body background
		GUI.DrawTexture(new Rect(0, (rect.height - bottomheight), bottomleft.width, bottomheight), bottomleft);

		// Draw the buttom right body background
		GUI.DrawTexture(new Rect((rect.width - bottomright.width), (rect.height - bottomheight), bottomright.width, bottomheight), bottomright);

		// Draw the top horizontal repeat
		RepeatHorizontal(new Rect(topleft.width, (withHeader ? 16.0f : 6.0f), (rect.width - topleft.width - topright.width), (topheight - (withHeader ? 16.0f : 6.0f))), horizontalrepeat, Vector2.zero);

		// Draw the bottom horizontal repeat
		RepeatHorizontal(new Rect(bottomleft.width, (rect.height - bottomheight), (rect.width - bottomleft.width - bottomright.width), bottomheight), horizontalrepeat, new Vector2(0.0f, (0.0f - (horizontalrepeat.height - (bottomheight - 6.0f)))));

		// Draw the left vertical repeat
		RepeatVertical(new Rect(21.0f, topheight, (topleft.width - 21.0f), (rect.height - topheight - bottomheight)), verticalrepeat, Vector2.zero); 

		// Draw the right vertical repeat
		RepeatVertical(new Rect((rect.width - topright.width), topheight, (topright.width - 21.0f), (rect.height - topheight - bottomheight)), verticalrepeat, new Vector2((0.0f - (verticalrepeat.width - (topright.width - 21.0f))), 0.0f)); 

		// Draw the middle repeat
		GUI.DrawTexture(new Rect(topleft.width, topheight, (rect.width - topleft.width - topright.width), (rect.height - topleft.height - bottomleft.height)), middlerepeat);

		// Draw the header if we have to
		if (withHeader)
		{
			// Draw the left header background
			GUI.DrawTexture(new Rect(24.0f, 24.0f, headerleft.width, headerleft.height), headerleft);

			// Draw the right header background
			GUI.DrawTexture(new Rect((rect.width - headerright.width - 23.0f), 24.0f, headerright.width, headerright.height), headerright);

			// Draw the header repeat
			RepeatHorizontal(new Rect((23.0f + headerleft.width), 41.0f, (rect.width - (23.0f + headerleft.width) - (headerright.width + 22.0f)), headerrepeat.height), headerrepeat, Vector2.zero);
		}

		// End the group
		GUI.EndGroup();
	}

	private static void RepeatHorizontal(Rect r, Texture2D tex, Vector2 texOffset)
	{
		// begin a group
		GUI.BeginGroup(r);

		// Determine how many times we need to draw the texture
		int DrawTimes = (int) Mathf.Floor(r.width / tex.width) + 1;

		for (int i = 0; i < DrawTimes; i++)
			GUI.DrawTexture(new Rect((i * tex.width), texOffset.y, tex.width, tex.height), tex);

		// end the group
		GUI.EndGroup();
	}

	private static void RepeatVertical(Rect r, Texture2D tex, Vector2 texOffset)
	{
		// begin a group
		GUI.BeginGroup(r);
		
		// Determine how many times we need to draw the texture
		int DrawTimes = (int) Mathf.Floor(r.height / tex.height) + 1;
		
		for (int i = 0; i < DrawTimes; i++)
			GUI.DrawTexture(new Rect(texOffset.x, (i * tex.height), tex.width, tex.height), tex);
		
		// end the group
		GUI.EndGroup();
	}

	// OOP
	private bool mShowWindow = false;
	private bool mShowWindowInternal = false;

	// This delegates will be called from the static methods OnShowWindow / OnHideWindow
	private OnWindowOpen mOpenCallback;
	public OnWindowOpen OnOpen { get { return this.mOpenCallback; } set { this.mOpenCallback = value; } }
	private OnWindowClose mCloseCallback;
	public OnWindowClose OnClose { get { return this.mCloseCallback; } set { this.mCloseCallback = value; } }

	public bool ShowWindow
	{
		get { return this.mShowWindow; }
		set {
			if (this.UseFadeAnimation)
			{
				FadeMethods method = (value ? FadeMethods.In : FadeMethods.Out);

				if (this.animationCurrentMethod != method && this.mFadeCoroutine != null)
					this.mFadeCoroutine.Stop();

				// Start the new animation
				if (this.animationCurrentMethod != method)
					this.mFadeCoroutine = new UICoroutine(GetCoroutinesObject(), this.FadeAnimation(method));
			}

			// Important to be set after the coroutine has been started
			this.mShowWindow = value;
		}
	}
	public bool IsShown() { return this.mShowWindow; }
	public void Show() { this.Toggle(true); }
	public void Hide() { this.Toggle(false); }

	// This is called internally
	private void Toggle(bool value)
	{
		this.ShowWindow = value;
	}

	private float WindowAlpha = 0.0f;

	private bool mAutoDraw = true;
	public bool AutoDraw
	{
		get { return this.mAutoDraw; }
		set { this.mAutoDraw = value; }
	}

	// This is the rect used for the current draw
	private Rect mCurrentRect;

	// This is the original window rect
	private Rect mWindowRect;
	public Rect WindowRect
	{
		get { return this.mWindowRect; }
		set { this.mWindowRect = value; }
	}

	public void SetPosition(Vector2 pos)
	{
		this.mWindowRect.x = pos.x;
		this.mWindowRect.y = pos.y;
	}
	public void SetSize(Vector2 size)
	{
		this.mWindowRect.width = size.x;
		this.mWindowRect.height = size.y;
	}

	private bool mAnchored = false;
	private UIWindow.Anchor mAnchor;
	public void SetAnchor(UIWindow.Anchor anch)
	{
		this.mAnchor = anch;
		this.mAnchored = true;
	}

	private int WindowId;
	private GUI.WindowFunction callback;

	private string mTitle = "";
	public string Title {
		get { return this.mTitle; }
		set {
			this.mTitle = value;
			this.mWithHeader = (!string.IsNullOrEmpty(value));
		}
	}

	private bool mWithHeader = false;

	private bool mWithBackground = true;
	public bool Background { get { return this.mWithBackground; } set { this.mWithBackground = value; } }

	private bool mDraggable = false;
	public bool Draggable { get { return this.mDraggable; } set { this.mDraggable = value; } }

	public bool mIsMouseOver = false;
	public bool IsMouseOver() { return mIsMouseOver; }

	public UIWindow(Rect rect, int WindowId, GUI.WindowFunction callback)
	{
		this.mWindowRect = rect;
		this.WindowId = WindowId;
		this.callback = callback;

		// Save it
		mWindows.Add(WindowId, this);
	}

	private void PreDrawCalculations()
	{
		// Apply original rect
		this.mCurrentRect = this.mWindowRect;

		// Check for anchoring and apply anchor position
		if (this.mAnchored)
		{
			this.mCurrentRect.x = this.mCurrentRect.x + GetAnchorPosition(this.mAnchor).x;
			this.mCurrentRect.y = this.mCurrentRect.y + GetAnchorPosition(this.mAnchor).y;
		}
	}

	private void ExtractPositionChanges()
	{
		// Check if we had anchoring
		if (this.mAnchored)
		{
			this.mCurrentRect.x = this.mCurrentRect.x - GetAnchorPosition(this.mAnchor).x;
			this.mCurrentRect.y = this.mCurrentRect.y - GetAnchorPosition(this.mAnchor).y;
		}

		// Apply the extracted position
		this.mWindowRect.x = this.mCurrentRect.x;
		this.mWindowRect.y = this.mCurrentRect.y;
	}

	private float mOriginalAlpha = 1.0f;
	private void ApplyAlpha(float newAlpha)
	{
		// Check if we need to change the alpha at all
		if (GUI.color.a == newAlpha)
			return;
		
		// Keep the original alpha so we can restore it later
		this.mOriginalAlpha = GUI.color.a;
		
		// Set the alpha
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, newAlpha);
	}
	
	private void RestoreAlpha()
	{
		// Check if we need restore it
		if (GUI.color.a == this.mOriginalAlpha)
			return;
		
		// Restore the alpha
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.mOriginalAlpha);
	}

	public void Draw()
	{
		// Make some changes to the current rect
		this.PreDrawCalculations();

		// Apply alpha outside the window
		this.ApplyAlpha(this.WindowAlpha);

		// Get the window style
		GUIStyle style = new GUIStyle(GUI.skin.GetStyle("window"));

		// Check if we dont wanna have background
		if (!this.mWithBackground)
		{
			style.padding = new RectOffset(0, 0, 0, 0);
			style.normal.background = null;
		}
		else
		// Check if we have a header
		if (!this.mWithHeader)
		{
			style.padding.top = 74;
		}

		bool display = (this.animationCurrentMethod != FadeMethods.None) ? this.mShowWindowInternal : this.mShowWindow;
		
		// Draw the window
		if (display)
			this.mCurrentRect = GUI.Window(this.WindowId, this.mCurrentRect, this.OnCallback, "", style);

		// Restore the original alpha
		this.RestoreAlpha();

		// After the window has been drawn
		// extract any position changes
		// made to the current rect
		this.ExtractPositionChanges();
	}

	private Rect GetBackgroundRect()
	{
		return new Rect(0, 0, this.mCurrentRect.width, this.mCurrentRect.height);
	}

	public void OnCallback(int WindowId)
	{
		// Set this window as the current one
		mCurrent = this;

		// Set the window alpha
		if (GUI.color.a != this.WindowAlpha)
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.WindowAlpha);

		// Draw the background
		if (this.mWithBackground)
			DrawBackground(this.GetBackgroundRect(), this.mWithHeader);

		// Draw the title
		if (this.mWithHeader)
			DrawTitle(this.GetBackgroundRect(), this.mTitle);

		// Call the window draw callback
		this.callback(this.WindowId);

		// Check for draggable
		if (this.mDraggable)
		{
			if (this.mWithHeader)
				GUI.DragWindow(new Rect(42.0f, 32.0f, (this.GetBackgroundRect().width - 84.0f), 81.0f));
			else
				GUI.DragWindow(new Rect(0, 0, this.GetBackgroundRect().width, this.GetBackgroundRect().height));
		}

		// Detect when the mouse is over this window
		DetectMouseOverWindow();

		// Null the current window variable
		mCurrent = null;
	}

	private void DetectMouseOverWindow()
	{
		GUI.Box(new Rect(0, 0, this.mCurrentRect.width, this.mCurrentRect.height), new GUIContent("", "mouseOverWindow:" + this.WindowId), new GUIStyle());
		
		if (Event.current.type == EventType.Repaint && GUI.tooltip == "mouseOverWindow:" + this.WindowId)
			MouseOverWindowId = this.WindowId;
	}

	/*
	 * Show / Hide Fade Animation
	 */

	private enum FadeMethods
	{
		None,
		In,
		Out
	}

	private bool mUseFadeAnimation = true;
	public bool UseFadeAnimation {
		get { return this.mUseFadeAnimation; }
		set {
			if (!value)
				this.WindowAlpha = 1.0f;
			else
			{
				if (this.mShowWindow)
					this.WindowAlpha = 1.0f;
				else
					this.WindowAlpha = 0.0f;
			}
			
			this.mUseFadeAnimation = value;
		}
	}

	private FadeMethods animationCurrentMethod = FadeMethods.None;
	private UICoroutine mFadeCoroutine;
	private float FadeDuration = 0.5f;
	public void SetFadeDuration(float v) { this.FadeDuration = v; }

	// Show / Hide fade animation coroutine
	private IEnumerator FadeAnimation(FadeMethods method)
	{
		// Check if we are trying to fade in and the window is already shown
		if (method == FadeMethods.In && this.IsShown())
			yield break;
		else if (method == FadeMethods.Out && !this.IsShown())
			yield break;

		// Define that animation is in progress
		this.animationCurrentMethod = method;

		// Get the timestamp
		float startTime = Time.time;
		
		// Determine Fade in or Fade out
		if (method == FadeMethods.In)
		{
			// Show the window
			this.mShowWindowInternal = true;
			
			// Call the on show
			OnShowWindow(this.WindowId);

			// Calculate the time we need to fade in from the current alpha
			float internalDuration = (this.FadeDuration - (this.FadeDuration * this.WindowAlpha));

			// Update the start time
			startTime -= (this.FadeDuration - internalDuration);

			// Fade In
			while (Time.time < (startTime + internalDuration))
			{
				float RemainingTime = (startTime + this.FadeDuration) - Time.time;
				float ElapsedTime = this.FadeDuration - RemainingTime;
				
				// Update the alpha by the percentage of the time elapsed
				this.WindowAlpha = ElapsedTime / this.FadeDuration;
				
				yield return 0;
			}

			// Make sure it's 1
			this.WindowAlpha = 1.0f;
		}
		else if (method == FadeMethods.Out)
		{
			// Calculate the time we need to fade in from the current alpha
			float internalDuration = (this.FadeDuration * this.WindowAlpha);
			
			// Update the start time
			startTime -= (this.FadeDuration - internalDuration);

			// Fade Out
			while (Time.time < (startTime + internalDuration))
			{
				float RemainingTime = (startTime + this.FadeDuration) - Time.time;
				
				// Update the alpha by the percentage of the remaing time
				this.WindowAlpha = RemainingTime / this.FadeDuration;
				
				yield return 0;
			}
			
			// Make sure it's 0
			this.WindowAlpha = 0.0f;
			
			// Hide the window
			this.mShowWindowInternal = false;
			
			// Call the on hide
			OnHideWindow(this.WindowId);
		}

		// No longer animating
		this.animationCurrentMethod = FadeMethods.None;
	}
}