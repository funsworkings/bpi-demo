using System;
using System.Collections;
using Abstract;
using Data;
using DG.Tweening;
using Elements;
using UnityEngine;

namespace Views
{
    public class ContentView : View
    {
        #region Internal

        [Serializable]
        public struct ContentData
        {
            public ConfigFile.ContentBlock[] Contents;
        }
        
        #endregion
        
        // Properties

        private float _inactiveT = 0f;
        private float TimeToDismiss = float.MaxValue;

        private ModalButtonOption[] _optionButtons;
        
        protected override IEnumerator Initialise(params object[] args)
        {
            yield return null;
            TimeToDismiss = (float) args[0];

            ContentData data = default;
            if (args.Length > 1)
            {
                data = (ContentData) args[1];
            }

            _optionButtons = GetComponentsInChildren<ModalButtonOption>();
            for (int i = 0; i < _optionButtons.Length; i++)
            {
                if (i >= data.Contents.Length)
                {
                    _optionButtons[i].gameObject.SetActive(false); // Disable button with invalid content
                    continue;
                }
                
                var content = (ConfigFile.ContentBlock) data.Contents[i];
                _optionButtons[i].Bind(content);
            }
        }

        protected override void Tick(float dt)
        {
            if (!Input.GetMouseButton(0))
            {
                _inactiveT += dt;
                if (_inactiveT >= TimeToDismiss)
                {
                    _vc.LandingView.Show(); // Revert to landing view after inactive threshold
                    Hide();
                }
            }
            else _inactiveT = 0f;
        }

        protected override void OnVisible()
        {
            _inactiveT = 0f;

            for (int i = 0; i < _optionButtons.Length; i++)
            {
                _optionButtons[i].transform.localScale = Vector3.zero;
                _optionButtons[i].transform.DOScale(Vector3.one, .67f).SetEase(Ease.OutBack).SetDelay(i * .3f);
            }
        }
    }
}