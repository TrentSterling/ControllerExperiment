﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControllerExperiment.SubComponents
{
    public class HorizontalMove : SubComponent
    {
        [Header("Debug")]
        public Vector3 TargetWalkDir = new Vector3();
        public float Speed;
        public Vector3 MoveForce = new Vector3();
        public Vector3 GroundNormal = new Vector3();

        int DefaultLayerMask = 1 << 0;

        private void Start()
        {
            processor.ProcDic.Add(PlayerFunction.SET_TARGETWALKDIRECTION, SetTargetWalkDir);
            processor.SetFloatDic.Add(SetFloat.TARGET_WALKSPEED, SetWalkSpeed);
            processor.ProcDic.Add(PlayerFunction.WALK_TARGETDIRECTION, WalkToTargetDir);

            processor.ProcDic.Add(PlayerFunction.CANCEL_HORIZONTAL_VELOCITY, CancelHorizontalVelocity);
        }

        void SetTargetWalkDir()
        {
            TargetWalkDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                TargetWalkDir += processor.owner.transform.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                TargetWalkDir -= processor.owner.transform.right;
            }

            if (Input.GetKey(KeyCode.S))
            {
                TargetWalkDir -= processor.owner.transform.forward;
            }

            if (Input.GetKey(KeyCode.D))
            {
                TargetWalkDir += processor.owner.transform.right;
            }

            if (Vector3.SqrMagnitude(TargetWalkDir) > 0.1f)
            {
                GroundNormal = GetGroundNormal();
                TargetWalkDir = Vector3.ProjectOnPlane(TargetWalkDir, GroundNormal);

                TargetWalkDir.Normalize();

                if (TargetWalkDir.y > 0f)
                {
                    TargetWalkDir -= Vector3.up * TargetWalkDir.y;
                    TargetWalkDir.Normalize();
                }
                else if (TargetWalkDir.y < 0f)
                {
                    TargetWalkDir.Normalize();
                }

                Debug.DrawLine(processor.owner.rbody.position, processor.owner.rbody.position + TargetWalkDir, Color.yellow, 0.25f);
            }
        }

        void CancelHorizontalVelocity()
        {
            Rigidbody ownerRigidBody = processor.owner.rbody;
            ownerRigidBody.AddForce(Vector3.right * -ownerRigidBody.velocity.x, ForceMode.VelocityChange);
            ownerRigidBody.AddForce(Vector3.forward * -ownerRigidBody.velocity.z, ForceMode.VelocityChange);
        }

        void WalkToTargetDir()
        {
            CancelHorizontalVelocity();
            MoveForce = TargetWalkDir.normalized * Speed;
            processor.owner.rbody.AddForce(MoveForce, ForceMode.VelocityChange);
        }

        Vector3 GetGroundNormal()
        {
            Ray ray = new Ray(processor.owner.rbody.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, DefaultLayerMask))
            {
                return hit.normal;
            }

            return Vector3.zero;
        }

        void SetWalkSpeed(float f)
        {
            Speed = f;
        }
    }
}