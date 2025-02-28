using System;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "New Modal Content", menuName = "Custom/Modal Content", order = 0)]
    public class ModalContent : ScriptableObject
    {
        #region Internal
        
        [Serializable]
        public struct Content
        {
            public Sprite Image;
            public string Caption;
        }
        
        #endregion

        [SerializeField] private string _title;
        public string Title => _title;

        [SerializeField] private Content[] _data;
        public Content[] Data => _data;
    }
}