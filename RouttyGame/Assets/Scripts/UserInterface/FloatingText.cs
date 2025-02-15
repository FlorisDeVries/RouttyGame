using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class FloatingText : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("How far (in world units) the text will travel.")]
        public float moveDistance = 5f;

        [Tooltip("Duration of the movement animation.")]
        public float moveDuration = 1.2f;

        [Tooltip("Duration of the fade animation.")]
        public float fadeDuration = 1.2f;

        private TextMeshProUGUI _textMesh;

        private void Awake()
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Initializes the floating text with the specified text, color, and font size,
        /// then starts the movement and fading animations.
        /// </summary>
        public void Setup(string text, Color color, int fontSize)
        {
            _textMesh.text = text;
            _textMesh.color = color;
            _textMesh.fontSize = fontSize;

            AnimateFloating();
        }

        private void AnimateFloating()
        {
            var randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f).normalized;
            var targetPos = transform.position + randomDir * moveDistance;

            transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutCubic);

            DOTween.To(
                    () => _textMesh.color.a, x =>
                    {
                        var color = _textMesh.color;
                        color.a = x;
                        _textMesh.color = color;
                    },
                    0f, fadeDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}