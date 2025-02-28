using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elements
{
    [RequireComponent(typeof(Button))]
    public class ModalButtonOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Events

        public static System.Action<ModalButtonOption> OnSelectOption;
        
        // Properties

        private Vector3 cacheScale;
        private Vector3 cacheRotation;

        [SerializeField] private ConfigFile.ContentBlock _content;
        public ConfigFile.ContentBlock Content => _content;

        [SerializeField] private TMP_Text _contentTitleText;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            cacheScale = transform.localScale;
            cacheRotation = transform.localEulerAngles;
            
            _button.onClick.AddListener(OnClickButton);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClickButton);
        }

        public void Bind(ConfigFile.ContentBlock content)
        {
            _content = content;
            _contentTitleText.text = _content.Title;
        }

        #region Callbacks
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            var scale = Random.Range(1.1f, 1.2f);
            transform.DOScale(Vector3.one * scale, .167f);
            transform.DOLocalRotate(
                new Vector3(cacheRotation.x, cacheRotation.y, cacheRotation.z + Random.Range(-15f, 15f)), .167f);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(cacheScale, .2f);
            transform.DOLocalRotate(cacheRotation, .167f);
        }

        private void OnClickButton()
        {
            OnSelectOption?.Invoke(this);
        }
        
        #endregion
    }
}