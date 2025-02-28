using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    [RequireComponent(typeof(Button))]
    public class ModalButtonOption : MonoBehaviour
    {
        // Events

        public static System.Action<ModalButtonOption> OnSelectOption;
        
        // Properties

        [SerializeField] private ModalContent _content;
        public ModalContent Content => _content;

        [SerializeField] private TMP_Text _contentTitleText;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _contentTitleText.text = _content.Title;
            _button.onClick.AddListener(OnClickButton);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClickButton);
        }

        #region Callbacks

        private void OnClickButton()
        {
            OnSelectOption?.Invoke(this);
        }
        
        #endregion
    }
}