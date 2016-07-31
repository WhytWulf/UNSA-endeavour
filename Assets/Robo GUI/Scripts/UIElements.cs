using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIElements {
	
	public enum Align
	{
		Left,
		Right,
		Center
	}

	public enum TextStyle
	{
		One,
		Two
	}

	public static void Text(Rect rect, string text)
	{
		Text(rect, text, TextStyle.One);
	}

	public static void Text(Rect rect, string text, TextStyle textStyle)
	{
		GUILayout.BeginArea(rect);
		Text(text, textStyle);
		GUILayout.EndArea();
	}

	public static void Text(string text)
	{
		Text(text, TextStyle.One);
	}

	public static void Text(string text, TextStyle textStyle)
	{
		string styleStr = "TextStyle1";

		switch (textStyle)
		{
			case TextStyle.One: styleStr = "TextStyle1"; break;
			case TextStyle.Two: styleStr = "TextStyle2"; break;
		}

		GUIStyle style = GUI.skin.GetStyle(styleStr);
		GUIStyle shadow = new GUIStyle(style);

		// Add the shadow offset to the content offset of the style
		shadow.contentOffset = new Vector2(1.0f, 2.0f);

		// Add the shadow color
		shadow.normal.textColor = new Color(0.0f, 0.0f, 0.0f, 0.35f);

		// Draw the text shadow
		GUILayout.Label(text, shadow);

		// Get the rect where the shadow was drawn
		Rect rect = GUILayoutUtility.GetLastRect();

		// Draw the text
		GUI.Label(rect, text, style);
	}

	public static void ImageBox(string textureResource)
	{
		Texture2D tex = Resources.Load(textureResource) as Texture2D;
		
		if (tex)
			ImageBox(tex);
	}
	
	public static void ImageBox(Texture2D imageTexture)
	{
		GUIStyle background = GUI.skin.GetStyle("ImageBoxBackground");
		GUIStyle overlay = GUI.skin.GetStyle("ImageBoxOverlay");

		// Draw the image with the background
		GUILayout.Box(imageTexture, background);

		// Get the last rect so we can draw the frame
		Rect rect = GUILayoutUtility.GetLastRect();

		// Draw the frame & overlay
		GUI.Box(new Rect((rect.x + background.padding.left), (rect.y + background.padding.top), (rect.width - (background.padding.left + background.padding.right)), (rect.height - (background.padding.top + background.padding.bottom))), new GUIContent(""), overlay);
	}

	private static string StripColorTag(string source)
	{
		return System.Text.RegularExpressions.Regex.Replace(source, "<(color=#(.*?)|/color)>", string.Empty);
	}

	public static void TextWithShadow(Rect rect, string text, GUIStyle style, Color txtColor, Color shadowColor)
	{
		TextWithShadow(rect, new GUIContent(text), style, txtColor, shadowColor, new Vector2(1.0f, 1.0f));
	}
	
	public static void TextWithShadow(Rect rect, string text, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
	{
		TextWithShadow(rect, new GUIContent(text), style, txtColor, shadowColor, direction);
	}

	public static void TextWithShadow(Rect rect, GUIContent content, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
	{
		GUIStyle newStyle = new GUIStyle(style);
		
		newStyle.normal.textColor = shadowColor;
		rect.x += direction.x;
		rect.y += direction.y;
		GUI.Label(rect, new GUIContent(StripColorTag(content.text)), newStyle);
		
		newStyle.normal.textColor = txtColor;
		rect.x -= direction.x;
		rect.y -= direction.y;
		GUI.Label(rect, content, newStyle);
	}

	public static void TextWithShadow(string content, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
	{
		TextWithShadow(new GUIContent(content), style, txtColor, shadowColor, direction);
	}

	public static void TextWithShadow(GUIContent content, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
	{
		GUIStyle newStyle = new GUIStyle(style);
		
		// Get the rect where the text should be placed
		Rect rect = GUILayoutUtility.GetRect(content, style);
		
		newStyle.normal.textColor = shadowColor;
		GUI.Label(new Rect((direction.x + rect.x), (direction.y + rect.y), rect.width, rect.height), new GUIContent(StripColorTag(content.text)), newStyle);
		
		newStyle.normal.textColor = txtColor;
		GUI.Label(rect, content, newStyle);
	}

	public static void Separator(Vector2 position)
	{
		GUILayout.BeginArea(new Rect(position.x, position.y, GUI.skin.GetStyle("Separator").fixedWidth, GUI.skin.GetStyle("Separator").fixedHeight));
		Separator();
		GUILayout.EndArea();
	}

	public static void Separator(Align align)
	{
		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		// Draw the sperator
		Separator();
		
		if (align == Align.Center)
			GUILayout.FlexibleSpace();

		if (align != Align.Left)
			GUILayout.EndHorizontal();
	}

	public static void Separator()
	{
		GUILayout.Box("", GUI.skin.GetStyle("Separator"));
	}

	public static bool Button(Rect rect, string text)
	{
		GUILayout.BeginArea(rect);
		bool result = Button(text, Align.Left);
		GUILayout.EndArea();

		return result;
	}

	public static bool Button(string text)
	{
		return Button(text, Align.Left);
	}

	public static bool Button(string text, Align align)
	{
		GUIStyle btnStyle = GUI.skin.GetStyle("button");
		GUIStyle textStyle = GUI.skin.GetStyle("buttonText");
		Texture2D textOverlay = GUI.skin.GetStyle("buttonTextOverlay").normal.background;

		// Upper case text
		text = text.ToUpper();

		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();

		// Get the button rect
		Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), btnStyle);

		// Get interaction for that button
		RectInteraction interaction = RectInteraction.Get(new Rect((rect.x + 16.0f), (rect.y + 15.0f), (rect.width - 32.0f), (rect.height - 15.0f - 18.0f)));

		// Draw the button background
		if (Event.current.type == EventType.Repaint)
			btnStyle.Draw(rect, text, interaction.IsHovered, interaction.IsPressed, false, false);

		// Draw the text
		if (!string.IsNullOrEmpty(text))
		{
			TextWithShadow(
				rect,
				text,
				textStyle,
				(interaction.IsPressed ? textStyle.active.textColor : (interaction.IsHovered ? textStyle.hover.textColor : textStyle.normal.textColor)),
				(interaction.IsPressed ? textStyle.onActive.textColor : (interaction.IsHovered ? textStyle.onHover.textColor : textStyle.onNormal.textColor)),
				new Vector2(1.0f, 2.0f)
			);
		}

		// Draw the text overlay
		GUI.DrawTexture(new Rect((rect.x + 45.0f), (rect.y + 31.0f), (rect.width - 90.0f), (rect.height - 62.0f)), textOverlay);

		if (align == Align.Center)
			GUILayout.FlexibleSpace();

		if (align != Align.Left)
			GUILayout.EndHorizontal();

		return interaction.Click;
	}

	public enum TinyButtons
	{
		Accept,
		Decline,
		Social,
		Mail
	}

	public static bool TinyButton(TinyButtons btn)
	{
		return TinyButton(btn, Align.Left);
	}

	public static bool TinyButton(TinyButtons btn, Align align)
	{
		GUIStyle btnStyle = GUI.skin.GetStyle("tinyButton");
		GUIStyle texStyle = new GUIStyle();

		// Get the texture based on the button type
		switch (btn)
		{
			case TinyButtons.Accept: texStyle = GUI.skin.GetStyle("tinyButtonAccept"); break;
			case TinyButtons.Decline: texStyle = GUI.skin.GetStyle("tinyButtonDecline"); break;
			case TinyButtons.Social: texStyle = GUI.skin.GetStyle("tinyButtonSocial"); break;
			case TinyButtons.Mail: texStyle = GUI.skin.GetStyle("tinyButtonMail"); break;
		}

		// Get the button texture
		Texture2D tex = texStyle.normal.background;

		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		// Get the button rect
		Rect rect = GUILayoutUtility.GetRect(new GUIContent(tex), btnStyle);
		
		// Get interaction for that button
		RectInteraction interaction = RectInteraction.Get(new Rect((rect.x + 4.0f), (rect.y + 4.0f), (rect.width - 8.0f), (rect.height - 8.0f)));
		
		// Draw the button background
		if (Event.current.type == EventType.Repaint)
			btnStyle.Draw(rect, new GUIContent(""), interaction.IsHovered, interaction.IsPressed, false, false);

		// Now draw the icon in the middle of the button
		GUI.DrawTexture(new Rect((texStyle.contentOffset.x + (rect.x + ((rect.width - tex.width) / 2))), (texStyle.contentOffset.y + (rect.y + ((rect.height - tex.height) / 2))), tex.width, tex.height), tex);

		if (align == Align.Center)
			GUILayout.FlexibleSpace();

		if (align != Align.Left)
			GUILayout.EndHorizontal();
		
		return interaction.Click;
	}

	public enum IconButtons
	{
		Heart,
		Plus,
		Decline,
		Star
	}

	public static bool IconButton(IconButtons btn)
	{
		return IconButton(btn, Align.Left);
	}

	public static bool IconButton(IconButtons btn, Align align)
	{
		string styleStr = "";

		// Get the button style string
		switch (btn)
		{
			case IconButtons.Star: styleStr = "StarButton"; break;
			case IconButtons.Plus: styleStr = "PlusButton"; break;
			case IconButtons.Decline: styleStr = "DeclineButton"; break;
			case IconButtons.Heart: styleStr = "HeartButton"; break;
		}

		// Convert the string style to GUIStyle
		GUIStyle style = GUI.skin.GetStyle(styleStr);

		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		bool result = GUILayout.Button("", style);
		
		if (align == Align.Center)
			GUILayout.FlexibleSpace();
		
		if (align != Align.Left)
			GUILayout.EndHorizontal();
		
		return result;
	}

	public enum ArrowButtons
	{
		Left,
		Middle,
		Right
	}

	public static bool ArrowButton(ArrowButtons type)
	{
		return ArrowButton(type, Align.Left);
	}

	public static bool ArrowButton(ArrowButtons type, Align align)
	{
		string style = "";

		switch (type)
		{
			case ArrowButtons.Left: style = "arrowButtonLeft"; break;
			case ArrowButtons.Middle: style = "arrowButtonMiddle"; break;
			case ArrowButtons.Right: style = "arrowButtonRight"; break;
		}

		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		bool result = GUILayout.Button("", style);

		if (align == Align.Center)
			GUILayout.FlexibleSpace();
		
		if (align != Align.Left)
			GUILayout.EndHorizontal();
		
		return result;
	}

	public static bool SmallButton(string text)
	{
		return SmallButton(text, Align.Left);
	}
	
	public static bool SmallButton(string text, Align align)
	{
		GUIStyle btnStyle = GUI.skin.GetStyle("smallButton");
		GUIStyle textStyle = GUI.skin.GetStyle("smallButtonText");
		Texture2D textOverlay = GUI.skin.GetStyle("smallButtonTextOverlay").normal.background;
		
		// Upper case text
		text = text.ToUpper();
		
		if (align != Align.Left)
			GUILayout.BeginHorizontal();
		
		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		// Get the button rect
		Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), btnStyle);
		
		// Get interaction for that button
		RectInteraction interaction = RectInteraction.Get(new Rect((rect.x + 4.0f), (rect.y + 4.0f), (rect.width - 8.0f), (rect.height - 8.0f)));
		
		// Draw the button background
		if (Event.current.type == EventType.Repaint)
			btnStyle.Draw(rect, text, interaction.IsHovered, interaction.IsPressed, false, false);
		
		// Draw the text
		if (!string.IsNullOrEmpty(text))
		{
			TextWithShadow(
				rect,
				text,
				textStyle,
				(interaction.IsPressed ? textStyle.active.textColor : (interaction.IsHovered ? textStyle.hover.textColor : textStyle.normal.textColor)),
				(interaction.IsPressed ? textStyle.onActive.textColor : (interaction.IsHovered ? textStyle.onHover.textColor : textStyle.onNormal.textColor)),
				new Vector2(1.0f, 2.0f)
			);
		}
		
		// Draw the text overlay
		GUI.DrawTexture(new Rect((rect.x + 11.0f), (rect.y + 12.0f), (rect.width - 22.0f), (rect.height - 24.0f)), textOverlay);
		
		if (align == Align.Center)
			GUILayout.FlexibleSpace();
		
		if (align != Align.Left)
			GUILayout.EndHorizontal();
		
		return interaction.Click;
	}

	public static void BeginBox(params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal("textArea", options);
		GUILayout.BeginVertical();
	}
	
	public static void EndBox()
	{
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	public static void BoxTitle(string text)
	{
		GUIStyle TextStyle = GUI.skin.GetStyle("textContainerTitle");
		
		TextWithShadow(text, TextStyle, TextStyle.normal.textColor, TextStyle.hover.textColor, new Vector2(0.0f, 2.0f));
	}
	
	public static void BoxText(string text)
	{
		GUIStyle TextStyle = GUI.skin.GetStyle("textContainerText");
		
		TextWithShadow(text, TextStyle, TextStyle.normal.textColor, TextStyle.hover.textColor, new Vector2(0.0f, 2.0f));
	}

	public static bool Toggle(bool toggle, string text)
	{
		return Toggle(toggle, text, Align.Left);
	}
	
	public static bool Toggle(bool toggle, string text, Align align)
	{
		GUIStyle ToggleTextStyle = GUI.skin.GetStyle("toggleText");
		
		GUILayout.BeginHorizontal();

		if (align == Align.Left)
			GUILayout.Space(9.0f);

		if (align != Align.Left)
			GUILayout.FlexibleSpace();
		
		// Get the rect where the toggle should be
		Rect toggleRect = GUILayoutUtility.GetRect(new GUIContent(""), "Toggle");

		// Get the rect of the label
		Rect textRect = GUILayoutUtility.GetRect(new GUIContent(text), ToggleTextStyle);

		// Now put together a little interaction rect
		Rect interRect = new Rect(toggleRect.x, toggleRect.y, (toggleRect.width + textRect.width), toggleRect.height);
		
		// Get the interaction
		RectInteraction interaction = RectInteraction.Get(interRect);
		
		// Draw the toggle
		if (Event.current.type == EventType.Repaint)
			GUI.skin.GetStyle("Toggle").Draw(toggleRect, interaction.IsHovered, interaction.IsPressed, toggle, false);
		
		// Draw the toggle text
		TextWithShadow(new Rect((toggleRect.x + toggleRect.width), toggleRect.y, textRect.width, textRect.height), text, ToggleTextStyle, ToggleTextStyle.normal.textColor, new Color(0.0f, 0.0f, 0.0f, 0.35f), new Vector2(1.0f, 2.0f));
		
		// Click
		if (interaction.Click)
			toggle = !toggle;
		
		if (align == Align.Center)
			GUILayout.FlexibleSpace();

		if (align == Align.Right)
			GUILayout.Space(9.0f);
		
		GUILayout.EndHorizontal();
		
		return toggle;
	}

	// Displays a vertical list of toggles and returns the index of the selected item.
	public static int ToggleList(int selected, string[] items)
	{
		return ToggleList(selected, items, 10.0f);
	}

	public static int ToggleList(int selected, string[] items, float spacing)
	{
		// Keep the selected index within the bounds of the items array
		selected = ((selected < 0) ? 0 : (selected >= items.Length ? (items.Length - 1) : selected));
		
		// Get the radio toggles style
		GUIStyle radioStyle = GUI.skin.GetStyle("radioToggle");
		GUIStyle ToggleTextStyle = GUI.skin.GetStyle("toggleText");
		
		for (int i = 0; i < items.Length; i++)
		{
			// Add spacing
			if (i > 0)
				GUILayout.Space(spacing);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(14.0f);
			
			// Get the rect where the toggle should be
			Rect toggleRect = GUILayoutUtility.GetRect(new GUIContent(""), radioStyle);
			
			// Get the rect for the text
			Rect textRect = GUILayoutUtility.GetRect(new GUIContent(items[i]), ToggleTextStyle);
			
			// Now put together a little interaction rect
			Rect interRect = new Rect(toggleRect.x, toggleRect.y, (toggleRect.width + textRect.width), toggleRect.height);
			
			// Get the interaction
			RectInteraction interaction = RectInteraction.Get(interRect);
			
			// Draw the toggle
			if (Event.current.type == EventType.Repaint)
				radioStyle.Draw(toggleRect, interaction.IsHovered, interaction.IsPressed, (selected == i), false);
			
			// Draw the toggle text
			TextWithShadow(new Rect((toggleRect.x + toggleRect.width), (textRect.y - 8.0f), textRect.width, textRect.height), items[i], ToggleTextStyle, ToggleTextStyle.normal.textColor, new Color(0.0f, 0.0f, 0.0f, 0.35f), new Vector2(1.0f, 2.0f));
			
			// Click
			if (interaction.Click)
				selected = i;
			
			GUILayout.EndHorizontal();
		}
		
		// Return the currently selected item's index
		return selected;
	}

	public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(GUI.skin.GetStyle("scrollview").overflow.left);
		GUILayout.BeginVertical();
		GUILayout.Space(GUI.skin.GetStyle("scrollview").overflow.top);
		
		return GUILayout.BeginScrollView(scrollPosition, options);
	}
	
	public static void EndScrollView()
	{
		GUILayout.EndScrollView();
		
		GUILayout.Space(GUI.skin.GetStyle("scrollview").overflow.bottom);
		GUILayout.EndVertical();
		GUILayout.Space(GUI.skin.GetStyle("scrollview").overflow.right);
		GUILayout.EndHorizontal();
	}

	public static void TooltipBox(Vector2 position, string text)
	{
		TooltipBox(position, text, 0.0f);
	}

	// In case we are using positioned tooltip max width is always needed
	private static float defaultTooltipMaxWidth = 410.0f;

	public static void TooltipBox(Vector2 position, string text, float maxWidth)
	{
		GUIStyle boxStyle = GUI.skin.GetStyle("tooltipBox");

		float internalMaxWidth = (maxWidth > 0.0f ? maxWidth : defaultTooltipMaxWidth);

		float contentHeight = boxStyle.CalcHeight(new GUIContent(text), internalMaxWidth);

		GUILayout.BeginArea(new Rect((position.x - boxStyle.overflow.left), 
		                             (position.y - boxStyle.overflow.top), 
		                             (internalMaxWidth + boxStyle.overflow.left + boxStyle.overflow.right), 
		                             (contentHeight + boxStyle.overflow.top + boxStyle.overflow.bottom)));

		GUILayout.BeginHorizontal();
		GUILayout.Space(boxStyle.overflow.left);

		TooltipBox(text, maxWidth);

		GUILayout.Space(boxStyle.overflow.right);
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	public static void TooltipBox(string text)
	{
		TooltipBox(text, 0.0f);
	}

	public static void TooltipBox(string text, float maxWidth)
	{
		GUIStyle boxStyle = GUI.skin.GetStyle("tooltipBox");
		GUIStyle textStyle = GUI.skin.GetStyle("tooltipBoxText");

		// Draw the box
		if (maxWidth > 0.0f)
			GUILayout.Box(text, boxStyle, GUILayout.MaxWidth(maxWidth));
		else
			GUILayout.Box(text, boxStyle);

		// Get the tooltip rect
		Rect rect = GUILayoutUtility.GetLastRect();

		// Place the tooltip anchor
		GUI.DrawTexture(new Rect((rect.x - 7.0f), (rect.y + 16.0f), 22.0f, 14.0f), GUI.skin.GetStyle("tooltipAnchor").normal.background);

		// Draw the box text
		TextWithShadow(
			new Rect((rect.x + boxStyle.padding.left), (rect.y + boxStyle.padding.top), (rect.width - boxStyle.padding.left - boxStyle.padding.right), (rect.height - boxStyle.padding.top - boxStyle.padding.bottom)),
			text,
			textStyle,
			textStyle.normal.textColor,
			textStyle.onNormal.textColor,
			new Vector2(0.0f, 2.0f)
		);
	}

	public static void Label(string text)
	{
		GUIStyle style = GUI.skin.GetStyle("label");

		// Draw the text with shadow
		TextWithShadow(
			text,
			style,
			style.normal.textColor,
			style.onNormal.textColor,
			new Vector2(0.0f, 2.0f)
		);
	}

	public static string LabeledTextField(string value, string label, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Space(23.0f);

		// Draw the text field
		value = GUILayout.TextField(value, options);

		// Get the rect of the field
		Rect rect = GUILayoutUtility.GetLastRect();

		// Begin a group for the label
		GUI.BeginGroup(new Rect(rect.x, (rect.y - 23.0f), rect.width, 34.0f));

		// Draw the label text
		GUIStyle labelStyle = new GUIStyle("label");
		labelStyle.alignment = TextAnchor.UpperCenter;

		// Draw the text with shadow
		TextWithShadow(
			new Rect(0, 0, rect.width, 20.0f),
			label,
			labelStyle,
			labelStyle.normal.textColor,
			labelStyle.onNormal.textColor,
			new Vector2(0.0f, 2.0f)
		);

		// Draw the label separator
		GUI.Box(new Rect(((rect.width - 333.0f) / 2.0f), 25.0f, 333.0f, 8.0f), "", "InputLabelBackground");

		// End the group
		GUI.EndGroup();

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		return value;
	}

	public static void LoadingBar(Vector2 position, float percent)
	{
		Texture2D background = GUI.skin.GetStyle("LoadingBarBackground").normal.background;
		Texture2D fill = GUI.skin.GetStyle("LoadingBarFill").normal.background;
		GUIStyle textStyle = GUI.skin.GetStyle("LoadingBarText");

		// Begin a group to hold everything
		GUI.BeginGroup(new Rect(position.x, position.y, 756.0f, 88.0f));

		// Draw the background
		GUI.DrawTexture(new Rect(0, 0, 756.0f, 88.0f), background);

		// Draw the fill
		GUI.BeginGroup(new Rect(26.0f, 27.0f, (percent * fill.width), fill.height));
		GUI.DrawTexture(new Rect(0, 0, fill.width, fill.height), fill);
		GUI.EndGroup();

		// Draw the text
		TextWithShadow(
			new Rect(0, 0, 756.0f, 88.0f),
			"Loading",
			textStyle,
			textStyle.normal.textColor,
			textStyle.onNormal.textColor,
			new Vector2(0.0f, 2.0f)
		);

		// End the group
		GUI.EndGroup();
	}

	/*
	 * TABLES
	 */

	private class TableData
	{
		public bool IsHeader = false;
		public int CurrentRowIndex = 0;
		public int CurrentColumnIndex = 0;
		public Dictionary<int, Rect> SavedRowRects = new Dictionary<int, Rect>();
		public Dictionary<int, float> SavedColumnWidth = new Dictionary<int, float>();

		public float BodyHeight = 0.0f;

		// Scroll view stuff
		public Vector2 ScrollPosition = Vector2.zero;
		public float ScrollViewContentHeight = 0.0f;

		public float CurrentColumnWidth()
		{
			if (this.SavedColumnWidth.ContainsKey(this.CurrentColumnIndex))
				return this.SavedColumnWidth[this.CurrentColumnIndex];

			return 0.0f;
		}

		public void SaveColumnWidth(float width)
		{
			this.SavedColumnWidth[this.CurrentColumnIndex] = width;
		}

		public void SaveRowRect(Rect r)
		{
			if (r.x == 0.0f && r.y == 0.0f && r.width == 1.0f && r.height == 1.0f)
				return;

			if (this.SavedRowRects.ContainsKey(this.CurrentRowIndex))
				this.SavedRowRects[this.CurrentRowIndex] = r;
			else
				this.SavedRowRects.Add(this.CurrentRowIndex, r);
		}

		public Rect GetCurrentRowRect()
		{
			if (this.SavedRowRects.ContainsKey(this.CurrentRowIndex))
				return this.SavedRowRects[this.CurrentRowIndex];

			return new Rect(0, 0, 0, 0);
		}
	}

	private static int TableCurrentControlID = 0;
	private static Dictionary<int, TableData> TablesData = new Dictionary<int, TableData>();
	
	private static TableData GetCurrentTableData()
	{
		if (TablesData.ContainsKey(TableCurrentControlID))
			return TablesData[TableCurrentControlID];
		else
			TablesData.Add(TableCurrentControlID, new TableData());

		return TablesData[TableCurrentControlID];
	}

	public static void BeginTable(int ControlID)
	{
		// Get the table control id
		TableCurrentControlID = ControlID;

		// Begin the table
		GUILayout.BeginVertical("table");

		// Reset the rows indexer
		GetCurrentTableData().CurrentRowIndex = 0;
	}
	
	public static void EndTable()
	{
		GUILayout.EndVertical();
	}

	public static void BeginTableHeader()
	{
		GUILayout.BeginHorizontal("tableHeader");

		GetCurrentTableData().IsHeader = true;
		// Reset the column indexer
		GetCurrentTableData().CurrentColumnIndex = 0;
	}
	
	public static void EndTableHeader()
	{
		GUILayout.EndHorizontal();

		GetCurrentTableData().IsHeader = false;
	}

	public static void BeginTableBody(float height)
	{
		// Save the height
		GetCurrentTableData().BodyHeight = height;
		
		GUILayout.BeginHorizontal();
		
		// Do the scroll view
		GetCurrentTableData().ScrollPosition = GUILayout.BeginScrollView(GetCurrentTableData().ScrollPosition, "tableScrollView", GUILayout.Height(height));
		
		GUILayout.BeginVertical();
	}
	
	public static void EndTableBody()
	{
		// Check if we need to make some corrections on the scrollview padding
		// only in case we have a scroll bar, so we need to determine if we have one
		
		GUILayout.EndVertical();

		// Save the scroll view content height
		if (Event.current.type == EventType.Repaint)
			GetCurrentTableData().ScrollViewContentHeight = GUILayoutUtility.GetLastRect().height;
		
		GUILayout.EndScrollView();
		
		// Check if the content is greater than the scroll view height
		if (Event.current.type != EventType.Repaint && GetCurrentTableData().ScrollViewContentHeight > GetCurrentTableData().BodyHeight)
			GUILayout.Space(6.0f);

		GUILayout.EndHorizontal();
	}

	public static RectInteraction BeginTableRow()
	{
		// Prepare the row style
		GUIStyle style = new GUIStyle("tableRow");

		// Get the row rect
		Rect rect = GetCurrentTableData().GetCurrentRowRect();
		
		// Get the row interaction
		RectInteraction interaction = RectInteraction.Get(rect, false);
		
		// Check if the row is hovered
		if (interaction.IsHovered)
			style.normal.background = style.hover.background;
		
		// remove the hover texture from the style
		style.hover.background = null;

		// Do the row
		GUILayout.BeginHorizontal(style);
		
		// Reset the column indexer
		GetCurrentTableData().CurrentColumnIndex = 0;
		
		// Return the row interaction
		return interaction;
	}
	
	public static void EndTableRow()
	{
		GUILayout.EndHorizontal();

		// Save the rect
		if (Event.current.type == EventType.Repaint)
			GetCurrentTableData().SaveRowRect(GUILayoutUtility.GetLastRect());

		// Increase the rows indexer
		GetCurrentTableData().CurrentRowIndex++;
	}

	public static void TableColumn(string text)
	{
		TableColumn(new GUIContent(text), 0.0f);
	}

	public static void TableColumn(GUIContent text)
	{
		TableColumn(text, 0.0f);
	}

	public static void TableColumn(string text, float width)
	{
		TableColumn(new GUIContent(text), width);
	}

	public static void TableColumn(GUIContent text, float width)
	{
		string textStyleStr = (GetCurrentTableData().IsHeader ? "tableHeaderText" : "tableText");
		GUIStyle textStyle = GUI.skin.GetStyle(textStyleStr);
		GUIStyle separatorStyle = GUI.skin.GetStyle(GetCurrentTableData().IsHeader ? "tableHeaderSeparator" : "tableSeparator");

		// Check if we need fixed width
		if (width > 0.0f)
		{
			textStyle = new GUIStyle(textStyleStr);
			textStyle.fixedWidth = width;
		}
		else if (!GetCurrentTableData().IsHeader)
		{
			// Try using a saved size
			if (GetCurrentTableData().CurrentColumnWidth() > 0.0f)
			{
				textStyle = new GUIStyle(textStyleStr);
				textStyle.fixedWidth = GetCurrentTableData().CurrentColumnWidth();
			}
		}

		// Check if we need place separator
		if (GetCurrentTableData().CurrentColumnIndex > 0)
			GUILayout.Box("", separatorStyle);

		// Draw the text
		TextWithShadow(
			text,
			textStyle,
			textStyle.normal.textColor,
			textStyle.onNormal.textColor,
			new Vector2(0.0f, 2.0f)
		);

		// If this is a header column, save it's width to the list
		if (GetCurrentTableData().IsHeader)
		{
			if (width > 0.0f)
			{
				GetCurrentTableData().SaveColumnWidth(width);
			}
			else if (Event.current.type == EventType.Repaint)
			{
				GetCurrentTableData().SaveColumnWidth(GUILayoutUtility.GetLastRect().width);
			}
		}

		// Increase the index
		GetCurrentTableData().CurrentColumnIndex++;
	}

	public static void BeginTableColumn()
	{
		BeginTableColumn(0.0f);
	}

	public static void BeginTableColumn(float width)
	{
		GUIStyle separatorStyle = GUI.skin.GetStyle(GetCurrentTableData().IsHeader ? "tableHeaderSeparator" : "tableSeparator");

		// Check if this is the header row and we can save the user defined width
		if (GetCurrentTableData().IsHeader && width > 0.0f)
			GetCurrentTableData().SaveColumnWidth(width);

		// Check if there is no user defined width and we should try using a saved width
		if (!GetCurrentTableData().IsHeader && width == 0.0f && GetCurrentTableData().CurrentColumnWidth() > 0.0f)
			width = GetCurrentTableData().CurrentColumnWidth();

		// Check if we need place separator
		if (GetCurrentTableData().CurrentColumnIndex > 0)
			GUILayout.Box("", separatorStyle);

		// Check if we have fixed width
		if (width > 0.0f)
			GUILayout.BeginVertical(GUILayout.Width(width));
		else
			GUILayout.BeginVertical();

		GUILayout.BeginHorizontal("tableColumn");
	}

	public static void EndTableColumn()
	{
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		// If this is a header column, save it's width to the list if no width was saved already
		if (GetCurrentTableData().IsHeader && Event.current.type == EventType.Repaint && GetCurrentTableData().CurrentColumnWidth() == 0.0f)
			GetCurrentTableData().SaveColumnWidth(GUILayoutUtility.GetLastRect().width);

		// Increase the index
		GetCurrentTableData().CurrentColumnIndex++;
	}
}

