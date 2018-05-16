using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float _dampTime = 0.2f;                 
    public float _screenEdgeBuffer = 4f;           
    public float _minSize = 4f;
    public float _defaultX = 0;
    public float _defaultY = 24;
    public float _defaultZ = -17;
    public float _defaultSize = 10;
    public bool _moveToDefault = true;
    [HideInInspector] public Transform[] _targets; 


    private Camera _camera;                        
    private float _zoomSpeed;                      
    private Vector3 _moveVelocity;                 
    private Vector3 _desiredPosition;
    private Vector3 _defaultPosition;
    private bool _atDefaultPosition = true;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _defaultPosition = new Vector3(_defaultX, _defaultY, _defaultZ);
    }

    public bool GetAtDefaultPosition()
    {
        return _atDefaultPosition;
    }
    
    public void SetMoveToDefault(bool moveToDefault)
    {
        _moveToDefault = moveToDefault;
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
        if (Mathf.Round(_camera.orthographicSize) == _defaultSize && transform.position == _defaultPosition && _atDefaultPosition == false)
        {

            StartCoroutine(SetAtDefaultPositionAfterResize());
        }
        else
        {
            _atDefaultPosition = false;
        }
    }

    IEnumerator SetAtDefaultPositionAfterResize()
    {
        yield return new WaitForSeconds(_dampTime);
        _atDefaultPosition = true;
    }

    private void Move()
    {
        if (_moveToDefault == false)
        {
            FindAveragePosition();
        }
        else
        {
            _desiredPosition = _defaultPosition;
        }
        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, _dampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < _targets.Length; i++)
        {
            if (!_targets[i].gameObject.activeSelf)
                continue;

            averagePos += _targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }
      
        averagePos.z = transform.position.z; ;
        averagePos.y = transform.position.y;

        _desiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize;
        if (_moveToDefault == false)
        {
            requiredSize = FindRequiredSize();
        }
        else
        {
            requiredSize = _defaultSize;
        }
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, _dampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(_desiredPosition);

        float size = 0f;

        for (int i = 0; i < _targets.Length; i++)
        {
            if (!_targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(_targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / _camera.aspect);
        }
        
        size += _screenEdgeBuffer;

        size = Mathf.Max(size, _minSize);

        if (size > _defaultSize)
        {
            size = _defaultSize;
        }

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = _desiredPosition;

        _camera.orthographicSize = FindRequiredSize();
    }
}