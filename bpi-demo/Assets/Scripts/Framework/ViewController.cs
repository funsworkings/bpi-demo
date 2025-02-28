using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Abstract;
using Data;
using Elements;
using UnityEngine;
using Views;

namespace Framework
{
    public class ViewController : MonoBehaviour
    {
        // Dependencies
        
        [SerializeField] private ConfigLoader _configLoader;
        
        private ConfigFile _config;

        public LandingView LandingView => Get<LandingView>();
        public ContentView ContentView => Get<ContentView>();
        public ModalView ModalView =>     Get<ModalView>();
        
        // Properties

        [SerializeField] private View[] _views;
        private Dictionary<Type, View> _viewLookup = new Dictionary<Type, View>();

        // Initialisation
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame(); // Slight frame delay

            yield return _configLoader.Load((config) =>
            {
                _config = config;
                Debug.LogWarning("Success load config! :)");

                var contents = _config.Contents;
                foreach (var content in contents)
                {
                    var images = content.Images;
                    foreach (var img in images)
                    {
                        TextureLoader.Instance.LoadTextureFromStreamingPath(img.ImagePath);
                    }
                }

            }, (err) =>
            {
                Debug.LogWarning($"Load config with errors: {err.Message}, fallback to default config");
                _config = default(ConfigFile);
            });

            // Populate views registry
            if (_views != null && _views.Length > 0)
            {
                foreach (var view in _views)
                {
                    var viewType = view.GetType();
                    if (!_viewLookup.ContainsKey(viewType))
                    {
                        Debug.LogWarning($"Add view of type {viewType.FullName} to registry");
                        _viewLookup.Add(viewType, view);
                    }
                }
            }

            yield return LandingView.Setup(this, _config.SlideshowImages);
            yield return ContentView.Setup(this, _config.TimeToDismissApp, new ContentView.ContentData(){ Contents = _config.Contents });
            yield return ModalView.Setup(this, _config.TimeToDismissModal);
            
            LandingView.Show(); // Default show landing view first
            
            ModalButtonOption.OnSelectOption += OnSelectContentOption;
        }

        private void OnDestroy()
        {
            ModalButtonOption.OnSelectOption -= OnSelectContentOption;
        }

        #region Callbacks

        private void OnSelectContentOption(ModalButtonOption option)
        {
            ModalView.OpenWithContent(option.Content);
        }
        
        #endregion

        #region Lookup

        T Get<T>() where T : View
        {
            var type = typeof(T);
            if (_viewLookup.TryGetValue(type, out var view))
            {
                return (T)view;
            }

            throw new SystemException($"Failed to find view of type {type.FullName}");
        }
        
        #endregion
    }
}