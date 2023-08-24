using System;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

namespace UnityEngine.InputSystem.LouiGCustom.RebindUI
{
    public class RebindActionUI : MonoBehaviour
    {
        private void Start()
        {
            UpdateActionLabel();
        }

        /// <summary>
        /// Reference to the action that is to be rebound.
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// ID (in string form) of the binding that is to be rebound on the action.
        /// </summary>
        /// <seealso cref="InputBinding.id"/>
        public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Text component that receives the name of the action. Optional.
        /// </summary>
        public TextMeshProUGUI actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        /// <summary>
        /// Text component that receives the display string of the binding. Can be <c>null</c> in which
        /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
        /// </summary>
        public TextMeshProUGUI bindingText
        {
            get => m_BindingText;
            set
            {
                m_BindingText = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Optional text component that receives a text prompt when waiting for a control to be actuated.
        /// </summary>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindOverlay"/>
        public TextMeshProUGUI rebindPrompt
        {
            get => m_RebindText;
            set => m_RebindText = value;
        }

        /// <summary>
        /// Optional UI that is activated when an interactive rebind is started and deactivated when the rebind
        /// is finished. This is normally used to display an overlay over the current UI while the system is
        /// waiting for a control to be actuated.
        /// </summary>
        /// <remarks>
        /// If neither <see cref="rebindPrompt"/> nor <c>rebindOverlay</c> is set, the component will temporarily
        /// replaced the <see cref="bindingText"/> (if not <c>null</c>) with <c>"Waiting..."</c>.
        /// </remarks>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindPrompt"/>
        public GameObject rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        /// <summary>
        /// Event that is triggered every time the UI updates to reflect the current binding.
        /// This can be used to tie custom visualizations to bindings.
        /// </summary>
        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        /// <summary>
        /// Event that is triggered when an interactive rebind is started on the action.
        /// </summary>
        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        /// <summary>
        /// Event that is triggered when an interactive rebind has been completed or canceled.
        /// </summary>
        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        /// <summary>
        /// When an interactive rebind is in progress, this is the rebind operation controller.
        /// Otherwise, it is <c>null</c>.
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

        /// <summary>
        /// Return the action and binding index for the binding that is targeted by the component
        /// according to
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = m_Action?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(m_BindingId))
                return false;

            // Look up binding index.
            var bindingId = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trigger a refresh of the currently displayed binding.
        /// </summary>
        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // Get display string from action.
            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            // Give listeners a chance to configure UI in response.
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        /// <summary>
        /// Remove currently applied binding overrides.
        /// </summary>
        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            //if (action.bindings[bindingIndex].isComposite)
            //{
            //    // It's a composite. Remove overrides from part bindings.
            //    for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
            //        action.RemoveBindingOverride(i);
            //}
            //else
            //{
            //    action.RemoveBindingOverride(bindingIndex);
            //}

            ResetBinding(action, bindingIndex);

            if (action.bindings[bindingIndex].isComposite)
            {
                //  It's a composite. Remove overrides from part bindings.
                for (int index = bindingIndex+1; 
                    index < action.bindings.Count && action.bindings[index].isPartOfComposite;
                    ++index)
                {
                    action.RemoveBindingOverride(index);
                }
            } else
            {
                action.RemoveBindingOverride(bindingIndex);
            }

            UpdateBindingDisplay();
        }

        private void ResetBinding(InputAction action, int bindingIndex)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            string oldOverridePath = newBinding.overridePath;

            action.RemoveBindingOverride(bindingIndex);
            int currentIndex = -1;

            foreach  (InputAction otherAction in action.actionMap.actions)
            {
                currentIndex++;
                InputBinding currentBinding = action.actionMap.bindings[currentIndex];

                if (otherAction == action)
                {
                    if (newBinding.isPartOfComposite)
                    {
                        if (currentBinding.overridePath == newBinding.path)
                        {
                            otherAction.ApplyBindingOverride(currentIndex, oldOverridePath);
                        }
                    } else
                    {
                        continue;
                    }
                }

                for (int index = 0; index < otherAction.bindings.Count; ++index)
                {
                    InputBinding binding = otherAction.bindings[index];
                    if (binding.overridePath == newBinding.path)
                    {
                        otherAction.ApplyBindingOverride(index, oldOverridePath);
                    }
                }
            }
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
            }

            //  Disable action before use to prevent error.
            //  為避免錯誤在更改按鍵錢必須要將該Action disable.
            action.Disable();

