using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuWheel : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private List<GameObject> _items;
    [SerializeField] private float _radius;
    [SerializeField] private float _timeToMove;
    [SerializeField] private bool _smoothCircle;
    [SerializeField] private bool _switchXY;
    [SerializeField] private float _incline;

    private float _currentTimer;
    private int _index;
    private List<float> _degrees;
    private float _xMin;

    // Start is called before the first frame update
    void Start()
    {
        _xMin = transform.transform.position.x - _radius;
        GameObject closestToCamera = null;
        _degrees = new List<float>();
        float current = 0;
        foreach (GameObject item in _items)
        {
            item.GetComponent<Menu>().IsActive = false;
            _degrees.Add(current);
            if(_switchXY) item.transform.position = new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[_index]), 0, _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[_index]));
            else item.transform.position = new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[_index]), _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[_index]), 0);
            item.transform.position = item.transform.position + new Vector3(0, _incline * (item.transform.position.x - _xMin), 0);
            current += 360f / _items.Count;
            if (closestToCamera == null || 
                Vector3.Distance(closestToCamera.transform.position, _camera.transform.position) > Vector3.Distance(item.transform.position, _camera.transform.position))
                closestToCamera = item;
                _index++;
        }
        closestToCamera.GetComponent<Menu>().IsActive = true;
        _index = _items.IndexOf(closestToCamera);
        ModuleManager.GetModule<UIEventManager>().OnMenuWheelNext += moveNext;
        ModuleManager.GetModule<UIEventManager>().OnMenuWheelPrevious += movePrev;
    }

    private void OnDestroy()
    {
        if (ModuleManager.ModuleRegistered<UIEventManager>())
        {
            ModuleManager.GetModule<UIEventManager>().OnMenuWheelNext -= moveNext;
            ModuleManager.GetModule<UIEventManager>().OnMenuWheelPrevious -= movePrev;
        }
    }

    private void Update()
    {
        if(_currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
        }
    }

    public void moveNext()
    {
        if (_currentTimer > 0 || _items.Count == 0 || !_items[_index].GetComponent<Menu>().IsActive) return;
        _currentTimer = _timeToMove;
        _items[_index].GetComponent<Menu>().IsActive = false;
        if (_index == _items.Count-1) _index = 0;
        else _index++;
        int current = 0;
        foreach (GameObject item in _items)
        {
            float oldValue = _degrees[current];
            _degrees[current] += 360f / _items.Count;

            if (_smoothCircle)
            {
                float step = _radius / _items.Count;
                List<Vector3> smooth = new List<Vector3>();
                while (oldValue + step < _degrees[current])
                {
                    oldValue += step;
                    if(_switchXY) smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * oldValue),0 , _radius * Mathf.Sin(Mathf.Deg2Rad * oldValue)));
                    else smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), _radius * Mathf.Sin(Mathf.Deg2Rad * oldValue), 0));
                }
                if (_switchXY) smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), 0, _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current])));
                else smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current]), 0));

                StartCoroutine(moveToPosition(item, smooth));
            }
            else StartCoroutine(moveToPosition(item, _switchXY ? new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), 0,_radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current])) : new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current]), 0)));
            if (_degrees[current] > 360) _degrees[current] -= 360;
            current++;
        }
    }

    public void movePrev()
    {
        if (_currentTimer > 0 || _items.Count == 0 || !_items[_index].GetComponent<Menu>().IsActive) return;
        _currentTimer = _timeToMove;
        _items[_index].GetComponent<Menu>().IsActive = false;
        if (_index == 0) _index = _items.Count -1;
        else _index--;
        int current = 0;
        foreach (GameObject item in _items)
        {
            float oldValue = _degrees[current];
            _degrees[current] -= 360f / _items.Count;

            if (_smoothCircle)
            {
                float step = _radius / _items.Count;
                List<Vector3> smooth = new List<Vector3>();
                while (oldValue - step > _degrees[current])
                {
                    oldValue -= step;
                    if (_switchXY) smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), 0, _radius * Mathf.Sin(Mathf.Deg2Rad * oldValue)));
                    else smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), _radius * Mathf.Sin(Mathf.Deg2Rad * oldValue), 0));
                }
                if (_switchXY) smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), 0, _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current])));
                else smooth.Add(new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current]), 0));

                StartCoroutine(moveToPosition(item, smooth));
            }
            else StartCoroutine(moveToPosition(item, _switchXY ? new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), 0, _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current])) : new Vector3(_radius * Mathf.Cos(Mathf.Deg2Rad * _degrees[current]), _radius * Mathf.Sin(Mathf.Deg2Rad * _degrees[current]), 0)));
            if (_degrees[current] < 0) _degrees[current] += 360;
            current++;
        }
    }

    public IEnumerator moveToPosition(GameObject item, Vector3 newPos)
    {
        float runtime = 0;
        Vector3 startPos = item.transform.position;
        while (runtime < _timeToMove)
        {
            runtime += Time.deltaTime;
            item.transform.position = Vector3.Lerp(startPos, newPos + new Vector3(0, _incline * (newPos.x - _xMin), 0), (runtime/_timeToMove));
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator moveToPosition(GameObject item, List<Vector3> newPos)
    {
        float timeToMoveForEachStep = _timeToMove/newPos.Count;
        Vector3 pastPos = item.transform.position;
        foreach(Vector3 pos in newPos)
        {
            float runtime = 0;
            while (runtime < timeToMoveForEachStep)
            {
                runtime += Time.deltaTime;
                item.transform.position = Vector3.Lerp(pastPos, pos + new Vector3(0, _incline * (pos.x-_xMin), 0), (runtime / timeToMoveForEachStep));
                pastPos = pos + new Vector3(0, _incline * (pos.x - _xMin));
                yield return new WaitForEndOfFrame();
            }
        }
        if(item.Equals(_items[_index]))
            _items[_index].GetComponent<Menu>().IsActive = true;
    }
}
