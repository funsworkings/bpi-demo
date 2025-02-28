using System;
using System.Collections;
using System.Collections.Generic;
using Abstract;
using Elements;
using UnityEngine;
using Views;

namespace Framework
{
    public class ViewController : MonoBehaviour
    {
        
        // Dependencies

        public LandingView LandingView => Get<LandingView>();
        public ContentView ContentView => Get<ContentView>();
        public ModalView ModalView =>     Get<ModalView>();
        
        // Properties

        [SerializeField] private View[] _views;
        private Dictionary<Type, View> _viewLookup = new Dictionary<Type, View>();

        private const float TIME_TO_DISMISS_APP = 60f;
        private const float TIME_TO_DISMISS_MODAL = 30f;

        // Initialisation
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame(); // Slight frame delay

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

            yield return LandingView.Setup(this, null);
            yield return ContentView.Setup(this, TIME_TO_DISMISS_APP);
            yield return ModalView.Setup(this, TIME_TO_DISMISS_MODAL);
            
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