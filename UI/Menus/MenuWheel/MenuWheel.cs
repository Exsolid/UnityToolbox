using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWheel : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    [SerializeField] private float radius;
    [SerializeField] private float timeToMove;
    [SerializeField] private bool smoothCircle;
    [SerializeField] private bool switchXY;
    [SerializeField] private float incline;

    private float currentTimer;
    private int index;
    private List<float> degrees;
    private float xMin;

    // Start is called before the first frame update
    void Start()
    {
        xMin = transform.transform.position.x - radius;

        degrees = new List<float>();
        float current = 0;
        foreach (GameObject item in items)
        {
            degrees.Add(current);
            if(switchXY) item.transform.position = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[index]), 0, radius * Mathf.Sin(Mathf.Deg2Rad * degrees[index]));
            else item.transform.position = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[index]), radius * Mathf.Sin(Mathf.Deg2Rad * degrees[index]), 0);
            item.transform.position = item.transform.position + new Vector3(0, incline * (item.transform.position.x - xMin), 0);
            current += 360f / items.Count;
            index++;
        }

        ModulManager.get<UIEventManager>().menuWheelNext += moveNext;
        ModulManager.get<UIEventManager>().menuWheelPrevious += movePrev;
    }
    private void Update()
    {
        if(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
    }

    public void moveNext()
    {
        if (currentTimer > 0 || items.Count == 0) return;
        currentTimer = timeToMove;
        if (index == items.Count-1) index = 0;
        else index++;
        int current = 0;
        foreach (GameObject item in items)
        {
            float oldValue = degrees[current];
            degrees[current] += 360f / items.Count;

            if (smoothCircle)
            {
                float step = radius / items.Count;
                List<Vector3> smooth = new List<Vector3>();
                while (oldValue + step < degrees[current])
                {
                    oldValue += step;
                    if(switchXY) smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * oldValue),0 , radius * Mathf.Sin(Mathf.Deg2Rad * oldValue)));
                    else smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), radius * Mathf.Sin(Mathf.Deg2Rad * oldValue), 0));
                }
                if (switchXY) smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), 0, radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current])));
                else smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current]), 0));

                StartCoroutine(moveToPosition(item, smooth));
            }
            else StartCoroutine(moveToPosition(item, switchXY ? new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), 0,radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current])) : new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current]), 0)));
            if (degrees[current] > 360) degrees[current] -= 360;
            current++;
        }
    }

    public void movePrev()
    {
        if (currentTimer > 0 || items.Count == 0) return;
        currentTimer = timeToMove;
        if (index < 0) index = items.Count;
        else index--;
        int current = 0;
        foreach (GameObject item in items)
        {
            float oldValue = degrees[current];
            degrees[current] -= 360f / items.Count;

            if (smoothCircle)
            {
                float step = radius / items.Count;
                List<Vector3> smooth = new List<Vector3>();
                while (oldValue - step > degrees[current])
                {
                    oldValue -= step;
                    if (switchXY) smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), 0, radius * Mathf.Sin(Mathf.Deg2Rad * oldValue)));
                    else smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * oldValue), radius * Mathf.Sin(Mathf.Deg2Rad * oldValue), 0));
                }
                if (switchXY) smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), 0, radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current])));
                else smooth.Add(new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current]), 0));

                StartCoroutine(moveToPosition(item, smooth));
            }
            else StartCoroutine(moveToPosition(item, switchXY ? new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), 0, radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current])) : new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * degrees[current]), radius * Mathf.Sin(Mathf.Deg2Rad * degrees[current]), 0)));
            if (degrees[current] < 0) degrees[current] += 360;
            current++;
        }
    }

    public IEnumerator moveToPosition(GameObject item, Vector3 newPos)
    {
        float runtime = 0;
        Vector3 startPos = item.transform.position;
        while (runtime < timeToMove)
        {
            runtime += Time.deltaTime;
            item.transform.position = Vector3.Lerp(startPos, newPos + new Vector3(0, incline * (newPos.x - xMin), 0), (runtime/timeToMove));
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator moveToPosition(GameObject item, List<Vector3> newPos)
    {
        float timeToMoveForEachStep = timeToMove/newPos.Count;
        Vector3 pastPos = item.transform.position;
        foreach(Vector3 pos in newPos)
        {
            float runtime = 0;
            while (runtime < timeToMoveForEachStep)
            {
                runtime += Time.deltaTime;
                item.transform.position = Vector3.Lerp(pastPos, pos + new Vector3(0, incline * (pos.x-xMin), 0), (runtime / timeToMoveForEachStep));
                pastPos = pos + new Vector3(0, incline * (pos.x - xMin));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
