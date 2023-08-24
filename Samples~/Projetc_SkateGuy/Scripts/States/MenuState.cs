using UnityEngine;
using UnityEngine.InputSystem;
using GrazerCore.GameFlow.States;
using SkateHero.UIs;

namespace SkateHero.GameFlow.States
{
    public class MenuState : GameState
    {
        private MenuStatePackage _menuStatePackage;
        private MenuUI _menuUI = null;
        private InputAction _CloseHotKey;
        private bool IsInitialize = false;

        public MenuState(MenuStatePackage menuStatePackage)
        {
            _menuStatePackage = menuStatePackage;
        }

        public override void OnEnter()
        {
            if (!IsInitialize)
            {
                Initialize();
            }
            _menuUI.Open();
            _CloseHotKey.Enable();
        }

        public override void Track(float dt)
        {
            
        }

        public override void OnExit()
        {
            _menuUI.Close();
            _CloseHotKey.Disable();
        }

        private void Initialize()
        {
            UIManager.Initialize();
            _CloseHotKey = _menuStatePackage.CloseUIHotKey.action;
            _CloseHotKey.started += (ctx) => {
                UIManager.RemoveNewestOpenUI(CommandCallingFrom.OTHERSIDE);
            };
            if (_menuUI == null)
            {
                _menuUI = GameObject.Instantiate<MenuUI>(_menuStatePackage.MenuUI);
                _menuUI.OnGameStart.AddListener(() => {
                    GoToNextState();
                });
            }
            if (!_menuUI.IsInitialize)
            {
                _menuUI.Initialize();
            }
            IsInitialize = true;
        }
    }

    public struct MenuStatePackage
    {
        public MenuUI MenuUI;

        public InputActionReference CloseUIHotKey;
    }
}
