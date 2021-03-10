using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tobii.StreamEngine;
using System.Threading;

namespace Test5L
{
    public class Eyetracking
    {
        private double eyetrackerPositionX;
        private double eyetrackerPositionY;
        private double eyetrackerOriginZ;

        public Eyetracking(IntPtr deviceContext)
        {
            tobii_error_t result;

            Thread processCallbackThread = new Thread(new ParameterizedThreadStart(ProcessCallbackFunction))
            {
                IsBackground = true,
                Name = "Process Callback Thread"
            };
            processCallbackThread.Start(deviceContext);

            result = Interop.tobii_gaze_point_subscribe(deviceContext, OnGazePoint);
            result = Interop.tobii_gaze_origin_subscribe(deviceContext, OnGazeOrigin);
        }

        private void ProcessCallbackFunction(Object deviceContextObject)
        {
            IntPtr deviceContext = (IntPtr)deviceContextObject;
            while (true)
            {
                Interop.tobii_wait_for_callbacks(new[] { deviceContext });
                Interop.tobii_device_process_callbacks(deviceContext);
                Thread.Sleep(1);
            }
        }

        private void OnGazeOrigin(ref tobii_gaze_origin_t gaze_origin, IntPtr user_data)
        {
            if (gaze_origin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID && gaze_origin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                double zLeft = gaze_origin.left.z;
                double zRight = gaze_origin.right.z;
                double res = (zLeft + zRight) / 2;
                eyetrackerOriginZ = res;
            }
            else if (gaze_origin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                eyetrackerOriginZ = gaze_origin.left.z;
            }
            else if (gaze_origin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                eyetrackerOriginZ = gaze_origin.right.z;
            }
        }

        private void OnGazePoint(ref tobii_gaze_point_t gaze_point, IntPtr user_data)
        {
            eyetrackerPositionX = gaze_point.position.x;
            eyetrackerPositionY = gaze_point.position.y;
        }

        public double GetEyetrackerPositionX()
        {
            return eyetrackerPositionX;
        } 

        public double GetEyetrackerPositionY()
        {
            return eyetrackerPositionY;
        }

        public double GetEyetrackerOriginZ()
        {
            return eyetrackerOriginZ;
        }
    }
}
