using System.Collections;
using _Common.Interfaces.Combat;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Common.UserInterface
{
    [ExecuteAlways]
    public class BoxHealthBar : ImmediateModeShapeDrawer, IHealthBar
    {
        [SerializeField]
        private float _smoothing = 5f;

        [SerializeField]
        private float _delaySmoothing = 5f;

        [SerializeField]
        private float _delay = 1f;

        [SerializeField]
        private float _width = 1.5f;
        
        [SerializeField]
        private float _height = .2f;

        [SerializeField, Range(0f, 1f)]
        private float _hpPercentage = .33f;

        [SerializeField, Range(0f, 1f)]
        private float _hpDelayPercentage = .5f;

        private float _targetPercentage;
        private float _targetDelayPercentage;

        private bool _awaitingDelay = false;
        private Transform _transform;

        private float _maxHealth;
        private float _currentHealth;

        public override void OnEnable()
        {
            base.OnEnable();
            _transform = transform;
        }

        public override void DrawShapes(Camera cam)
        {
            if (_hpPercentage >= 1f)
            {
                return;
            }

            var targetPosition = _transform.position + Vector3.left * _width / 2f;
            var currentRotation = _transform.rotation;

            var borderThickness = .05f;
            var innerWidth = _width - borderThickness * 2f;
            var innerHeight = _height - borderThickness * 2f;
            var innerTargetPosition = targetPosition + Vector3.one * borderThickness;

            using (Draw.Command(cam))
            {
                Draw.Rectangle(innerTargetPosition, currentRotation,
                    innerWidth, 
                    innerHeight, 
                    RectPivot.Corner, Color.red);

                Draw.Rectangle(innerTargetPosition, currentRotation,
                    innerWidth * _hpDelayPercentage, 
                    innerHeight,
                    RectPivot.Corner, Color.white);

                Draw.Rectangle(innerTargetPosition, currentRotation,
                    innerWidth * _hpPercentage,
                    innerHeight,
                    RectPivot.Corner, Color.green);

                Draw.RectangleBorder(targetPosition, currentRotation, _width, _height, 
                    RectPivot.Corner, borderThickness, Color.black);
            }
        }

        private void Update()
        {
            _hpPercentage = Mathf.Lerp(_hpPercentage, _targetPercentage, Time.deltaTime * _smoothing);
            _hpDelayPercentage =
                Mathf.Lerp(_hpDelayPercentage, _targetDelayPercentage, Time.deltaTime * _delaySmoothing);
        }

        private void UpdateHealthPercentage(float percentage)
        {
            _targetPercentage = percentage;

            if (!_awaitingDelay)
                StartCoroutine(UpdateDelay());
        }

        private IEnumerator UpdateDelay()
        {
            _awaitingDelay = true;
            yield return new WaitForSeconds(_delay);

            _awaitingDelay = false;
            _targetDelayPercentage = _targetPercentage;
        }

        [Button]
        public void SetMaxHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
        }

        [Button]
        public void SetCurrentHealth(float currentHealth)
        {
            _currentHealth = currentHealth;
            UpdateHealthPercentage(_currentHealth / _maxHealth);
        }
        
        public void Setup(float maxHealth, float currentHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;
            _hpPercentage = _currentHealth / _maxHealth;
            UpdateHealthPercentage(_currentHealth / _maxHealth);
        }
    }
}