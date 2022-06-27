using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomMenuItem : MonoBehaviour
{
    [SerializeField] private GameObject _objectToMove;
    [SerializeField] private Transform _zoomPos;
    private Vector3 _originalPos;

    public void ZoomIn()
    {
        _originalPos = new Vector3(_objectToMove.transform.position.x, _objectToMove.transform.position.y, _objectToMove.transform.position.z);
        StartCoroutine(Zoom(_zoomPos.position, 1f));
    }

    public void ZoomOut()
    {
         StartCoroutine(Zoom(_originalPos, 1f));
    }

    IEnumerator Zoom(Vector3 newPos, float time)
    {
        float currentTime = 0;
        Vector3 currentPos = new Vector3(_objectToMove.transform.position.x, _objectToMove.transform.position.y, _objectToMove.transform.position.z);
        while (time > currentTime)
        {
            currentTime += Time.deltaTime;
            _objectToMove.transform.position = Vector3.Lerp(currentPos, newPos, currentTime/time);
            yield return null;
        }
    }
}
