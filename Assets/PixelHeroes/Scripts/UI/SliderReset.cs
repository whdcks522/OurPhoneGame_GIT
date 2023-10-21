using UnityEngine;
using UnityEngine.UI;

namespace Assets.PixelHeroes.Scripts.UI
{
    /// <summary>
    /// Used to set Slider zero value.
    /// </summary>
    public class SliderReset : MonoBehaviour
    {
        public void Reset()
        {
            GetComponentInParent<Slider>().value = 0;
        }
    }
}