using System;
using NCore;
using UnityEngine;

namespace NCore
{
    public class GameConfig : MonoBehaviour, ISingleton
    {
        [SerializeField] private bool m_useLocalAsset = true;
        [SerializeField] private bool m_hotUpdate = false;

        [SerializeField] private bool ShowFPSOnGUI = true;

        public static bool UseLocalAsset => Singleton.m_useLocalAsset;
        public static bool HotUpdate => Singleton.m_hotUpdate;

        #region Singleton

        private static GameConfig _instence = null;

        public void InitSingleton()
        {
            gameObject.name = "[GameConfig]";
        }

        public static GameConfig Singleton
        {
            get
            {
                if (_instence == null)
                    _instence = GameObject.FindObjectOfType<GameConfig>();

                return _instence;
            }
        }

        #endregion

        private void Awake()
        {
            InitSingleton();

            // FPS OnGUI
            FpsOnGUI fps = gameObject.GetOrAddCompoment<FpsOnGUI>();
            fps.enabled = ShowFPSOnGUI;

            Debug.Log("[ResMgr]: UseLocalAsset：" + m_useLocalAsset);
        }

        private void OnDestroy()
        {
            _instence = null;
        }
    }
}