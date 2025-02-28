using System;
using System.Collections;
using Abstract;
using Data;
using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ModalView : View
    {
        // Properties

        private ConfigFile.ContentBlock _content;

        private int _contentItemIndex = -1;

        private float _inactiveT = 0f;
        private float TimeToDismiss = float.MaxValue;

        [Header("UI")] 
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private TMP_Text _contentTitleText;
        [SerializeField] private TMP_Text _imageCaptionText;
        [SerializeField] private RawImage _contentImage, _transitionImage;
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
            
            _contentTransform.localScale = Vector3.zero;
            _contentTransform.DOScale(1f, .67f).SetEase(Ease.OutBack);
        }

        protected override void OnHidden()
        {
            _contentTransform.DOScale(0f, .167f).SetEase(Ease.InExpo);
        }

        #endregion
        
        #region Ops

        public void OpenWithContent(ConfigFile.ContentBlock content)
        {
            _content = content;
            
            _contentTitleText.text = content.Title;

            _contentItemIndex = 0;
            GoToImageIndex(0);
            
            Show();
        }

        #endregion

        #region Images

        void GoToPreviousImage() => GoToImageIndex(_contentItemIndex - 1);
        void GoToNextImage() => GoToImageIndex(_contentItemIndex + 1);

        void GoToImageIndex(int index)
        {
            index = (int)Mathf.Repeat(index, _content.Images.Length); // Safe wrap index value within acceptable range

            int cacheItemIndex = _contentItemIndex;
            _contentItemIndex = index;
            
            OnUpdateImageIndex(cacheItemIndex != _contentItemIndex);
        }

        void OnUpdateImageIndex(bool animate)
        {
            var content = _content.Images[_contentItemIndex];
            var image = TextureLoader.Instance.LoadTextureFromStreamingPath(content.ImagePath);
                _contentImage.texture = image;
                _contentImageFitter.aspectRatio = 1f * image.width / image.height;
                _imageCaptionText.text = content.Caption;
        }
        
        #endregion
    }
}