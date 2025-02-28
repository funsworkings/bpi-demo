using System.Collections;
using Abstract;
using UnityEngine;

namespace Views
{
    public class ContentView : View
    {
        // Properties

        private float _inactiveT = 0f;
        private float TimeToDismiss = float.MaxValue;
        
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
                    _vc.LandingView.Show(); // Revert to landing view after inactive threshold
                    Hide();
                }
            }
            else _inactiveT = 0f;
        }

        protected override void OnVisible()
        {
            _inactiveT = 0f;
        }
    }
}