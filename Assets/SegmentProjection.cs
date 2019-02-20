using UnityEngine;

public class SegmentProjection : MonoBehaviour
{
    [SerializeField]
    LineRenderer _line;

    SpriteRenderer _mouse;
    SpriteRenderer _projection;
    SpriteRenderer[] _points;

    void Start ()
    {
        _points = new SpriteRenderer[_line.positionCount];
        var prefab = Resources.Load<SpriteRenderer>("Point");
        for (int i = 0; i < _line.positionCount; i++)
        {
            Vector3 pos = _line.GetPosition(i);
            _points[i] = Instantiate(prefab, pos, Quaternion.identity);
        }

        _mouse = Instantiate(Resources.Load<SpriteRenderer>("Point2"));
        _projection = Instantiate(Resources.Load<SpriteRenderer>("Point3"));
    }

    void Update ()
    {
        Vector3 realPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        realPoint.z = 0;
        _mouse.transform.position = realPoint;

        CalcProjectionAtLine(realPoint);
    }

    void CalcProjectionAtLine(Vector2 realPoint)
    {
        if (_line.positionCount < 2)
            return;

        float minSqrMagnitude = float.MaxValue;
        Vector2 projection = _line.GetPosition(0);
        for (int i = 0; i < _line.positionCount - 1; i++)
        {
            Vector2 a = _line.GetPosition(i);
            Vector2 b = _line.GetPosition(i + 1);

            Vector2 pr;
            float sqrMagnitude;
            if (CheckInsideSegment(realPoint, a, b))
            {
                pr = GetProjection(realPoint, a, b);
                sqrMagnitude = (realPoint - pr).sqrMagnitude;
            }
            else
            {
                float sqrMagnitudeA = (realPoint - a).sqrMagnitude;
                float sqrMagnitudeB = (realPoint - b).sqrMagnitude;
                if (sqrMagnitudeA < sqrMagnitudeB)
                {
                    pr = a;
                    sqrMagnitude = sqrMagnitudeA;
                }
                else
                {
                    pr = b;
                    sqrMagnitude = sqrMagnitudeB;
                }
            }

            if (sqrMagnitude < minSqrMagnitude)
            {
                minSqrMagnitude = sqrMagnitude;
                projection = pr;
            }
        }
        _projection.transform.position = projection;
    }

    static bool CheckInsideSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        float ab = (b - a).sqrMagnitude;
        float bp = (p - b).sqrMagnitude;
        float ap = (p - a).sqrMagnitude;
        return bp <= ab + ap && ap <= ab + bp;
    }

    static Vector2 GetProjection(Vector2 p, Vector2 a, Vector2 b)
    {
        float sqrMagnitude = (b - a).sqrMagnitude;
        if (sqrMagnitude == 0f)
            return a;

        float k = ((b.y - a.y) * (p.x - a.x) - (b.x - a.x) * (p.y - a.y)) / sqrMagnitude;
        return new Vector2(p.x - k * (b.y - a.y), p.y + k * (b.x - a.x));
    }
}