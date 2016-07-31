using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour {

	public GUISkin Skin;
	public Texture2D Background;

	private static float GUIScale = 1.0f;
	private static Matrix4x4 oMatrix;
	private static int CurrentDesk = 0;
	private static float LoadingBarPercent = 0.0f;

	private static UIWindow Window1;
	private static UIWindow Window2;
	private static UIWindow Window3;
	private static UIWindow Window4;
	private static UIWindow Window5;
	private static UIWindow Window6;
	private static UIWindow Window7;

	private static UIAnimation Animation1;
	public Texture2D AnimationSprite;

	// Variables used for the ui elements
	private static string textAreaStr = "Suspendisse potenti. Cras eleifend nisi sit amet molestie pellentesque. Fusce vehicula eros neque, a suscipit tortor tristique id. Nam ullamcorper luctus tempus. Sed posuere volutpat dolor. Nunc nibh lacus, congue eu scelerisque non, euismod non libero.";
	private static string textFieldStr = "Suspendisse potenti";
	private static int radioSelected = 0;

	// Variable to hold the cursor textures
	private static Texture2D CursorNormal;
	private static Texture2D CursorActive;

	public void Start()
	{
		// Prepare the windows
		Window1 = UIWindow.Create(new Rect(0, 0, 504, 723), 1, DrawWindow1);
		Window1.Title = "Window Title";
		Window1.Draggable = true;

		Window2 = UIWindow.Create(new Rect(504, 0, 504, 723), 2, DrawWindow2);
		Window2.Title = "Second Window";
		Window2.Draggable = true;

		Window3 = UIWindow.Create(new Rect(1008, 0, 504, 723), 3, DrawWindow3);
		Window3.Title = "Third Window";
		Window3.Draggable = true;

		Window4 = UIWindow.Create(new Rect(0, 0, 503, 527), 4, DrawLoginWindow);
		Window4.Draggable = true;

		// A window containing the demo controls
		Window5 = UIWindow.Create(new Rect(30, -90, (Screen.width - 60), 60), 5, DrawDemoControls);
		Window5.SetAnchor(UIWindow.Anchor.BottomLeft);
		Window5.AutoDraw = false;
		Window5.Background = false;
		Window5.Show();

		Window6 = UIWindow.Create(new Rect(0, 0, 944, 681), 6, DrawTableWindow);
		Window6.Title = "Table Window";
		Window6.Draggable = true;

		Window7 = UIWindow.Create(new Rect(((Screen.width / 2) - 378.0f), ((Screen.height / 2) - 44.0f), 756.0f, 88.0f), 7, DrawLoadingBar);
		Window7.Background = false;

		// Show the current desk windows
		SwitchToDesk(CurrentDesk);

		// Prepare the loading animation
		Animation1 = new UIAnimation(AnimationSprite, 21, 1);

		// Start the example loading bar progress coroutine
		StartCoroutine(LoadingProgress());

		// Try loading the cursors
		CursorNormal = Resources.Load("Cursor/normal") as Texture2D;
		CursorActive = Resources.Load("Cursor/active") as Texture2D;
	}

	public void Update()
	{
		// Feed our animation update event
		Animation1.Update();

		// Set the custom cursor
		if (Input.GetMouseButton(0))
		{
			if (CursorActive)
				Cursor.SetCursor(CursorActive, Vector2.zero, CursorMode.Auto);
		}
		else
		{
			if (CursorNormal)
				Cursor.SetCursor(CursorNormal, Vector2.zero, CursorMode.Auto);
		}
	}

	private static void ApplyScaling()
	{
		// save current matrix
		oMatrix = GUI.matrix;
		
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(GUIScale, GUIScale, 1.0f));
	}

	public void OnGUI()
	{
		// Set the skin
		GUI.skin = Skin;

		// Draw the background
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background, ScaleMode.ScaleAndCrop);

		// The scaling control window has the auto draw disabled
		// that means we decide where and when to draw it
		// so draw it outside the scaling
		Window5.Draw();

		// Apply GUI scaling before drawing the windows
		ApplyScaling();

		// Feed the UIWindow class OnGUI event
		UIWindow.OnGUI();

		// restore matrix before returning
		GUI.matrix = oMatrix;
	}

	private static void DrawDemoControls(int WindowId)
	{
		GUILayout.BeginHorizontal();

		// Scale box
		UIElements.BeginBox(GUILayout.Width(300.0f));
		GUILayout.BeginHorizontal();
		
		UIElements.Label("GUI Scale:");
		GUILayout.Space(10.0f);
		
		GUILayout.BeginVertical();
		GUILayout.Space(2.0f);
		// Make a slider for the GUI scaling value
		GUIScale = GUILayout.HorizontalSlider(GUIScale, 0.75f, 1.0f);
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		UIElements.EndBox();

		// Toggle windows
		UIElements.BeginBox(GUILayout.Width(640.0f));
		GUILayout.BeginHorizontal();
		UIElements.Label("Show:");
		GUILayout.BeginVertical();
		GUILayout.Space(-8.0f);
		GUILayout.BeginHorizontal();

		// Toggle between the diferrent windows, since there's not enough space for all of them
		int selectedDesk = UIElements.ToggleList(CurrentDesk, new string[4] {"Standard windows", "Login window", "Table window", "Loading bar"}, 5.0f);

		// Detect a change in the toggles
		if (selectedDesk != CurrentDesk)
			SwitchToDesk(selectedDesk);

		GUILayout.EndHorizontal();
		GUILayout.Space(-8.0f);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		UIElements.EndBox();

		GUILayout.EndHorizontal();
	}

	private static void SwitchToDesk(int desk)
	{
		// Hide the current desk windows
		switch (CurrentDesk)
		{
			case 0:
				Window1.Hide();
				Window2.Hide();
				Window3.Hide();
				break;
			case 1:
				Window4.Hide();
				break;
			case 2:
				Window6.Hide();
				break;
			case 3:
				Window7.Hide();
				break;
		}

		// Show the new desk
		switch (desk)
		{
			case 0:
				Window1.Show();
				Window2.Show();
				Window3.Show();
				break;
			case 1:
				Window4.Show();
				break;
			case 2:
				Window6.Show();
				break;
			case 3:
				Window7.Show();
				break;
		}

		// Update the current desk variable
		CurrentDesk = desk;
	}

	private static bool toggle1 = false;

	private static void DrawWindow1(int WindowId)
	{
		// Begin a vertical
		GUILayout.BeginVertical();
		GUILayout.Space(10.0f);

		// Do a text with the first style
		UIElements.Text("Donec commodo est ligula, eget pulvinar dui feugiat a. Aenean semper et mauris eu euismod. Nunc interdum velit eu enim venenatis, vitae sollicitudin magna sagittis. Aenean nec pharetra turpis.");

		// Add space
		GUILayout.Space(18.0f);

		// Do an image
		UIElements.ImageBox("image");

		// Add space
		GUILayout.Space(12.0f);

		// Do a separator
		UIElements.Separator();

		// Add space
		GUILayout.Space(20.0f);

		// Do some toggles
		toggle1 = UIElements.Toggle(toggle1, "Donec commodo est ligula?");
		GUILayout.Space(10.0f);
		Window2.ShowWindow = UIElements.Toggle(Window2.ShowWindow, "Display the second window?");
		GUILayout.Space(10.0f);
		Window3.ShowWindow = UIElements.Toggle(Window3.ShowWindow, "Display the thrid window?");

		// Add space
		GUILayout.Space(24.0f);

		// Do a separator
		UIElements.Separator();

		// Add space
		GUILayout.Space(30.0f);

		// Do a text with the second style
		UIElements.Text("Donec commodo est ligula, eget pulvinar dui feugiat a. Aenean semper et mauris eu euismod. Nunc interdum velit eu enim venenatis, vitae sollicitudin magna sagittis. Aenean nec pharetra turpis.", UIElements.TextStyle.Two);

		// End the vertical
		GUILayout.EndVertical();
	}

	private static void DrawWindow2(int WindowId)
	{
		// Begin a vertical
		GUILayout.BeginVertical();

		// Add space
		GUILayout.Space(10.0f);

		// Do a centered button
		UIElements.Button("Button", UIElements.Align.Center);

		// Add space
		GUILayout.Space(20.0f);

		// Do a box
		UIElements.BeginBox();
		UIElements.BoxTitle("Text Container");
		UIElements.BoxText(textAreaStr);
		UIElements.EndBox();

		// Do a text area
		//textAreaStr = GUILayout.TextArea(textAreaStr);

		// Add space
		GUILayout.Space(14.0f);

		// Do a text field
		textFieldStr = GUILayout.TextField(textFieldStr);

		// Add space
		GUILayout.Space(26.0f);

		// Do a group of radio style toggles
		radioSelected = UIElements.ToggleList(radioSelected, new string[3] {"Semper facilisis tellus ?", "Phasellus eu sodales leo!", "Aliquam semper facilisis tellus..."});

		// End the vertical
		GUILayout.EndVertical();
	}

	private static Vector2 scrollPosition = Vector2.zero;

	private static void DrawWindow3(int WindowId)
	{
		// Begin a vertical
		GUILayout.BeginVertical();

		// Do a scroll view
		scrollPosition = UIElements.BeginScrollView(scrollPosition, GUILayout.Height(151.0f));
		UIElements.BoxText("Suspendisse potenti. Cras eleifend nisi sit amet molestie pellentesque. Fusce vehicula eros neque, a suscipit tortor tristique id. Nam ullamcorper luctus tempus. Sed posuere volutpat dolor. Nunc nibh lacus, congue eu scelerisque non, euismod non libero. Sed posuere volutpat dolor. Sed posuere volutpat dolor. Nunc nibh lacus, congue eu scelerisque non, euismod non libero. Sed posuere volutpat dolor. Sed posuere volutpat...");
		GUILayout.Space(20.0f);
		UIElements.BoxText("Nunc nibh lacus, congue eu scelerisque non, euismod non libero. Nunc nibh lacus, congue eu scelerisque non, euismod non libero. Nunc nibh lacus, congue eu scelerisque non, euismod non libero.");
		UIElements.EndScrollView();

		// Add space
		GUILayout.Space(14.0f);

		// Do a horizontal slider
		Animation1.FPS = GUILayout.HorizontalSlider(Animation1.FPS, 20.0f, 40.0f);

		// Add space
		GUILayout.Space(19.0f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(4.0f);

		// Do a vertical slider
		Animation1.PercentageUpdatePeriod = GUILayout.VerticalSlider(Animation1.PercentageUpdatePeriod, 0.0f, 0.2f, GUILayout.Height(173.0f));

		// Add horizontal space
		GUILayout.Space(106.0f);

		// Draw the animation
		DrawAnimation();

		GUILayout.EndHorizontal();

		// Add space
		GUILayout.Space(11.0f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(1.0f);

		// Do the tiny icon buttons
		UIElements.TinyButton(UIElements.TinyButtons.Accept);
		UIElements.TinyButton(UIElements.TinyButtons.Decline);
		UIElements.TinyButton(UIElements.TinyButtons.Social);
		UIElements.TinyButton(UIElements.TinyButtons.Mail);

		// Add horizonta space
		GUILayout.Space(26.0f);
		GUILayout.BeginVertical();
		GUILayout.Space(9.0f);
		GUILayout.BeginHorizontal();

		// Do pagination arrows
		UIElements.ArrowButton(UIElements.ArrowButtons.Left);
		UIElements.ArrowButton(UIElements.ArrowButtons.Middle);
		UIElements.ArrowButton(UIElements.ArrowButtons.Right);

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		// Add space
		GUILayout.Space(14.0f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(3.0f);

		// Do a small button
		UIElements.SmallButton("Button");

		GUILayout.EndHorizontal();

		// Do a positioned tooltip box
		// there is a non-positioned but i dont see why anybody would need it... : ]
		UIElements.TooltipBox(new Vector2(227.0f, 609.0f), "Aliquam varius, nisi eu porttitor.");

		// End the vertical
		GUILayout.EndVertical();
	}

	private static void DrawAnimation()
	{
		// Begin vertical
		GUILayout.BeginVertical();
		
		// Add space
		GUILayout.Space(36.0f);
		
		// Do the loading animation
		Rect rect = Animation1.Draw();

		// Begin a group
		GUI.BeginGroup(rect);

		// Draw the percentage text
		UIElements.TextWithShadow(
			new Rect(30, 37, 50, 20),
			Animation1.Percentage.ToString() + "%",
			GUI.skin.GetStyle("animationPctText"),
			GUI.skin.GetStyle("animationPctText").normal.textColor,
			GUI.skin.GetStyle("animationPctText").hover.textColor,
			new Vector2(0.0f, 2.0f)
		);

		// Draw the percentage text
		UIElements.TextWithShadow(
			new Rect(30, 58, 50, 20),
			"loading",
			GUI.skin.GetStyle("animationSubText"),
			GUI.skin.GetStyle("animationSubText").normal.textColor,
			GUI.skin.GetStyle("animationSubText").hover.textColor,
			new Vector2(0.0f, 2.0f)
		);

		// End the group
		GUI.EndGroup();

		// End the vertical
		GUILayout.EndVertical();
	}

	private static void DrawTableWindow(int WindowId)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(2.0f);

		// Do a table, make sure you give unique ID's to every diferrent table
		UIElements.BeginTable(1);

		// Do the table header
		UIElements.BeginTableHeader();
		UIElements.TableColumn("Etiam eu", 150.0f);
		UIElements.TableColumn("Ipsum diam", 140.0f);
		UIElements.TableColumn("Nunc", 88.0f);
		UIElements.TableColumn("Convallis", 120.0f);
		UIElements.TableColumn("Blandit", 104.0f);
		UIElements.TableColumn("");
		UIElements.EndTableHeader();

		// Do the table body
		UIElements.BeginTableBody(432.0f);

		// Do some rows
		for (int i = 0; i < 20; i++)
		{
			RectInteraction rowInteraction = UIElements.BeginTableRow();

			UIElements.TableColumn((rowInteraction.IsHovered ? "<color=#d4d4d4ff>" : "<color=#9a9a9aff>") + "<b>An title goes here</b></color>");
			UIElements.TableColumn("255 / 255   <color=#aa3330ff><b>FULL</b></color>");
			UIElements.TableColumn("12 ms");
			UIElements.TableColumn("<size=11><i>None</i></size>");
			UIElements.TableColumn("<size=11><i>None</i></size>");

			// Do a column group
			UIElements.BeginTableColumn(104.0f);

			GUILayout.BeginVertical();
			GUILayout.Space(6.0f);
			GUILayout.BeginHorizontal();
			GUILayout.Space(14.0f);

			// Do some buttons
			//GUILayout.Button("", "HeartButton");
			UIElements.IconButton(UIElements.IconButtons.Heart);
			GUILayout.Space(22.0f);
			UIElements.IconButton(UIElements.IconButtons.Plus);
			GUILayout.Space(22.0f);
			UIElements.IconButton(UIElements.IconButtons.Decline);
			GUILayout.Space(22.0f);
			UIElements.IconButton(UIElements.IconButtons.Star);

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			UIElements.EndTableColumn();

			UIElements.EndTableRow();
		}

		UIElements.EndTableBody();
		UIElements.EndTable();

		GUILayout.Space(2.0f);
		GUILayout.EndHorizontal();
	}

	private static string username = "Suspendisse potenti";
	private static string password = "Suspendisse potenti";
	private static bool rememberme = false;

	private static void DrawLoginWindow(int WindowId)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(19.0f);
		username = UIElements.LabeledTextField(username, "Username / Email");
		GUILayout.Space(25.0f);
		password = UIElements.LabeledTextField(password, "Password");
		GUILayout.Space(25.0f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(-5.0f); // space correction
		rememberme = UIElements.Toggle(rememberme, "Remember me", UIElements.Align.Center);
		GUILayout.EndHorizontal();
		GUILayout.Space(20.0f);
		UIElements.Button("Button", UIElements.Align.Center);
		GUILayout.EndVertical();
	}

	private static void DrawLoadingBar(int WindowId)
	{
		// Update the position of this window on every draw
		// to make sure it's always centered
		Window7.SetPosition(new Vector2(((Screen.width / 2) - 378.0f), ((Screen.height / 2) - 44.0f)));

		// Draw the bar
		UIElements.LoadingBar(new Vector2(0.0f, 0.0f), LoadingBarPercent);
	}

	private IEnumerator LoadingProgress()
	{
		float Duration = 4.0f;
		float ResetDelay = 1.0f;
		
		// Reset to 0%
		LoadingBarPercent = 0.0f;
		
		// Get the timestamp
		float startTime = Time.time;
		
		while (Time.time < (startTime + Duration))
		{
			float RemainingTime = (startTime + Duration) - Time.time;
			float ElapsedTime = Duration - RemainingTime;
			
			// update the percent value
			LoadingBarPercent = (ElapsedTime / Duration);
			
			yield return 0;
		}
		
		// Round to 100%
		LoadingBarPercent = 1.0f;
		
		// Duration of the display of the notification
		yield return new WaitForSeconds(ResetDelay);
		
		// Get the timestamp
		startTime = Time.time;
		
		while (Time.time < (startTime + Duration))
		{
			float RemainingTime = (startTime + Duration) - Time.time;
			
			// update the percent value
			LoadingBarPercent = (RemainingTime / Duration);
			
			yield return 0;
		}
		
		// Reset to 0%
		LoadingBarPercent = 0.0f;
		
		// Duration of the display of the notification
		yield return new WaitForSeconds(ResetDelay);
		
		// Start it again
		StartCoroutine(LoadingProgress());
	}
}

