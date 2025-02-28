using System.Collections;
using Abstract;
using Unity.VisualScripting;
using UnityEngine;

namespace Views
{
    public class LandingView : View
    {
        protected override IEnumerator Initialise(params object[] args)
        {
            yield break;
        }

        protected override void Tick(float dt)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _vc.ContentView.Show();
                Hide();
            }
        }
    }
}