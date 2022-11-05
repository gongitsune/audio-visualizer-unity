using Config;
using Lasp;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Audios
{
    public class AudioBinder : MonoBehaviour
    {
        [SerializeField] private SpectrumAnalyzer spectrum;
        [SerializeField] private AudioLevelTracker levelTrackerHigh, levelTrackerMid, levelTrackerLow, levelTrackerBase;
        [SerializeField] private VisualEffect effect;

        private Vector4VfxProp _colorProp;
        private ConfigUI _conf;
        private float _currentHue;
        private FloatVfxProp _sizeProp, _amplitudeProp,_numOfParticle;

        private void Start()
        {
            _sizeProp = new FloatVfxProp(effect, "Size");
            _amplitudeProp = new FloatVfxProp(effect, "Amplitude");
            _colorProp = new Vector4VfxProp(effect, "Color");
            _numOfParticle = new FloatVfxProp(effect, "SpawnRate");
            
            _conf = ConfigUI.Instance;
            _conf.OnUpdate.Subscribe(_ =>
            {
                levelTrackerHigh.dynamicRange = _conf.Sensitivity;
                levelTrackerMid.dynamicRange = _conf.Sensitivity;
                levelTrackerLow.dynamicRange = _conf.Sensitivity;
                levelTrackerBase.dynamicRange = _conf.Sensitivity;

                _numOfParticle.Value = (int)_conf.NumOfParticle;
            }).AddTo(this);
        }

        private void Update()
        {
            var baseLevel = levelTrackerBase.normalizedLevel;

            _sizeProp.Value = math.remap(0, 1, _conf.MinSize, _conf.MaxSize, baseLevel);
            _amplitudeProp.Value = math.remap(0, 1, _conf.MinAmplitude, _conf.MaxAmplitude, baseLevel);

            _currentHue += _conf.ColorSpeed * Time.deltaTime;
            if (_currentHue > 1) _currentHue = 0;
            var color = Color.HSVToRGB(_currentHue, _conf.Saturation, _conf.Brightness);
            _colorProp.Value = new Vector4(color.r, color.g, color.b, 1);
        }
    }
}