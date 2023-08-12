using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MuchoBestoStudio.MinimalistSceneSwitcher.Editor
{
	[InitializeOnLoad]
	public static class SceneDatabase
	{
		#region Variables

		public static SceneData[] Data { get; private set; } = new SceneData[0];

		#endregion

		#region Constructor

		static SceneDatabase()
		{
			PopulateData();

			EditorApplication.projectChanged -= OnProjectChanged;
			EditorApplication.projectChanged += OnProjectChanged;

			EditorSceneManager.sceneOpening -= OnSceneOpening;
			EditorSceneManager.sceneOpening += OnSceneOpening;

			EditorSceneManager.sceneClosing -= OnSceneClosing;
			EditorSceneManager.sceneClosing += OnSceneClosing;
		}

		#endregion

		#region Unity's Callbacks

		private static void OnProjectChanged()
		{
			PopulateData();
		}

		private static void OnSceneOpening(string path, OpenSceneMode mode)
		{
			for (int index = 0; index < Data.Length; index++)
			{
				if (Data[index].Path == path)
				{
					Data[index].IsInHierarchy = true;
					return;
				}
			}
		}

		private static void OnSceneClosing(Scene scene, bool removingScene)
		{
			if (!removingScene)
			{
				return;
			}

			for (int index = 0; index < Data.Length; index++)
			{
				if (Data[index].Path == scene.path)
				{
					Data[index].IsInHierarchy = false;
					return;
				}
			}
		}


		#endregion

		#region Data

		public static void PopulateData()
		{
			string[] scenesGuid = AssetDatabase.FindAssets("t:Scene");

			Data = new SceneData[scenesGuid.Length];
			for (int index = 0; index < scenesGuid.Length; index++)
			{
				string scenePath = AssetDatabase.GUIDToAssetPath(scenesGuid[index]);
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);

				Scene scene = EditorSceneManager.GetSceneByPath(scenePath);

				Data[index] = new SceneData()
				{
					Path = scenePath,
					Name = sceneName,
					IsInHierarchy = scenePath == scene.path,
				};
			}
		}

		public static bool HasScene()
		{
			return Data.Length > 0;
		}

		public static bool IsAllScenesOpened()
		{
			return Data.Length == EditorSceneManager.sceneCount;
		}

		private static int GetDataIndexByPath(string path)
		{
			for (int index = 0; index < Data.Length; index++)
			{
				if (Data[index].Path == path)
				{
					return index;
				}
			}

			throw new Exception($"Failed to get index of the data with path {path}.");
		}

		#endregion
	}
}