            // Configure the rebind.
            //  於OnCancel和OnComplete事件中恢復Action的狀態(Enable)
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                //.WithCancelingThrough("<Keyboard>/escape") /*設置取消編輯事件，這邊是按鍵盤的ESC會取消編輯，應該可以做更多自定義*/
                .OnCancel(
                    operation =>
                    {
                        action.Enable();
                        m_RebindStopEvent?.Invoke(this, operation);
                        m_RebindOverlay?.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        action.Enable();
                        m_RebindOverlay?.SetActive(false);
                        m_RebindStopEvent?.Invoke(this, operation);

                        if (CheckDuplicateBindings(action, bindingIndex, allCompositeParts))
                        {
                            action.RemoveBindingOverride(bindingIndex);
                            CleanUp();
                            PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                            return;
                        }

                        UpdateBindingDisplay();
                        CleanUp();

                        // If there's more composite parts we should bind, initiate a rebind
                        // for the next part.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            // If it's a part binding, show the name of the part in the UI.
            var partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = action.bindings[bindingIndex].name;

            // Bring up rebind overlay, if we have one.
            m_RebindOverlay?.SetActive(true);
            if (m_RebindText != null)
            {
                //  var isNullOrEmptyTrue = LocalizationManager.Localize("Menu.Control_WaitA", partName, m_RebindOperation.expectedControlType);
                var isNullOrEmptyFalse = partName;
                //var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                //    ? isNullOrEmptyTrue : isNullOrEmptyFalse;
                m_RebindText.text = isNullOrEmptyFalse;
            }

            // If we have no rebind overlay and no callback but we have a binding text label,
            // temporarily set the binding text label to "<Waiting>".
            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<Waiting...>";

            // Give listeners a chance to act on the rebind starting.
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();
        }

        private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            int currentIndex = -1;

            /*
             *  檢查當前使用的InputAsset是否有設置的按鈕
             */
            foreach(InputBinding binding in action.actionMap.bindings)
            {
                currentIndex++;

                if (binding.action == newBinding.action)
                {
                    if (binding.isPartOfComposite && (currentIndex != bindingIndex))
                    {
                        if (binding.effectivePath == newBinding.effectivePath)
                        {
                            var duplicateText = newBinding.effectivePath;
                            m_DuplicateText.enabled = true;
                            m_DuplicateText.text = duplicateText;
                            return true;
                        }
                    }   else
                    {
                        continue;
                    }
                }

                if (binding.effectivePath == newBinding.effectivePath)
                {
                    var duplicateText = newBinding.effectivePath;
                    m_DuplicateText.enabled = true;
                    m_DuplicateText.text = duplicateText;
                    return true;
                }
            }

            /*
            *  檢查當前使用的InputAsset的集合(Vector2)是否有設置的按鈕
            */
            if (allCompositeParts)
            {
                for (int index = 1; index < bindingIndex; ++index)
                {
                    if (action.bindings[index].effectivePath == newBinding.overridePath)
                    {
                        var duplicateText = newBinding.effectivePath;
                        m_DuplicateText.enabled = true;
                        m_DuplicateText.text = duplicateText;
                        return true;
                    }
                }
            }

            m_DuplicateText.enabled = false;
            return false;
        }
             
        protected void OnEnable()
        {
            if (s_RebindActionUIs == null)
                s_RebindActionUIs = new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        // When the action system re-resolves bindings, we want to update our UI in response. While this will
        // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
        // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
        // will update our UI to reflect the current keyboard layout.
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                var component = s_RebindActionUIs[i];
                var referencedAction = component.actionReference?.action;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [Tooltip("Reference to action that is to be rebound from the UI.")]
        [SerializeField]
        private InputActionReference m_Action;

        [SerializeField]
        private string m_BindingId;

        [SerializeField]
        private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the "
            + "rebind UI not show a label for the action.")]
        [SerializeField]
        private TextMeshProUGUI m_ActionLabel;

        [Tooltip("Text label that will receive the current, formatted binding string.")]
        [SerializeField]
        private TextMeshProUGUI m_BindingText;

        [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
        [SerializeField]
        private GameObject m_RebindOverlay;

        [Tooltip("Optional text label that will be updated with prompt for user input.")]
        [SerializeField]
        private TextMeshProUGUI m_RebindText;
        [SerializeField]
        private TextMeshProUGUI m_DuplicateText;

        /// <summary>
        /// 客製化按鈕標題名稱選項(Bool)
        /// </summary>
        [Tooltip("Optional bool field which allows you to OVERRIDE the action label with your own text")]
        [SerializeField]
        public bool m_OverRideActionLabel = false;

        /// <summary>
        /// 客製化按鈕標題名稱，需將m_OverRideActionLabel 設為True才會作用。
        /// </summary>
        [Tooltip("What text should be displayed for the action label?")]
        [SerializeField]
        private string m_ActionLabelString = "";

        [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
            + "bindings in custom ways, e.g. using images instead of text.")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
            + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
            + "customize the rebind.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        // We want the label for the action name to update in edit mode, too, so
        // we kick that off from here.
#if UNITY_EDITOR
        protected void OnValidate()
        {
            //UpdateActionLabel();
            UpdateBindingDisplay();
        }

#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                var action = m_Action?.action;

                if (m_OverRideActionLabel)
                {
                    m_ActionLabel.text = m_ActionLabelString;
                } else
                {
                    m_ActionLabel.text = action != null ? action.name : string.Empty;
                    m_ActionLabelString = string.Empty;
                }
            }
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }
    }
}
