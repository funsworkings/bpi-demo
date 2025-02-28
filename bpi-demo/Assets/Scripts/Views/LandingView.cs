using System.Collections;
using System.Collections.Generic;
using System.IO;
using Abstract;
using DG.Tweening;
using Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Views
{
    public class LandingView : View
    {
        // Properties

        [SerializeField] private RectTransform _callToActionTransform;
        [SerializeField] private RawImage _imageA, _imageB;
        [SerializeField] private AspectRatioFitter _fitterA, _fitterB;
        [SerializeField] private CanvasGroup _canvasGroupA, _canvasGroupB;

        [SerializeField] private Texture2D[] _options;
        private int _optionIndex = 0;

        // Attributes

        [SerializeField] private float _slideDuration = 2f;
        [SerializeField] private float _transitionDuration = 1f;
        
        
        protected override IEnumerator Initialise(params object[] args)
        {
            List<Texture2D> options = new List<Texture2D>();
            
            int imageCount = args.Length;
            for (int i = 0; i < imageCount; i++)
            {
                var imagePath = (string)args[i]; 
                var imageTexture = TextureLoader.Instance.LoadTextureFromStreamingPath(imagePath);

                if (imageTexture != null)
                {
                    options.Add(imageTexture);
                }
            }

            _options = options.ToArray();

            _callToActionTransform.localEulerAngles = new Vector3(0f, 0f, -15f);
            _callToActionTransform.DOLocalRotate(new Vector3(0f, 0f, 15f), 1f).SetLoops(-1, LoopType.Yoyo);

            yield return null;
        }

        protected override void Tick(float dt)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _vc.ContentView.Show();
                Hide();
            }
        }

        protected override void OnVisible()
        {
            _optionIndex = 0;
            _imageA.texture = _options[_optionIndex];
            _fitterA.aspectRatio = 1f * _imageA.texture.width / _imageA.texture.height;
            _canvasGroupA.alpha = 1f;
            _canvasGroupB.alpha = 0f;

            StartCoroutine(SlideRoutine());
        }

        protected override void OnHidden()
        {
            StopAllCoroutines();
        }
        
        #region Slideshow

        IEnumerator SlideRoutine()
        {
            float t = 0f;
            
            while (true)
            {
                t += Time.deltaTime;
                if (t >= _slideDuration)
                {
                    int aIndex = _optionIndex;
                    int bIndex = (int) Mathf.Repeat(_optionIndex + 1, _options.Length);
                    
                    _imageA.texture = _options[aIndex];
                    _canvasGroupA.alpha = 1f;
                    _fitterA.aspectRatio = 1f * _imageA.texture.width / _imageA.texture.height;
                    _imageB.texture = _options[bIndex];
                    _canvasGroupB.alpha = 0f;
                    _fitterB.aspectRatio = 1f * _imageB.texture.width / _imageB.texture.height;

                    yield return _canvasGroupB.DOFade(1f, _transitionDuration);

                    _optionIndex = bIndex;
                    t = 0f;
                }
                
                yield return null;
            }
        }
        
        #endregion
    }
}