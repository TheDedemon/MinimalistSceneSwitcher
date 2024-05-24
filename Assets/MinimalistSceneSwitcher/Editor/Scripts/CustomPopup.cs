using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheDedemon.MinimalistSceneSwitcher.Editor
{
	public class CustomPopup : PopupWindowContent
	{
		#region Variables

		private string _currentScenePath = string.Empty;

		#endregion

		#region Constructor

		public CustomPopup(string currentScenePath)
		{
			_currentScenePath = currentScenePath;
		}

		#endregion

		#region Unity's Messaging

		public override void OnGUI(Rect rect)
		{
			for (int index = 0; index < SceneDatabase.Data.Length; index++)
			{
				SceneData data = SceneDatabase.Data[index];
				if (data.IsInHierarchy)
				{
					GUI.enabled = false;

					GUILayout.Button($"{data.Name} (loaded)");
				}
				else
				{
					GUI.enabled = true;

					if (GUILayout.Button(data.Name))
					{
						editorWindow.Close();

						ChangeScene(_currentScenePath, data.Path);
					}
				}
			}

			GUI.enabled = true;
		}

		#endregion

		#region Scenes

		private static void ChangeScene(string previousScenePath, string newScenePath)
		{
			// Note(rc): Do nothing if it is the same scene path
			if (previousScenePath == newScenePath)
			{
				return;
			}

			Scene activeScene = EditorSceneManager.GetActiveScene();
			if (activeScene.path == previousScenePath)
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
				}
			}
			else
			{
				Scene previousScene = SceneManager.GetSceneByPath(previousScenePath);
				if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { previousScene }))
				{
					// Note(rc): Can't close previous scene here because struct become invalid

					if (previousScene.isLoaded)
					{
						EditorSceneManager.CloseScene(previousScene, true);

						EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Additive);
					}
					else
					{
						EditorSceneManager.CloseScene(previousScene, true);

						EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.AdditiveWithoutLoading);
					}
				}
			}
		}

		#endregion
	}
}
