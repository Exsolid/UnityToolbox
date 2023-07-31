using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// An alternative to the Unity <see cref="Slider"/>. The fill area of this is a cropped instead of scaled image.
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageSliderHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _fillImage;

    private Camera _camera;

    private float _scaledMinPos;
    private float _scaledMaxPos;

    private bool _dragging;

    public bool Enabled;
    private bool _initialized;

    public event Action<float> ValueUpdate;

    private float _scaling;
    private float _prevScaling;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _camera = Camera.main;
        _scaling = _camera.scaledPixelWidth / _fillImage.GetComponentInParent<CanvasScaler>().referenceResolution.x;
        _scaledMinPos = transform.parent.transform.position.x - ((RectTransform)transform.parent.transform).sizeDelta.x / 2 * _scaling + GetComponent<Image>().rectTransform.sizeDelta.x * _scaling;
        _scaledMaxPos = transform.parent.transform.position.x + ((RectTransform)transform.parent.transform).sizeDelta.x / 2 * _scaling - GetComponent<Image>().rectTransform.sizeDelta.x * _scaling;
        _initialized = true;
        _prevScaling = _scaling;
    }

    private void Update()
    {
        _scaling = _camera.scaledPixelWidth / _fillImage.GetComponentInParent<CanvasScaler>().referenceResolution.x;

        _scaledMinPos = transform.parent.transform.position.x - ((RectTransform)transform.parent.transform).sizeDelta.x / 2 * _scaling + GetComponent<Image>().rectTransform.sizeDelta.x * _scaling;
        _scaledMaxPos = transform.parent.transform.position.x + ((RectTransform)transform.parent.transform).sizeDelta.x / 2 * _scaling - GetComponent<Image>().rectTransform.sizeDelta.x * _scaling;

        if (_prevScaling != _scaling)
        {
            float x = _fillImage.fillAmount == 0 ? _scaledMinPos : _scaledMinPos + (_scaledMaxPos - _scaledMinPos) * _fillImage.fillAmount;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        _prevScaling = _scaling;

        if (_dragging && Enabled)
        {
            float x = Mathf.Clamp(Mouse.current.position.ReadValue().x, _scaledMinPos, _scaledMaxPos);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
            _fillImage.fillAmount = (x - _scaledMinPos) == 0 ? 0 : (x - _scaledMinPos) / (_scaledMaxPos - _scaledMinPos);
        }
    }

    /// <summary>
    /// Sets the value of the slider.
    /// </summary>
    /// <param name="value">A value in range of 0-1</param>
    public void SetValue(float value)
    {
        if (!_initialized)
        {
            Init();
        }

        _fillImage.fillAmount = value;
        float x = value == 0 ? _scaledMinPos : _scaledMinPos + (_scaledMaxPos - _scaledMinPos) * value;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _dragging = false;
        ValueUpdate?.Invoke(_fillImage.fillAmount);
    }
}
