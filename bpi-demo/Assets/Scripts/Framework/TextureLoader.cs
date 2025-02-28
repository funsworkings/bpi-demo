using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public class TextureLoader : MonoBehaviour
    {
        public static TextureLoader Instance = null;

        private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

        [SerializeField] private List<Texture2D> _loadedTextures = new List<Texture2D>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;

                var textureCache = _textureCache.ToArray();
                foreach (var tc in textureCache)
                {
                    if (tc.Value != null)
                    {
                        Destroy(tc.Value); // Dispose loaded texture from memory
                    }
                }
            }
        }
        
        #region Ops

        /// <summary>
        /// Loads a given texture path from StreamingAssets folder
        /// </summary>
        public Texture2D LoadTextureFromStreamingPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            path = Path.Combine(Application.streamingAssetsPath, path);
            Debug.LogWarning($"Try load: {path}");
            
            if (_textureCache.TryGetValue(path, out var texture))
            {
                Debug.LogWarning($"Ignore duplicate load for cached texture: {path}");
                return texture;
            }

            try
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                if (bytes == null || bytes.Length == 0)
                {
                    throw new SystemException("Empty byte array found for image path!");
                }
                
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                
                _textureCache.Add(path, tex);
                _loadedTextures.Add(tex);
                
                return tex;
            }
            catch (SystemException err)
            {
                Debug.LogWarning($"Failed to load texture for path: {path}");
                return null;
            }
        }
        
        #endregion
    }
}