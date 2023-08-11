using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MuchoBestoStudio.MinimalistSceneSwitcher.Editor
{
	[InitializeOnLoad]
	public static class CustomHierarchy
	{
		#region Variables

		public const string ButtonIcon = "preAudioLoopOff";
		public const string ButtonTooltip = "Switch scene";
		public const float ButtonWidth = 24f;

		#endregion

		#region Constructor

		static CustomHierarchy()
		{
			EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
		}

		#endregion

		#region Editor Application's Callback

		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			Object obj = EditorUtility.InstanceIDToObject(instanceID);
			if (obj != null) 
			{
				// Note(rc): Stop here if obj exists because it means that it is not a scene
				return; 
			}

			for (int index = 0; index < SceneManager.sceneCount; index++)
			{
				Scene loadedScene = SceneManager.GetSceneAt(index);
				if (loadedScene.GetHashCode() == instanceID)
				{
					Rect buttonRect = new Rect(selectionRect.xMax - ButtonWidth, selectionRect.yMin, ButtonWidth, selectionRect.height);
					if (GUI.Button(buttonRect, new GUIContent(EditorGUIUtility.IconContent(ButtonIcon).image, ButtonTooltip)))
					{
						PopupWindow.Show(buttonRect, new CustomPopup(loadedScene.path));
					}

					return;
				}
			}
		}

		#endregion
	}
}
