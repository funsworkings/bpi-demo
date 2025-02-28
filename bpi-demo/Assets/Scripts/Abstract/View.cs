using System;
using System.Collections;
using Framework;
using UnityEngine;

namespace Abstract
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour
    {
        // Dependencies

        protected ViewController _vc;
        
        // Properties

        private CanvasGroup _canvasGroup;

        [SerializeField] private bool _defaultIsVisible = false;

        public bool IsVisible { get; private set; } = false;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected void Start()
        {
            if (_defaultIsVisible) Show();
            else Hide();
            
            AddCallbacks();
        }

        private void Update()
        {
            if (!IsVisible) return; // Ignore update requests when not visible, reduce update ops
            
            float deltaTime = Time.deltaTime;
            Tick(deltaTime);
        }

        protected void OnDestroy()
        {
            RemoveCallbacks();
        }

        public IEnumerator Setup(ViewController vc, params object[] args)
        {
            _vc = vc;
            return Initialise(args);
        }
        
        #region Virtual methods

        protected virtual void Tick(float dt){}
        
        protected virtual void AddCallbacks(){}
        protected virtual void RemoveCallbacks(){}
        
        protected virtual void OnVisible(){}
        protected virtual void OnHidden(){}
        
        #endregion
        
        #region Abstract methods

        protected abstract IEnumerator Initialise(params object[] args);
        
        #endregion
        
        #region Visibility

        public void Show()
        {
            IsVisible = true;
            UpdateCanvasGroupFromVisibility();
            OnVisible();
        }

        public void Hide()
        {
            IsVisible = false;
            UpdateCanvasGroupFromVisibility();
            OnHidden();
        }

        public void Toggle(bool visible)
        {
            if(visible) Show();
            else Hide();
        }

        void UpdateCanvasGroupFromVisibility()
        {
            _canvasGroup.alpha = (IsVisible) ? 1f : 0f;
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = IsVisible;
        }
        
        #endregion
    }
}