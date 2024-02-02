using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    public class TerrainGenerationEditorEvents
    {
        private static TerrainGenerationEditorEvents _instance;

        public static TerrainGenerationEditorEvents Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TerrainGenerationEditorEvents();
                }
                return _instance;
            }
        }

        private TerrainGenerationEditorEvents()
        {
        }

        public Action OnClose;
        public Action<string> OnUpdateStatus;

        public void UpdateStatus(string status)
        {
            OnUpdateStatus?.Invoke(status);
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}
