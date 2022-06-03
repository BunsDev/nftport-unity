using System;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

namespace NFTPort.Internal {
    class InstallPortDependencies : EditorWindow
    {
        static AddRequest Request;


        [MenuItem("NFTPort/Install Dependencies")]
        public static void ShowWindow()
        {
            var win = GetWindow<InstallPortDependencies>("Port Dependencies");
            SetSize(win);
        }
        void OnGUI()
        {
            GUILayout.Label("\n" +
                            " NFTPort needs the following packages : \n" +
                            "", EditorStyles.label);
            
            GUILayout.Label("com.unity.nuget.newtonsoft-json");
            
            if (GUILayout.Button("Install Now", GUILayout.Height(25)))
                Add();

        }
        
        static void SetSize(InstallPortDependencies win) 
        {
            win.minSize = new Vector2(330, 100);
            win.maxSize = new Vector2(330, 100);
        }
        
        
        /// ///////// CHECK
        static ListRequest listRequest;
        public static void CheckPkgsListAndInstall()
        {
            listRequest = Client.List();    // List packages installed for the Project
            EditorApplication.update += CheckPackageProgress;
        }
        
        private static UnityAction<bool> OnListCheckCompleteActionPackageExists;
        public static void OnListCheckComplete(UnityAction<bool> action)
        {
            OnListCheckCompleteActionPackageExists = action;
          
        }

        static void CheckPackageProgress()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name.Contains("newtonsoft"))
                        {
                            OnListCheckCompleteActionPackageExists.Invoke(true);
                            EndListCheck();
                        }
                    }
                else if (listRequest.Status >= StatusCode.Failure)
                    Debug.Log(listRequest.Error.message);

                EndListCheck();
            }
        }
        static void EndListCheck()
        {
            EditorApplication.update -= CheckPackageProgress;
        }
        
        
          
        //////////// ADD
        public static void Add()
        {
            // Add a package to the project
            Request = Client.Add("com.unity.nuget.newtonsoft-json");
            EditorApplication.update += AddProgress;
        }
        
        static void AddProgress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + Request.Result.packageId);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= AddProgress;
            }
        }
        
    }
}