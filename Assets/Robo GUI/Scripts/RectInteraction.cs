using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RectInteraction {

	// STATIC
	private static List<int> mDragged = new List<int>();
	private static List<int> mHotControls = new List<int>();

	public static RectInteraction Get(Rect bounds)
	{
		return Get(bounds, true, false);
	}

	public static RectInteraction Get(Rect bounds, bool useEvents)
	{
		return Get(bounds, useEvents, false);
	}

	public static RectInteraction Get(Rect bounds, bool useEvents, bool releaseOnDrag)
	{
		int ControlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);

		RectInteraction interaction = new RectInteraction();

		// Check if we have a current window
		if (UIWindow.Current != null)
		{
			// Check if the mouse is over the current window
			if (!UIWindow.Current.IsMouseOver())
				return interaction;
		}

		interaction.IsHovered = (bounds.Contains(Event.current.mousePosition));
		interaction.IsPressed = (mHotControls.Contains(ControlID));
		
		if (interaction.IsHovered)
		{
			// Check for clicks
			if (Event.current.type == EventType.MouseDown)
			{
				// pressed
				if (!mHotControls.Contains(ControlID))
					mHotControls.Add(ControlID);

				// Prevent propagation
				if (useEvents)
					Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseUp)
			{
				if ((mHotControls.Contains(ControlID) && !releaseOnDrag) || (mHotControls.Contains(ControlID) && releaseOnDrag && !mDragged.Contains(ControlID)))
				{
					// Register as click
					interaction.Click = true;

					// Prevent propagation
					if (useEvents)
						Event.current.Use();
				}

				if (mHotControls.Contains(ControlID))
				{
					// Remove from the hot list
					mHotControls.Remove(ControlID);
				}
			}

			if (Input.GetMouseButtonDown(0) && Event.current.type == EventType.MouseDrag)
			{
				if (mHotControls.Contains(ControlID))
				{
					// Register as drag
					interaction.Drag = true;

					// Add to the dragged list
					if (!mDragged.Contains(ControlID))
						mDragged.Add(ControlID);

					// Remove from the hot list
					if (mHotControls.Contains(ControlID) && releaseOnDrag)
						mHotControls.Remove(ControlID);

					// Prevent propagation
					if (useEvents)
						Event.current.Use();
				}
			}
		}
		else
		{
			// If no longer hovered and the hot control id is this one
			// release the hot control
			if (mHotControls.Contains(ControlID))
				mHotControls.Remove(ControlID);
		}

		// On mouse release clear the drag and clear saved mouse positions
		if (Input.GetMouseButtonUp(0))
		{
			if (mDragged.Contains(ControlID))
				mDragged.Remove(ControlID);
		}
		
		return interaction;
	}

	// OOP
	public bool IsPressed = false;
	public bool IsHovered = false;
	public bool Click = false;
	public bool Drag = false;
}

