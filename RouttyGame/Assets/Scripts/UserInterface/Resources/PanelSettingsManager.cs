using _Common.BaseClasses;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface.Resources
{
    [CreateAssetMenu(fileName = "PanelSettingsManager", menuName = "Project/UserInterface/PanelSettingsManager")]
    public class PanelSettingsManager : ASingletonResource<PanelSettingsManager>
    {
        protected override string ResourcePath => "UserInterface/PanelSettingsManager";

        [SerializeField]
        private PanelSettings _panelSettings;

        public PanelSettings PanelSettings => _panelSettings;

        /// <summary>
        /// Scales a UI Toolkit coordinate to match screen space, accounting for PanelSettings scaling.
        /// </summary>
        /// <param name="uiPosition">The UI Toolkit coordinate (element.worldBound.position)</param>
        /// <returns>The corresponding screen space coordinate</returns>
        public Vector2 ScaleToScreenSpace(Vector2 uiPosition)
        {
            float referenceWidth = _panelSettings.referenceResolution.x;
            float referenceHeight = _panelSettings.referenceResolution.y;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            var match = _panelSettings.match;

            var scaleFactorWidth = screenWidth / referenceWidth;
            var scaleFactorHeight = screenHeight / referenceHeight;

            var scaleFactor = Mathf.Lerp(scaleFactorWidth, scaleFactorHeight, match);

            var screenX = uiPosition.x * scaleFactor;
            var screenY = uiPosition.y * scaleFactor;

            return new Vector2(screenX, screenY);
        }

        /// <summary>
        /// Scales a UI Toolkit size to match screen space, accounting for PanelSettings scaling.
        /// </summary>
        /// <param name="uiSize">The UI Toolkit size (element.worldBound.size)</param>
        /// <returns>The corresponding screen space size</returns>
        public Vector2 ScaleSizeToScreenSpace(Vector2 uiSize)
        {
            float referenceWidth = _panelSettings.referenceResolution.x;
            float referenceHeight = _panelSettings.referenceResolution.y;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            var match = _panelSettings.match;

            var scaleFactorWidth = screenWidth / referenceWidth;
            var scaleFactorHeight = screenHeight / referenceHeight;

            var scaleFactor = Mathf.Lerp(scaleFactorWidth, scaleFactorHeight, match);

            var screenWidthSize = uiSize.x * scaleFactor;
            var screenHeightSize = uiSize.y * scaleFactor;

            return new Vector2(screenWidthSize, screenHeightSize);
        }

        /// <summary>
        /// Converts a UI Toolkit position to a world position at z = 0.
        /// </summary>
        /// <param name="uiPosition">The position from UI element's worldBound.position</param>
        /// <param name="camera"></param>
        /// <returns>The corresponding world position at z = 0</returns>
        public Vector3 ScreenPositionToWorld(Vector2 uiPosition, Camera camera)
        {
            var adjustedY = Screen.height - uiPosition.y;
        
            var distance = -camera.transform.position.z;
            var screenPosition = new Vector3(uiPosition.x, adjustedY, distance);
        
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0; // camera z is 0
        
            return worldPosition;
        }
    }
}