using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Factories;
using GrazerCore.GameFlow.States;
using GrazerCore.GameFlow;
using UnityEngine.InputSystem;
using SkateHero.GameElements;
using SkateHero.UIs;

namespace SkateHero.GameFlow.States
{
    public class GamePlayState : GameState
    {
        private GamePlayStatePackage _gamePlayStatePackage;
        private BasicPlayer _Player = null;
        private Vector2 _BirthPoint = Vector2.zero;
        private PlayerInput _PlayerInput = null;
        private string _GamePlayActionMap = "";
        private string _GameUIActionMap = "";
        //  Player damage response
        private DamageResponseTypeProtect _DamageResponse = null;
        private GamePlayUI _GamePlayUI = null;
        private GamePauseUI _GamePauseUI = null;
        private InputAction _PauseAction = null;
        private bool isPause = false;
        private GameEndUI _GameEndUI = null;
        private StageController _StageController = null;
        private AudioSource _BGMPlayer = null;
        private GameEventController _GameEventController = null;
        private bool IsInitialize = false;

        public GamePlayState(GamePlayStatePackage gamePlayStatePackage)
        {
            _gamePlayStatePackage = gamePlayStatePackage;
        }

        public override void OnEnter()
        {
            if (!IsInitialize)
            {
                Initialize();
            }
            _PlayerInput.DeactivateInput();
            StartGame();
        }

        public override void Track(float dt)
        {
            
        }

        public override void OnExit()
        {
            //  Disable pause action
            _PauseAction.Disable();
            _BGMPlayer.Stop();
            _GameEventController.CloseGameEvent();
            ClearBattleField();
            //  CLose UI
            _GamePlayUI.Close();
            _GamePauseUI.Close();
            _GameEndUI.Close();
        }

        private void Win()
        {
            _PauseAction.Disable();

            _GameEventController.CloseGameEvent();
            _Player.SleepObject();
            _Player.Invincible = true;
            _GameEndUI.GameEnd(true);
        }

        private void GameOver()
        {
            _PauseAction.Disable();

            _GameEventController.CloseGameEvent();
            //  Stop all alive enemys
            var aliveEnemys = EnemyFactory.GetAliveEnemys();
            var enemyCount = aliveEnemys.Count;
            for (int index = 0; index < enemyCount; ++index)
            {
                var enemy = aliveEnemys[index];
                enemy.SleepObject();
            }
            _GameEndUI.GameEnd(false);
        }

        private void ClearBattleField()
        {
            // Clear all bullet and enemy.
            _StageController.StageClose();
        }

        private void Pause()
        {
            Time.timeScale = 0;
            //_PlayerInput.SwitchCurrentActionMap(_GameUIActionMap);
            _PlayerInput.DeactivateInput();
        }

        private void Continue()
        {
            Time.timeScale = 1;
            //_PlayerInput.SwitchCurrentActionMap(_GamePlayActionMap);
            _PlayerInput.ActivateInput();
        }

        private void StartGame()
        {
            Continue();
            //  UI set
            _GamePlayUI.Open();
            _GamePauseUI.Close();
            _GameEndUI.Close();

            //  Enable pause action
            isPause = false;
            _PauseAction.Enable();

            //  Stage set
            _StageController.ResetStage();
            _StageController.StageStart();
            _BGMPlayer.clip = _StageController.StageBGM;
            _BGMPlayer.Play(0);

            //  Player set
            _Player.WakeUpObject();
            //  Set Player UI
            if (!_GamePlayUI.IsInitialize)
            {
                _GamePlayUI.SetPlayer(_Player);
                _GamePlayUI.Initialize();
            }
            _Player.MoveTarget.localPosition = _BirthPoint;

            //  Game event set
            _GameEventController.Reset();
            _GameEventController.StartFlow();

            _PlayerInput.ActivateInput();
        }

        private void ReStart()
        {
            _GameEventController.CloseGameEvent();
            StartGame();
        }

        private void BackToTitle()
        {
            _Player.SleepObject();
            GoToNextState();
        }

        private void Initialize()
        {
            //  Create and Initialize player UI.
            Application.targetFrameRate = _gamePlayStatePackage.FPS;
            //  Create player
            _Player = GameObject.Instantiate<BasicPlayer>(_gamePlayStatePackage.PlayerPrefab);
            _Player.Initialization();
            _BirthPoint = _gamePlayStatePackage.BirthPoint;
            //  Set game over event on player die.
            _Player.OnPlayerDie.AddListener(GameOver);
            //  Set damage response 
            _DamageResponse = _gamePlayStatePackage.DamageResponseTypeProtect;
            _DamageResponse.Install(_Player);

            //  Set player input(From input system)
            _PlayerInput = _gamePlayStatePackage.PlayerInput;
            _GamePlayActionMap = _gamePlayStatePackage.GamePlayActionMap;
            _GameUIActionMap = _gamePlayStatePackage.GameUIActionMap;

            //  Create UIs
            _GamePlayUI = GameObject.Instantiate<GamePlayUI>(_gamePlayStatePackage.GamePlayUI);
            _GamePauseUI = GameObject.Instantiate<GamePauseUI>(_gamePlayStatePackage.GamePauseUI);
            _GameEndUI = GameObject.Instantiate<GameEndUI>(_gamePlayStatePackage.GameEndUI);
            _GamePauseUI.Initialize();
            _GameEndUI.Initialize();
            //  Set ui event
            _GamePauseUI.OnRestartGame.AddListener(ReStart);
            _GameEndUI.OnRestartGame.AddListener(ReStart);
            _GamePauseUI.OnBackToTitle.AddListener(BackToTitle);
            _GameEndUI.OnBackToTitle.AddListener(BackToTitle);

            //  Set pause action and event
            _PauseAction = _gamePlayStatePackage.PauseAction.action;
            _PauseAction.performed += (ctx) => {
                isPause = !isPause;

                if (isPause)
                {
                    Pause();
                    _GamePauseUI.Open();

                }
                else
                {
                    Continue();
                    UIManager.RemoveNewestOpenUI(CommandCallingFrom.OTHERSIDE);
                }
            };

            //  Create stage controller
            _StageController = GameObject.Instantiate<StageController>(_gamePlayStatePackage.StageController);

            _BGMPlayer = _gamePlayStatePackage.BGMPlayer;

            _GameEventController = _gamePlayStatePackage.GameEventController;
            _GameEventController.GameEvents = _StageController.GameEvents;
            _GameEventController.Initialize();
            //  Set win event when all game event work done.
            _GameEventController.OnEventAllDone.AddListener(Win);
            IsInitialize = true;
        }
    }

    public struct GamePlayStatePackage
    {
        public GamePlayUI GamePlayUI;

        public GamePauseUI GamePauseUI;

        public InputActionReference PauseAction;

        public GameEndUI GameEndUI;

        public BasicPlayer PlayerPrefab;

        public Vector2 BirthPoint;

        public PlayerInput PlayerInput;

        public string GamePlayActionMap;

        public string GameUIActionMap;

        public DamageResponseTypeProtect DamageResponseTypeProtect;

        public StageController StageController;

        public AudioSource BGMPlayer;

        public GameEventController GameEventController;

        public int FPS;
    }
}
