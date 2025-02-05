using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    [SerializeField] public List<Transform> joints;
    public Transform endEffector;

    public Transform target;

    private float costFunction;

    private List<Vector3> distances;

    public float alpha;
    public float initAlpha;

    private Vector6 theta; 

    public float tolerance = 1f;

    private Vector6 gradient;

    private bool canMove;
    private Vector3[] initPosition;
    private float resetProgress = 0f;
    public float resetDuration = 1f;

    public ArmController armController;
    public bool firtsArm;

    void Start()
    {
        distances = new List<Vector3>();

        for(int i = 1; i < joints.Count; i++)
        {
            distances.Add(joints[i].position - joints[i - 1].position);
        }

        theta = Vector6.zero;

        costFunction = Vector3.Distance(endEffector.position, target.position) * Vector3.Distance(endEffector.position, target.position);

        initPosition = new Vector3[6];

        for(int i = 1; i < joints.Count; i++)
        {
            initPosition[i - 1] = joints[i].localPosition;
        }

        initAlpha = alpha;
    }

    void Update()
    {
        if (canMove)
        {
            if (costFunction > tolerance)
            {
                gradient = GetGradient(theta);
                theta -= alpha * gradient;
                Vector3[] newPosition = new Vector3[6];
                newPosition = endFactorFunction(theta);

                float distanceToTarget = Vector3.Distance(armController.transform.position, target.position);
                float distanceFactor = Mathf.Clamp01(distanceToTarget / armController.GetDistance());

                for(int i = 1; i < joints.Count; i++)
                {
                    joints[i].position = Vector3.Lerp(newPosition[i - 1], joints[i].position, 1 - distanceFactor);
                }
            }

            costFunction = lossCostFunction(theta);
        }
        else
        {
            ResetToInitialPositions();
        }
    }

    Vector3[] endFactorFunction(Vector6 theta)
    {
        Quaternion baseRotation = joints[0].rotation;

        Quaternion[] q = new Quaternion[6];
        q[0] = baseRotation * Quaternion.AngleAxis(theta.x, Vector3.up);
        q[1] = Quaternion.AngleAxis(theta.y, Vector3.forward);
        q[2] = Quaternion.AngleAxis(theta.z, Vector3.up);
        q[3] = Quaternion.AngleAxis(theta.w, Vector3.forward);
        q[4] = Quaternion.AngleAxis(theta.v, Vector3.up);
        q[5] = Quaternion.AngleAxis(theta.u, Vector3.forward);

        Vector3 j1 = joints[0].position + q[0] * q[1] * distances[0];
        Vector3 j2 = j1 + q[0] * q[1] * q[2] * distances[1];
        Vector3 j3 = j2 + q[0] * q[1] * q[2] * q[3] * distances[2]; 
        Vector3 j4 = j3 + q[0] * q[1] * q[2] * q[3] * q[4] * distances[3]; 
        Vector3 j5 = j4 + q[0] * q[1] * q[2] * q[3] * q[4] * q[5] * distances[4];
        Vector3 endfactor = j5 + q[0] * q[1] * q[2] * q[3] * q[4] * q[5] * distances[5];

        Vector3[] result = new Vector3[6];

        result[0] = j1;
        result[1] = j2;
        result[2] = j3; 
        result[3] = j4; 
        result[4] = j5; 
        result[5] = endfactor;

        return result;
    }

    float lossCostFunction(Vector6 theta)
    {
        Vector3 endpostion = endFactorFunction(theta)[5];
        return Vector3.Distance(endpostion, target.position) * Vector3.Distance(endpostion, target.position);
    }

    Vector6 GetGradient(Vector6 theta)
    {
        Vector6 gradientVector = Vector6.zero;
        float step = 1e-2f;

        // x
        Vector6 thetaPlus = theta;
        thetaPlus.x = theta.x + step;
        gradientVector.x = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        // y
        thetaPlus = theta;
        thetaPlus.y = theta.y + step;
        gradientVector.y = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        // z
        thetaPlus = theta;
        thetaPlus.z = theta.z + step;
        gradientVector.z = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        // w
        thetaPlus = theta;
        thetaPlus.w = theta.w + step;
        gradientVector.w = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        // v
        thetaPlus = theta;
        thetaPlus.v = theta.v + step;
        gradientVector.v = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        // u
        thetaPlus = theta;
        thetaPlus.u = theta.u + step;
        gradientVector.u = (lossCostFunction(thetaPlus) - lossCostFunction(theta)) / step;

        gradientVector.Normalize();
        return gradientVector;
    }

    private void ResetToInitialPositions()
    {
        resetProgress += Time.deltaTime / resetDuration;

        for(int i = 1; i < joints.Count; i++)
        {
            joints[i].localPosition = Vector3.Lerp(joints[i].localPosition, initPosition[i - 1], resetProgress);
        }

        if (resetProgress >= 1f)
        {
            resetProgress = 0f;
        }
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
        if(state == false)
        {
            StartReset();
        }
    }

    public void StartReset()
    {
        theta = Vector6.zero;
        alpha = initAlpha;
        resetProgress = 0f;
    }
}

public struct Vector6
{
    public float x, y, z, w, v, u;

    public static Vector6 zero => new Vector6 { x = 0, y = 0, z = 0, w = 0, v = 0, u = 0 };

    public static Vector6 operator -(Vector6 a, Vector6 b)
    {
        return new Vector6 { x = a.x - b.x, y = a.y - b.y, z = a.z - b.z, w = a.w - b.w, v = a.v - b.v, u = a.u - b.u };
    }

    public static Vector6 operator *(float d, Vector6 a)
    {
        return new Vector6 { x = d * a.x, y = d * a.y, z = d * a.z, w = d * a.w, v = d * a.v, u = d * a.u };
    }

    public void Normalize()
    {
        float magnitude = Mathf.Sqrt(x * x + y * y + z * z + w * w + v * v + u * u);
        if (magnitude > 1e-5f)
        {
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
            w /= magnitude;
            v /= magnitude;
            u /= magnitude;
        }
    }
}
