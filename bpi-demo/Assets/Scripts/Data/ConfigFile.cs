using System;

namespace Data
{
    [Serializable]
    public class ConfigFile
    {
        #region Internal

        [Serializable]
        public class ContentBlock
        {
            public string Title;
            public ImageBlock[] Images;
        }

        [Serializable]
        public class ImageBlock
        {
            public string ImagePath;
            public string Caption;
        }
        
        #endregion
        
        // General Settings
        
        /*
         * Supplying default values that can be recreated if config fails to load
         */

        public float TimeToDismissApp = 60f;
        public float TimeToDismissModal = 30f;

        public string[] SlideshowImages;
        
        // Content Settings

        public ContentBlock[] Contents;
    }
}