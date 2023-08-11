using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MuchoBestoStudio.MinimalistSceneSwitcher.Editor
{
	public class CustomPopup : PopupWindowContent
	{
		#region Variables

		private string _currentScenePath = string.Empty;
		private SceneData[] _scenesData = new SceneData[0];

		#endregion

		#region Constructor

		public CustomPopup(string currentScenePath)
		{
			_currentScenePath = currentScenePath;

			RetrieveScenesAssets();

			EditorApplication.projectChanged -= OnProjectChanged;
			EditorApplication.projectChanged += OnProjectChanged;
		}

		#endregion

		#region Unity's Editor Application Callback

		private void OnProjectChanged()
		{
			RetrieveScenesAssets();
		}

		#endregion

		#region Unity's Messaging

		public override void OnGUI(Rect rect)
		{
			for (int sceneIndex = 0; sceneIndex < _scenesData.Length; sceneIndex++)
			{
				if (GUILayout.Button(_scenesData[sceneIndex].Name))
				{
					editorWindow.Close();

					ChangeScene(_currentScenePath, _scenesData[sceneIndex].Path);
				}
			}
		}

		#endregion

		#region Scenes

		private void RetrieveScenesAssets()
		{
			string[] scenesGuid = AssetDatabase.FindAssets("t:Scene");

			_scenesData = new SceneData[scenesGuid.Length];
			for (int index = 0; index < scenesGuid.Length; index++)
			{
				string scenePath = AssetDatabase.GUIDToAssetPath(scenesGuid[index]);
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);

				_scenesData[index] = new SceneData()
				{
					Path = scenePath,
					Name = sceneName,
				};
			}
		}

		private static void ChangeScene(string previousScenePath, string newScenePath)
		{
			// Note(rc): Do nothing if it is the same scene path
			if (previousScenePath == newScenePath)
			{
				return;
			}

			Scene activeScene = SceneManager.GetActiveScene();
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
