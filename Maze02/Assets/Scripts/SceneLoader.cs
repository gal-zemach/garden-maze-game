using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
	public class SceneLoader : MonoBehaviour {

		private static int MIN_LEVEL_IDX = 2;
		private static int MAX_LEVEL_IDX = 4;

		public void LoadScene(int index) {
			Debug.Log("SceneLoader: loading scene " + index);
			SceneManager.LoadScene(index);
		}

		public void ReloadCurrentScene()
		{
			int currentScene = SceneManager.GetActiveScene().buildIndex;
			LoadScene(currentScene);
		}

		public void LoadRandomScene() {
			int scene_idx = Random.Range(MIN_LEVEL_IDX,MAX_LEVEL_IDX);
			SceneManager.LoadScene(scene_idx);
		}
	}
}

