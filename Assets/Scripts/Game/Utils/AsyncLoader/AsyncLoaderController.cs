using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Everest.PuzzleGame
{
    public class AsyncLoaderController
    {
        public void LoadScene(string sceneToLoad, bool isAdditive, MonoBehaviour mono, Action<float> onProgressUpdate = null)
        {
            mono.StartCoroutine(LoadAsyncScene(sceneToLoad, isAdditive, onProgressUpdate));
        }

        private IEnumerator LoadAsyncScene(string sceneToLoad, bool isAdditive, Action<float> onProgressUpdate = null)
        {
            AsyncOperation asyncLoad;

            if (!isAdditive)
                asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
            else
                asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                onProgressUpdate?.Invoke((asyncLoad.progress / 0.9f));
                yield return null;
            }

            if (asyncLoad.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }
    }
}
