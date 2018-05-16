using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 4f;
    public float m_DefaultX = 0;
    public float m_DefaultY = 24;
    public float m_DefaultZ = -17;
    public float m_DefaultSize = 10;
    public bool m_MoveToDefault = true;
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;
    private Vector3 m_DefaultPosition;
    private bool m_AtDefaultPosition = true;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
        m_DefaultPosition = new Vector3(m_DefaultX, m_DefaultY, m_DefaultZ);
    }

    public bool GetAtDefaultPosition()
    {
        return m_AtDefaultPosition;
    }
    
    public void SetMoveToDefault(bool moveToDefault)
    {
        m_MoveToDefault = moveToDefault;
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
        if (Mathf.Round(m_Camera.orthographicSize) == m_DefaultSize && transform.position == m_DefaultPosition && m_AtDefaultPosition == false)
        {

            StartCoroutine(SetAtDefaultPositionAfterResize());
        }
        else
        {
            m_AtDefaultPosition = false;
        }
    }

    IEnumerator SetAtDefaultPositionAfterResize()
    {
        yield return new WaitForSeconds(m_DampTime);
        m_AtDefaultPosition = true;
    }

    private void Move()
    {
        if (m_MoveToDefault == false)
        {
            FindAveragePosition();
        }
        else
        {
            m_DesiredPosition = m_DefaultPosition;
        }
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }
      
        averagePos.z = transform.position.z; ;
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize;
        if (m_MoveToDefault == false)
        {
            requiredSize = FindRequiredSize();
        }
        else
        {
            requiredSize = m_DefaultSize;
        }
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        if (size > m_DefaultSize)
        {
            size = m_DefaultSize;
        }

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}