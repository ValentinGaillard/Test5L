using System;
using System.Collections.Generic;
using System.Windows;
using Tobii.StreamEngine;

namespace Test5L
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        Eyetracking eyetracker;

        public App()
        {
            //Initialisation de l'eyetracker

            tobii_error_t result;
            result = Interop.tobii_api_create(out IntPtr apiContext, null);

            Console.WriteLine("Api create " + result);

            result = Interop.tobii_enumerate_local_device_urls(apiContext, out List<String> urls);
            Console.WriteLine("Enumerate Local Device " + result);
            if (urls.Count == 0)
                Console.WriteLine("Error: No device(s) found\n");

            IntPtr deviceContext;
            result = Interop.tobii_device_create(apiContext, urls[0], Interop.tobii_field_of_use_t.TOBII_FIELD_OF_USE_INTERACTIVE, out deviceContext);
            Console.WriteLine("Device create " + result);

            //On crée un objet eyetracker
            eyetracker = new Eyetracking(deviceContext);
        }
       
        public Eyetracking GetEyetracker()
        {
            return eyetracker;
        }
    }
}
