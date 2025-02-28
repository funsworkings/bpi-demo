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

        private float _contentWidth;
        private Sequence _contentAnimateSequence;

        [Header("UI")] 
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private TMP_Text _contentTitleText;
        [SerializeField] private TMP_Text _imageCaptionText;
        [SerializeField] private RawImage _contentImage, _cacheImage;
        [SerializeField] private AspectRatioFitter _contentImageFitter, _cacheImageFitter;
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

            _contentWidth = _contentTransform.rect.width;
            
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

        void GoToPreviousImage() => GoToImageIndex(_contentItemIndex - 1, -1);
        void GoToNextImage() => GoToImageIndex(_contentItemIndex + 1, 1);

        void GoToImageIndex(int index, int direction = 0)
        {
            if (_contentAnimateSequence != null) return;  // Ignore request to jump to new item if still animating
            index = (int)Mathf.Repeat(index, _content.Images.Length); // Safe wrap index value within acceptable range

            int cacheItemIndex = _contentItemIndex;
            _contentItemIndex = index;

            if (cacheItemIndex == _contentItemIndex) cacheItemIndex = -1;
            OnUpdateImageIndex(direction, cacheItemIndex);
        }

        void OnUpdateImageIndex(int direction, int cacheItemIndex = -1)
        {
            var content = _content.Images[_contentItemIndex];
                _imageCaptionText.text = content.Caption;
            var image = TextureLoader.Instance.LoadTextureFromStreamingPath(content.ImagePath);
                UpdateImageContent(image, _contentImage, _contentImageFitter);

            if (cacheItemIndex >= 0) // Has valid cached index to animate from
            {
                _cacheImage.gameObject.SetActive(true);
                
                var cacheImage =
                    TextureLoader.Instance.LoadTextureFromStreamingPath(_content.Images[cacheItemIndex].ImagePath);
                UpdateImageContent(cacheImage, _cacheImage, _cacheImageFitter);

                _contentImage.transform.localPosition = new Vector3(_contentWidth * direction, 0f, 0f); // Offset content transform
                _cacheImage.transform.localPosition = Vector3.zero;

                _contentAnimateSequence = DOTween.Sequence();
                _contentAnimateSequence
                    .Append(_contentImage.transform.DOLocalMoveX(0f, .33f).SetEase(Ease.Linear))
                    .OnComplete(
                    () =>
                    {
                        _cacheImage.gameObject.SetActive(false); // Hide cached image
                        _contentAnimateSequence = null;
                    });
                
                _cacheImage.transform.DOLocalMoveX(_contentWidth * -direction, .33f).SetEase(Ease.Linear);
            }
            else
            {
                _cacheImage.gameObject.SetActive(false);
            }
        }

        void UpdateImageContent(Texture2D image, RawImage tImage, AspectRatioFitter tFitter)
        {
            tImage.texture = image;
            tFitter.aspectRatio = 1f * image.width / image.height;
        }
        
        #endregion
    }
}