using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameFlow;
using SkateHero.GameElements;
using SkateHero.GameFlow.States;
using SkateHero.UIs;

namespace SkateHero.GameFlow
{
    public class SkateGuyGameController : GameController
    {
        [Header("MenuState")]
        [SerializeField]
        private MenuUI m_MenuUIPrefab = null;
        [SerializeField]
        private UnityEngine.InputSystem.InputActionReference m_CloseUIHotKey = null;
        [Header("GamePlayState")]
        [SerializeField]
        private GamePlayUI m_GamePlayUIPrefab = null;
        [SerializeField]
        private GamePauseUI m_GamePauseUIPrefab = null;
        [SerializeField]
        private UnityEngine.InputSystem.InputActionReference m_PauseAction = null;
        [SerializeField]
        private GameEndUI m_GameEndUI = null;
        [SerializeField]
        private BasicPlayer m_BasicPlayerPrefab = null;
        [SerializeField]
        private Vector2 m_BirthPoint = Vector2.zero;
        [SerializeField]
        private UnityEngine.InputSystem.PlayerInput m_PlayerInput = null;
        [SerializeField]
        private string m_GamePlayActionMap = "";
        [SerializeField]
        private string m_GameUIActionMap = "";
        [SerializeField]
        private DamageResponseTypeProtect m_DamageResponseTypeProtect = null;
        [SerializeField]
        private StageController m_StageController = null;
        [SerializeField]
        private AudioSource m_BGMPlayer = null;
        //[SerializeField]
        //private GameEventController m_GameEventController = null;
        [SerializeField]
        private int m_GameFPS = 60;


        public void Start()
        {
            //  Hide Cursor
            Cursor.visible = false;

            //  Set first state and work
            var menuStagePackage = new MenuStatePackage();
            menuStagePackage.MenuUI = m_MenuUIPrefab;
            menuStagePackage.CloseUIHotKey = m_CloseUIHotKey;

            var gamePlayStatePackage = new GamePlayStatePackage();
            gamePlayStatePackage.GamePlayUI = m_GamePlayUIPrefab;
            gamePlayStatePackage.GamePauseUI = m_GamePauseUIPrefab;
            gamePlayStatePackage.PauseAction = m_PauseAction;
            gamePlayStatePackage.GameEndUI = m_GameEndUI;
            gamePlayStatePackage.PlayerPrefab = m_BasicPlayerPrefab;
            gamePlayStatePackage.BirthPoint = m_BirthPoint;
            gamePlayStatePackage.PlayerInput = m_PlayerInput;
            gamePlayStatePackage.GamePlayActionMap = m_GamePlayActionMap;
            gamePlayStatePackage.GameUIActionMap = m_GameUIActionMap;
            gamePlayStatePackage.DamageResponseTypeProtect = m_DamageResponseTypeProtect;
            gamePlayStatePackage.StageController = m_StageController;
            gamePlayStatePackage.BGMPlayer = m_BGMPlayer;
            var gameEventController = new GameEventController();
            gamePlayStatePackage.GameEventController = gameEventController;
            gamePlayStatePackage.FPS = m_GameFPS;

            var startState = new MenuState(menuStagePackage);
            var gamePlayState = new GamePlayState(gamePlayStatePackage);
            startState.NextState = gamePlayState;
            gamePlayState.NextState = startState;
            ChangeState(startState);
        }
    }
}
