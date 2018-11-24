/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundCamera.cs
 *  Description  :  Camera rotate around target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Camera rotate around target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/AroundCamera")]
    [RequireComponent(typeof(Camera))]
    public class AroundCamera : MonoBehaviour
    {
        #region Field and Property
        /// <summary>
        /// Around center.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Settings of mouse button, pointer and scrollwheel.
        /// </summary>
        public MouseSettings mouseSettings = new MouseSettings(1, 10, 10);

        /// <summary>
        /// Range limit of angle.
        /// </summary>
        public Range angleRange = new Range(-90, 90);

        /// <summary>
        /// Range limit of distance.
        /// </summary>
        public Range distanceRange = new Range(1, 10);

        /// <summary>
        /// Damper for move and rotate.
        /// </summary>
        [Range(0, 10)]
        public float damper = 5;

        /// <summary>
        /// Camera current angls.
        /// </summary>
        public Vector2 CurrentAngles { protected set; get; }

        /// <summary>
        /// Current distance from camera to target.
        /// </summary>
        public float CurrentDistance { protected set; get; }

        /// <summary>
        /// Camera target angls.
        /// </summary>
        protected Vector2 targetAngles;

        /// <summary>
        /// Target distance from camera to target.
        /// </summary>
        protected float targetDistance;

        /// <summary>
        /// 是否关闭鼠标旋转
        /// </summary>
        private bool IsDisableMouseInput;
        #endregion

        #region Protected Method
        protected virtual void Start()
        {
            CurrentAngles = targetAngles = transform.eulerAngles;
            CurrentDistance = targetDistance = Vector3.Distance(transform.position, target.position);
        }

        protected virtual void LateUpdate()
        {
            AroundByMouseInput();
        }
        /// <summary>
        /// 打开/关闭 鼠标旋转功能
        /// </summary>
        /// <param name="isEnableMouse">是否启用鼠标旋转</param>
        public virtual void SetMouseInputState(bool isEnableMouse)
        {
            IsDisableMouseInput = !isEnableMouse;
        }
        /// <summary>
        /// Camera around target by mouse input.
        /// </summary>
        protected void AroundByMouseInput()
        {
            if (IsDisableMouseInput) return;
            if (IsClickUGUIorNGUI.Instance)
            {
                if (IsClickUGUIorNGUI.Instance.isClickedUI) return;
            }
            if (Input.GetMouseButton(mouseSettings.mouseButtonID))
            {
                //Mouse pointer.
                targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //Range.
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }

            //Mouse scrollwheel.
            targetDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseSettings.wheelSensitivity;
            targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

            //Lerp.
            CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

            //Update transform position and rotation.
            transform.rotation = Quaternion.Euler(CurrentAngles);
            transform.position = target.position - transform.forward * CurrentDistance;
        }
        #endregion
    }
}