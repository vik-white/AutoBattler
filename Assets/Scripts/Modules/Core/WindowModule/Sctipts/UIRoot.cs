using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace vikwhite
{
    public enum UILayer { WORLD, GUI, WINDOW, FLYTEXT, DRAG, POPUP }
    
    public interface IUIRoot
    {
        Vector2 CanvasSize { get; }
        Vector2 CanvasCenter { get; }

        public void Initialize(RectTransform rectTransform);
        RectTransform GetLayer(UILayer layer);
    }
    
    public class UIRoot : IUIRoot
    {
        private const string UICameraName = "UI Camera";
        private const string OverlayCanvasName = "Overlay Canvas";

        private RectTransform _rectTransform;
        private Dictionary<UILayer, RectTransform> _layers;
        public Vector2 CanvasSize => _rectTransform.sizeDelta;
        public Vector2 CanvasCenter => CanvasSize * 0.5f;
        
        public void Initialize(RectTransform rectTransform) {
            _rectTransform = rectTransform;
            var overlayRoot = ConfigureBlurFriendlyCanvas();

            _layers = new Dictionary<UILayer, RectTransform>();
            _layers[UILayer.WORLD] = CreateLayer("WORLD", _rectTransform);
            _layers[UILayer.GUI] = CreateLayer("GUI", _rectTransform);
            _layers[UILayer.WINDOW] = CreateLayer("WINDOW", _rectTransform);
            _layers[UILayer.FLYTEXT] = CreateLayer("FLYTEXT", _rectTransform);
            _layers[UILayer.DRAG] = CreateLayer("DRAG", overlayRoot);
            _layers[UILayer.POPUP] = CreateLayer("POPUP", overlayRoot);
        }

        private RectTransform ConfigureBlurFriendlyCanvas()
        {
            var canvas = _rectTransform.GetComponent<Canvas>();
            if (canvas == null)
                return _rectTransform;

            var uiCamera = GetOrCreateUICamera(canvas);
            if (uiCamera != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = uiCamera;
                canvas.planeDistance = 100;
            }

            return GetOrCreateOverlayCanvas(canvas);
        }

        private Camera GetOrCreateUICamera(Canvas canvas)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return canvas.worldCamera;

            var uiCameraObject = GameObject.Find(UICameraName);
            var uiCamera = uiCameraObject != null
                ? uiCameraObject.GetComponent<Camera>()
                : null;

            if (uiCamera == null)
            {
                var cameraObject = new GameObject(UICameraName);
                uiCamera = cameraObject.AddComponent<Camera>();
            }

            uiCamera.transform.SetParent(null, false);
            uiCamera.transform.position = Vector3.zero;
            uiCamera.transform.rotation = Quaternion.identity;
            uiCamera.transform.localScale = Vector3.one;
            var uiLayerMask = 1 << canvas.gameObject.layer;

            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.cullingMask = uiLayerMask;
            uiCamera.depth = mainCamera.depth + 1;
            uiCamera.nearClipPlane = -100;
            uiCamera.farClipPlane = 1000;
            uiCamera.allowHDR = mainCamera.allowHDR;
            uiCamera.allowMSAA = mainCamera.allowMSAA;
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = 5;

            mainCamera.cullingMask &= ~uiLayerMask;

            var mainCameraData = mainCamera.GetUniversalAdditionalCameraData();
            var uiCameraData = uiCamera.GetUniversalAdditionalCameraData();
            uiCameraData.renderType = CameraRenderType.Overlay;
            uiCameraData.renderPostProcessing = false;

            if (!mainCameraData.cameraStack.Contains(uiCamera))
                mainCameraData.cameraStack.Add(uiCamera);

            var mainSource = mainCamera.GetComponent<TranslucentImageSource>();
            var uiSource = uiCamera.GetComponent<TranslucentImageSource>();
            if (uiSource == null)
                uiSource = uiCamera.gameObject.AddComponent<TranslucentImageSource>();

            if (mainSource != null)
            {
                uiSource.BlurConfig = mainSource.BlurConfig;
                uiSource.Downsample = mainSource.Downsample;
                uiSource.BlurRegion = mainSource.BlurRegion;
                uiSource.maxUpdateRate = mainSource.maxUpdateRate;
            }

            return uiCamera;
        }

        private RectTransform GetOrCreateOverlayCanvas(Canvas sourceCanvas)
        {
            var overlay = GameObject.Find(OverlayCanvasName);
            if (overlay == null)
                overlay = new GameObject(OverlayCanvasName);

            overlay.layer = sourceCanvas.gameObject.layer;

            var overlayRect = overlay.GetComponent<RectTransform>();
            if (overlayRect == null)
                overlayRect = overlay.AddComponent<RectTransform>();

            var overlayCanvas = overlay.GetComponent<Canvas>();
            if (overlayCanvas == null)
                overlayCanvas = overlay.AddComponent<Canvas>();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = sourceCanvas.sortingOrder + 1000;

            var sourceScaler = sourceCanvas.GetComponent<CanvasScaler>();
            if (sourceScaler != null)
            {
                var overlayScaler = overlay.GetComponent<CanvasScaler>();
                if (overlayScaler == null)
                    overlayScaler = overlay.AddComponent<CanvasScaler>();

                overlayScaler.uiScaleMode = sourceScaler.uiScaleMode;
                overlayScaler.referenceResolution = sourceScaler.referenceResolution;
                overlayScaler.screenMatchMode = sourceScaler.screenMatchMode;
                overlayScaler.matchWidthOrHeight = sourceScaler.matchWidthOrHeight;
                overlayScaler.referencePixelsPerUnit = sourceScaler.referencePixelsPerUnit;
                overlayScaler.scaleFactor = sourceScaler.scaleFactor;
                overlayScaler.physicalUnit = sourceScaler.physicalUnit;
                overlayScaler.fallbackScreenDPI = sourceScaler.fallbackScreenDPI;
                overlayScaler.defaultSpriteDPI = sourceScaler.defaultSpriteDPI;
                overlayScaler.dynamicPixelsPerUnit = sourceScaler.dynamicPixelsPerUnit;
            }

            if (overlay.GetComponent<GraphicRaycaster>() == null)
                overlay.AddComponent<GraphicRaycaster>();

            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayRect.localPosition = Vector3.zero;
            overlayRect.localScale = Vector3.one;

            return overlayRect;
        }

        private RectTransform CreateLayer(string name, RectTransform parent) {
            GameObject gameObject = new GameObject(name);
            gameObject.layer = parent.gameObject.layer;
            gameObject.transform.SetParent(parent);
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }
        
        public RectTransform GetLayer(UILayer layer)
        {
            return _layers[layer];
        }
    }
}
