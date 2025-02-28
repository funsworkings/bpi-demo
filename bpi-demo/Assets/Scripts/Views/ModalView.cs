using System;
using System.Collections;
using Abstract;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ModalView : View
    {
        // Properties

        private ModalContent _content;

        private int _contentItemIndex = -1;

        private float _inactiveT = 0f;
        private float TimeToDismiss = float.MaxValue;

        [Header("UI")] 
        [SerializeField] private TMP_Text _contentTitleText;
        [SerializeField] private TMP_Text _imageCaptionText;
        [SerializeField] private Image _contentImage;
        [SerializeField] private AspectRatioFitter _contentImageFitter;
        [SerializeField] private Button _previousNavButton, _nextNavButton, _closeButton;
        
        
        protected override IEnumerator Initialise(params object[] args)
        {
            yield return null;

            TimeToDismiss = (float) args[0];
        }

        protected override void Tick(float dt)
        {
            if (!Input.GetMouseButton(0))
            {
                _inactiveT += dt;
                if (_inactiveT >= TimeToDismiss)
                {
                    Hide(); // Hide view after inactive for too long
                }
            }
            else _inactiveT = 0f;
        }

        #region View

        protected override void AddCallbacks()
        {
            _previousNavButton.onClick.AddListener(GoToPreviousImage);
            _nextNavButton.onClick.AddListener(GoToNextImage);
            _closeButton.onClick.AddListener(Hide);
        }

        protected override void RemoveCallbacks()
        {
            _previousNavButton.onClick.RemoveListener(GoToPreviousImage);
            _nextNavButton.onClick.RemoveListener(GoToNextImage);
            _closeButton.onClick.RemoveListener(Hide);
        }

        protected override void OnVisible()
        {
            _inactiveT = 0f; // Reset timer for inactivity
        }

        #endregion
        
        #region Ops

        public void OpenWithContent(ModalContent content)
        {
            _content = content;
            
            _contentTitleText.text = content.Title;
            GoToImageIndex(0);
            
            Show();
        }

        #endregion

        #region Images

        void GoToPreviousImage() => GoToImageIndex(_contentItemIndex - 1);
        void GoToNextImage() => GoToImageIndex(_contentItemIndex + 1);

        void GoToImageIndex(int index)
        {
            index = (int)Mathf.Repeat(index, _content.Data.Length); // Safe wrap index value within acceptable range
            _contentItemIndex = index;
            
            OnUpdateImageIndex();
        }

        void OnUpdateImageIndex()
        {
            var content = _content.Data[_contentItemIndex];
            var image = content.Image;
                _contentImage.sprite = image;
            var imageSize = image.bounds.size;
                _contentImageFitter.aspectRatio = 1f * imageSize.x / imageSize.y;
                _imageCaptionText.text = content.Caption;
        }
        
        #endregion
    }
}