using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public enum state { TOO_FAR, CAN_MOVE, GRABBED}
    public state currentState;

    public Transform target;
    public float distance;

    public float visionAngle = 45f;

    public Gradient[] arms;
    private void Update()
    {
        switch(currentState)
        {
            case state.GRABBED:
            case state.TOO_FAR:
                SetGradientMovement(false);
                break;
            case state.CAN_MOVE:
                SetGradientMovement(true);
                break;
        }

        DetectDistances();
    }

    private void DetectDistances()
    {
        if (Vector3.Distance(transform.position, target.position) < distance && IsTargetInVisionAngle())
        {
            if(currentState == state.TOO_FAR) 
                currentState = state.CAN_MOVE;
        }
        else
        {
            currentState = state.TOO_FAR;
        }
    }

    private bool IsTargetInVisionAngle()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        return angle <= visionAngle;
    }

    private void SetGradientMovement(bool state)
    {
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].SetCanMove(state);
        }
    }

    public float GetDistance() { return distance; }
}
