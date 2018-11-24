﻿/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundAlignCamera.cs
 *  Description  :  Camera rotate around and align to target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/9/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Camera rotate around and align to target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/AroundAlignCamera")]
    public class AroundAlignCamera : AroundCamera
    {
        #region Field and Property
        /// <summary>
        /// Damper for align.
        /// </summary>
        [Range(0, 5)]
        public float alignDamper = 2;

        /// <summary>
        /// Threshold of linear adsorbent.
        /// </summary>
        [Range(0, 1)]
        public float threshold = 0.1f;

        /// <summary>
        /// Camera is auto aligning.
        /// </summary>
        public bool IsAligning { protected set; get; }

        /// <summary>
        /// Start align event.
        /// </summary>
        public event Action OnAlignStart;

        /// <summary>
        /// End align event.
        /// </summary>
        public event Action OnAlignEnd;

        protected Vector2 lastAngles;
        protected Vector3 currentDirection, targetDirection, lastDirection;
        protected float lastDistance;
        protected float anglesSpeed, directionSpeed, distanceSpeed;
        protected float anglesOffset, directionOffset, distanceOffset;
        protected bool linearAdsorbent;
        #endregion

        #region Protected Method
        protected override void LateUpdate()
        {
            if (IsAligning)
                AutoAlignView();
            else
                AroundByMouseInput();
        }
        /// <summary>
        /// Check Offset Value
        /// </summary>
        /// <param name="anglesOffset"></param>
        /// <param name="directionOffset"></param>
        /// <param name="distanceOffset"></param>
        private void CheckValue(ref float anglesOffset,ref float directionOffset,ref float distanceOffset)
        {
            anglesOffset = Math.Round(anglesOffset, 4) == 0 ? 0 : anglesOffset;
            directionOffset = Math.Round(directionOffset, 4) == 0 ? 0 : directionOffset;
            distanceOffset = Math.Round(distanceOffset, 4) == 0 ? 0 : distanceOffset;
        }
        /// <summary>
        /// Auto align camera veiw to target.
        /// </summary>
        protected void AutoAlignView()
        {
            //Calculate current offset.
            var currentAnglesOffset = (targetAngles - CurrentAngles).magnitude;
            var currentDirectionOffset = (targetDirection - currentDirection).magnitude;
            var currentDistanceOffset = Mathf.Abs(targetDistance - CurrentDistance);
            CheckValue(ref currentAnglesOffset,ref currentDirectionOffset,ref currentDistanceOffset);

            //Check align finish.
            if (currentAnglesOffset < Vector3.kEpsilon && currentDirectionOffset < Vector3.kEpsilon &&
                currentDistanceOffset < Vector3.kEpsilon)
            {
                IsAligning = false;
                if (OnAlignEnd != null)
                    OnAlignEnd.Invoke();
            }
            else
            {
                if (linearAdsorbent)
                {
                    //MoveTowards to linear adsorbent align.
                    CurrentAngles = Vector2.MoveTowards(CurrentAngles, targetAngles, anglesSpeed * Time.deltaTime);
                    currentDirection = Vector3.MoveTowards(currentDirection, targetDirection, directionSpeed * Time.deltaTime);
                    CurrentDistance = Mathf.MoveTowards(CurrentDistance, targetDistance, distanceSpeed * Time.deltaTime);
                }
                else
                {
                    //Record last.
                    lastAngles = CurrentAngles;
                    lastDirection = currentDirection;
                    lastDistance = CurrentDistance;

                    //Lerp to align.
                    CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, alignDamper * Time.deltaTime);
                    currentDirection = Vector3.Lerp(currentDirection, targetDirection, alignDamper * Time.deltaTime);
                    CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, alignDamper * Time.deltaTime);

                    //Check into linear adsorbent.
                    if (currentAnglesOffset / anglesOffset < threshold && currentDirectionOffset / directionOffset < threshold &&
                        currentDistanceOffset / distanceOffset < threshold)
                    {
                        anglesSpeed = (CurrentAngles - lastAngles).magnitude / Time.deltaTime;
                        directionSpeed = (currentDirection - lastDirection).magnitude / Time.deltaTime;
                        distanceSpeed = Mathf.Abs(CurrentDistance - lastDistance) / Time.deltaTime;
                        linearAdsorbent = true;
                    }
                }

                //Update position and rotation.
                transform.position = target.position + currentDirection.normalized * CurrentDistance;
                transform.rotation = Quaternion.Euler(CurrentAngles);
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// SetCurrentAngle
        /// </summary>
        /// <param name="angle"></param>
        public void SetCurrentAngle(Vector2 angle)
        {
            CurrentAngles = angle;
        }
        /// <summary>
        /// Align camera veiw to target.
        /// </summary>
        /// <param name="center">Around center.</param>
        /// <param name="angles">Rotate angles.</param>
        /// <param name="distance">Distance from camera to target.</param>
        public void AlignVeiwToTarget(Transform center, Vector2 angles, float distance)
        {
            target = center;
            targetAngles = angles;
            targetDistance = distance;

            //Optimal angles.
            while (targetAngles.y - CurrentAngles.y > 180)
                targetAngles.y -= 360;
            while (targetAngles.y - CurrentAngles.y < -180)
                targetAngles.y += 360;

            //Calculate lerp parameter.
            currentDirection = (transform.position - target.position).normalized;
            targetDirection = (Quaternion.Euler(targetAngles) * Vector3.back).normalized;
            CurrentDistance = Vector3.Distance(transform.position, target.position);

            //Calculate offset.
            anglesOffset = Mathf.Max((targetAngles - CurrentAngles).magnitude, Vector3.kEpsilon);
            directionOffset = Mathf.Max((targetDirection - currentDirection).magnitude, Vector3.kEpsilon);
            distanceOffset = Mathf.Max(Mathf.Abs(targetDistance - CurrentDistance), Vector3.kEpsilon);

            //Start align.
            linearAdsorbent = false;
            IsAligning = true;
            if (OnAlignStart != null)
                OnAlignStart.Invoke();
        }

        /// <summary>
        /// Align camera veiw to target.
        /// </summary>
        /// <param name="alignTarget">Target of camera align.</param>
        public void AlignVeiwToTarget(AlignTarget alignTarget)
        {
            AlignVeiwToTarget(alignTarget.center, alignTarget.angles, alignTarget.distance);

            //Override range.
            angleRange = alignTarget.angleRange;
            distanceRange = alignTarget.distanceRange;
        }
        #endregion
    }
}