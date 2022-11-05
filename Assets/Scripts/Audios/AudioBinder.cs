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
        private float _currentHue, _lowElapsed;

        private Rigidbody _effectRigid;
        private FloatVfxProp _sizeProp, _amplitudeProp, _numOfParticle;

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

            effect.TryGetComponent(out _effectRigid);
        }

        private void Update()
        {
            var baseLevel = levelTrackerBase.normalizedLevel;

            _sizeProp.Value = math.remap(0, 1, _conf.MinSize, _conf.MaxSize, baseLevel);
            _amplitudeProp.Value = math.remap(0, 1, _conf.MinAmplitude, _conf.MaxAmplitude, baseLevel);

            if (levelTrackerHigh.normalizedLevel > 0.8f)
                _effectRigid.AddTorque(0, baseLevel * 0.5f, 0, ForceMode.Impulse);
            if (levelTrackerMid.normalizedLevel > 0.8f)
                _effectRigid.AddTorque(baseLevel * 0.5f, 0, 0, ForceMode.Impulse);
            if (levelTrackerLow.normalizedLevel > 0.8f)
                _effectRigid.AddTorque(0, 0, baseLevel * 0.5f, ForceMode.Impulse);

            _currentHue += _conf.ColorSpeed * Time.deltaTime;
            if (_currentHue > 1) _currentHue = 0;
            var color = Color.HSVToRGB(_currentHue, _conf.Saturation, _conf.Brightness);
            _colorProp.Value = new Vector4(color.r, color.g, color.b, 1);

            if (baseLevel < 0.1f)
            {
                _lowElapsed += Time.deltaTime;
                if (_lowElapsed < 0.2f) return;
                _lowElapsed = 0;
                levelTrackerBase.ResetAutoGain();
                levelTrackerHigh.ResetAutoGain();
                levelTrackerMid.ResetAutoGain();
                levelTrackerLow.ResetAutoGain();
            }
            else
            {
                _lowElapsed = 0;
            }
        }
    }
}