using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Tobii.StreamEngine;

namespace Test5L
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr deviceContext;

        public MainWindow()
        {
            InitializeComponent();
            //Initialise eyetracker
            tobii_error_t result;
            result = Interop.tobii_api_create(out IntPtr apiContext, null);

            Console.WriteLine("Api create " + result);

            result = Interop.tobii_enumerate_local_device_urls(apiContext, out List<String> urls);
            Console.WriteLine("Enumerate Local Device " + result);
            if (urls.Count == 0)
                Console.WriteLine("Error: No device(s) found\n");

            result = Interop.tobii_device_create(apiContext, urls[0], Interop.tobii_field_of_use_t.TOBII_FIELD_OF_USE_INTERACTIVE, out deviceContext);
            Console.WriteLine("Device create " + result);

            Init();

        }

        //launch thread for callback function
        public void Init()
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

            while (processCallbackThread.IsAlive) ;
            result = Interop.tobii_gaze_point_unsubscribe(deviceContext);
            Console.WriteLine("Gaze Point Unsubscribe : " + result);
            result = Interop.tobii_device_destroy(deviceContext);
            Console.WriteLine("Device Destroy : " + result);
        }

        private void OnGazeOrigin(ref tobii_gaze_origin_t gaze_origin, IntPtr user_data)
        {
            if (gaze_origin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID && gaze_origin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                double zLeft = gaze_origin.left.z;
                double zRight = gaze_origin.right.z;
                double res = (zLeft + zRight) / 2;
                Console.WriteLine(res);
            }
            else if (gaze_origin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                Console.WriteLine(gaze_origin.left.z);
            }
            else if (gaze_origin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
            {
                Console.WriteLine(gaze_origin.right.z);
            }
        }

        private void OnGazePoint(ref tobii_gaze_point_t gaze_point, IntPtr user_data)
        {
            Console.WriteLine(gaze_point.position.x);
            Console.WriteLine(gaze_point.position.y);
        }

        //launch callback function
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

        public void OnGazePoint(ref tobii_gaze_point_t gaze_point)
        {
        }

        public void OnGazeOrigin(ref tobii_gaze_origin_t gaze_origin)
        {
      
        }

    }
}
