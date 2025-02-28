using System;
using System.Collections;
using System.IO;
using Data;
using UnityEngine;

namespace Framework
{
    public class ConfigLoader : MonoBehaviour
    {
        // Properties

        [SerializeField] private ConfigFile _config;

        private string CONFIG_PATH => Path.Join(Application.streamingAssetsPath, "config.json");

        public IEnumerator Load(System.Action<ConfigFile> onSuccess, System.Action<System.Exception> onError)
        {
            yield return null;
            
            var configTxt = System.IO.File.ReadAllText(CONFIG_PATH);
            if (!string.IsNullOrEmpty(configTxt))
            {
                Debug.LogWarning($"Found config file: {configTxt}");

                try
                {
                    var config = JsonUtility.FromJson<ConfigFile>(configTxt);
                    _config = config;
                    
                    onSuccess?.Invoke(_config);
                }
                catch (SystemException err)
                {
                    onError?.Invoke(err);
                }
            }
            else
            {
                onError?.Invoke(new SystemException("Text file for config is empty!"));
            }
        }
    }
}