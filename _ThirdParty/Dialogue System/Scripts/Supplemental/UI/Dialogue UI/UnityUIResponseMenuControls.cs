#if !(UNITY_4_3 || UNITY_4_5)
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Response menu controls for UnityUIDialogueUI.
    /// </summary>
    [System.Serializable]
    public class UnityUIResponseMenuControls : AbstractUIResponseMenuControls
    {

        /// <summary>
        /// The panel containing the response menu controls. A panel is optional, but you may want one
        /// so you can include a background image, panel-wide effects, etc.
        /// </summary>
        [Tooltip("The panel containing the response menu controls. A panel is optional, but you may want one so you can include a background image, panel-wide effects, etc.")]
        public UnityEngine.UI.Graphic panel;

        /// <summary>
        /// The PC portrait image to show during the response menu.
        /// </summary>
        [Tooltip("The PC portrait image to show during the response menu.")]
        public UnityEngine.UI.Image pcImage;

        /// <summary>
        /// The label that will show the PC name.
        /// </summary>
        [Tooltip("The label that will show the PC name.")]
        public UnityEngine.UI.Text pcName;

        /// <summary>
        /// The reminder of the last subtitle.
        /// </summary>
        [Tooltip("The reminder of the last subtitle.")]
        public UnityUISubtitleControls subtitleReminder;

        /// <summary>
        /// The (optional) timer.
        /// </summary>
        [Tooltip("The (optional) timer.")]
        public UnityEngine.UI.Slider timer;

        /// <summary>
        /// If ticked, then select the currently-focused response on timeout.
        /// </summary>
        [Tooltip("Select the currently-focused response on timeout.")]
        public bool selectCurrentOnTimeout = false;

        /// <summary>
        /// The response buttons, if you want to specify buttons at design time.
        /// </summary>
        [Tooltip("Design-time positioned response buttons")]
        public UnityUIResponseButton[] buttons;

        [Tooltip("Template from which to instantiate response buttons; optional to use instead of positioned buttons above")]
        public UnityUIResponseButton buttonTemplate;

        [Tooltip("If using Button Template, instantiated buttons are parented under this GameObject")]
        public UnityEngine.UI.Graphic buttonTemplateHolder;

        [Tooltip("Optional scrollbar if the instantiated button holder is in a scroll rect")]
        public UnityEngine.UI.Scrollbar buttonTemplateScrollbar;

        [Tooltip("Reset the scroll bar to this value when preparing the response menu")]
        public float buttonTemplateScrollbarResetValue = 1;

        [Tooltip("Automatically set up explicit navigation for instantiated template buttons instead of using Automatic navigation")]
        public bool explicitNavigationForTemplateButtons = true;

        [Serializable]
        public class AnimationTransitions
        {
            [Tooltip("Trigger to set when showing the response menu panel.")]
            public string showTrigger = string.Empty;
            [Tooltip("Trigger to set when hiding the response menu panel.")]
            public string hideTrigger = string.Empty;
        }

        [Serializable]
        public class AutonumberSettings
        {
            [Tooltip("Enable autonumbering of responses.")]
            public bool enabled = false;
            [Tooltip("Format for response button text, where {0} is the number and {1} is the menu text.")]
            public string format = "{0}. {1}";
        }

        public AutonumberSettings autonumber = new AutonumberSettings();

        public AnimationTransitions animationTransitions = new AnimationTransitions();

        public UnityEvent onContentChanged = new UnityEvent();

        /// <summary>
        /// The instantiated buttons. These are only valid during a specific response menu,
        /// and only if you're using templates. Each showing of the response menu clears 
        /// this list and re-populates it with new buttons.
        /// </summary>
        [HideInInspector]
        public List<GameObject> instantiatedButtons = new List<GameObject>();

        /// <summary>
        /// Assign this delegate if you want it to replace the default timeout handler.
        /// </summary>
        public System.Action TimeoutHandler = null;

        private UnityUITimer unityUITimer = null;

        private Texture2D pcPortraitTexture = null;
        private string pcPortraitName = null;
        private Animator animator = null;
        private bool lookedForAnimator = false;
        private bool isVisible = false;
        private bool isHiding = false;

        /// <summary>
        /// Sets the PC portrait name and texture to use in the response menu.
        /// </summary>
        /// <param name="portraitTexture">Portrait texture.</param>
        /// <param name="portraitName">Portrait name.</param>
        public override void SetPCPortrait(Texture2D portraitTexture, string portraitName)
        {
            pcPortraitTexture = portraitTexture;
            pcPortraitName = portraitName;
        }

        /// <summary>
        /// Sets the portrait texture to use in the response menu if the named actor is the player.
        /// This is used to immediately update the GUI control if the SetPortrait() sequencer 
        /// command changes the portrait texture.
        /// </summary>
        /// <param name="actorName">Actor name in database.</param>
        /// <param name="portraitTexture">Portrait texture.</param>
        public override void SetActorPortraitTexture(string actorName, Texture2D portraitTexture)
        {
            if (string.Equals(actorName, pcPortraitName))
            {
                Texture2D actorPortraitTexture = AbstractDialogueUI.GetValidPortraitTexture(actorName, portraitTexture);
                pcPortraitTexture = actorPortraitTexture;
                if ((pcImage != null) && (DialogueManager.MasterDatabase.IsPlayer(actorName)))
                {
                    pcImage.sprite = UITools.CreateSprite(actorPortraitTexture);
                }
            }
        }

        public override AbstractUISubtitleControls SubtitleReminder
        {
            get { return subtitleReminder; }
        }

        /// <summary>
        /// Sets the controls active/inactive, except this method never activates the timer. If the
        /// UI's display settings specify a timeout, then the UI will call StartTimer() to manually
        /// activate the timer.
        /// </summary>
        /// <param name='value'>
        /// Value (<c>true</c> for active; otherwise inactive).
        /// </param>
        public override void SetActive(bool value)
        {
            try
            {
                SubtitleReminder.SetActive(value && SubtitleReminder.HasText);
                Tools.SetGameObjectActive(buttonTemplate, false);
                foreach (var button in buttons)
                {
                    if (button != null)
                    {
                        Tools.SetGameObjectActive(button, value && button.visible);
                    }
                }
                Tools.SetGameObjectActive(timer, false);
                Tools.SetGameObjectActive(pcName, value);
                Tools.SetGameObjectActive(pcImage, value);
                if (value == true)
                {
                    if ((pcImage != null) && (pcPortraitTexture != null)) pcImage.sprite = UITools.CreateSprite(pcPortraitTexture);
                    if ((pcName != null) && (pcPortraitName != null)) pcName.text = pcPortraitName;
                    Tools.SetGameObjectActive(panel, true);
                    if (!isVisible && CanTriggerAnimation(animationTransitions.showTrigger))
                    {
                        animator.SetTrigger(animationTransitions.showTrigger);
                    }
                    if (explicitNavigationForTemplateButtons) SetupTemplateButtonNavigation();
                }
                else
                {
                    if (isVisible && CanTriggerAnimation(animationTransitions.hideTrigger))
                    {
                        animator.SetTrigger(animationTransitions.hideTrigger);
                        DialogueManager.Instance.StartCoroutine(DisableAfterAnimation(panel));
                    }
                    else if (!isHiding)
                    {
                        if (panel != null) Tools.SetGameObjectActive(panel, false);
                    }
                }
            }
            finally
            {
                isVisible = value;
            }
        }

        /// <summary>
        /// Clears the response buttons.
        /// </summary>
        protected override void ClearResponseButtons()
        {
            DestroyInstantiatedButtons();
            if (buttons != null)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] == null) continue;
                    buttons[i].Reset();
                    buttons[i].visible = showUnusedButtons;
                }
            }
        }

        /// <summary>
        /// Sets the response buttons.
        /// </summary>
        /// <param name='responses'>
        /// Responses.
        /// </param>
        /// <param name='target'>
        /// Target that will receive OnClick events from the buttons.
        /// </param>
        protected override void SetResponseButtons(Response[] responses, Transform target)
        {
            DestroyInstantiatedButtons();

            if ((buttons != null) && (responses != null))
            {

                int buttonNumber = 0;

                // Add explicitly-positioned buttons:
                for (int i = 0; i < responses.Length; i++)
                {
                    if (responses[i].formattedText.position != FormattedText.NoAssignedPosition)
                    {
                        int position = responses[i].formattedText.position;
                        if (0 <= position && position < buttons.Length && buttons[position] != null)
                        {
                            SetResponseButton(buttons[position], responses[i], target, buttonNumber++);
                        }
                        else
                        {
                            Debug.LogWarning(DialogueDebug.Prefix + ": Buttons list doesn't contain a button for position " + position);
                        }
                    }
                }

                if ((buttonTemplate != null) && (buttonTemplateHolder != null))
                {

                    // Reset scrollbar to top:
                    if (buttonTemplateScrollbar != null)
                    {
                        buttonTemplateScrollbar.value = buttonTemplateScrollbarResetValue;
                    }

                    // Instantiate buttons from template:
                    for (int i = 0; i < responses.Length; i++)
                    {
                        if (responses[i].formattedText.position != FormattedText.NoAssignedPosition) continue;
                        GameObject buttonGameObject = GameObject.Instantiate(buttonTemplate.gameObject) as GameObject;
                        if (buttonGameObject == null)
                        {
                            Debug.LogError(string.Format("{0}: Couldn't instantiate response button template", DialogueDebug.Prefix));
                        }
                        else
                        {
                            instantiatedButtons.Add(buttonGameObject);
                            buttonGameObject.transform.SetParent(buttonTemplateHolder.transform, false);
                            buttonGameObject.SetActive(true);
                            UnityUIResponseButton responseButton = buttonGameObject.GetComponent<UnityUIResponseButton>();
                            SetResponseButton(responseButton, responses[i], target, buttonNumber++);
                            if (responseButton != null) buttonGameObject.name = "Response: " + responseButton.Text;

                        }
                    }
                }
                else
                {

                    // Auto-position remaining buttons:
                    if (buttonAlignment == ResponseButtonAlignment.ToFirst)
                    {

                        // Align to first, so add in order to front:
                        for (int i = 0; i < Mathf.Min(buttons.Length, responses.Length); i++)
                        {
                            if (responses[i].formattedText.position == FormattedText.NoAssignedPosition)
                            {
                                int position = Mathf.Clamp(GetNextAvailableResponseButtonPosition(0, 1), 0, buttons.Length - 1);
                                SetResponseButton(buttons[position], responses[i], target, buttonNumber++);
                            }
                        }
                    }
                    else
                    {

                        // Align to last, so add in reverse order to back:
                        for (int i = Mathf.Min(buttons.Length, responses.Length) - 1; i >= 0; i--)
                        {
                            if (responses[i].formattedText.position == FormattedText.NoAssignedPosition)
                            {
                                int position = Mathf.Clamp(GetNextAvailableResponseButtonPosition(buttons.Length - 1, -1), 0, buttons.Length - 1);
                                SetResponseButton(buttons[position], responses[i], target, buttonNumber++);
                            }
                        }
                    }
                }
            }
            NotifyContentChanged();
        }

        private void SetResponseButton(UnityUIResponseButton button, Response response, Transform target, int buttonNumber)
        {
            if (button != null)
            {
                button.visible = true;
                button.clickable = response.enabled;
                button.target = target;
                if (response != null) button.SetFormattedText(response.formattedText);
                button.response = response;

                // Auto-number:
                if (autonumber.enabled)
                {
                    button.Text = string.Format(autonumber.format, buttonNumber + 1, button.Text);
                    var keyTrigger = button.GetComponent<UIButtonKeyTrigger>();
                    if (keyTrigger == null) keyTrigger = button.gameObject.AddComponent<UIButtonKeyTrigger>();
                    keyTrigger.key = (KeyCode)((int)KeyCode.Alpha1 + buttonNumber);
                }
            }
        }

        private int GetNextAvailableResponseButtonPosition(int start, int direction)
        {
            if (buttons != null)
            {
                int position = start;
                while ((0 <= position) && (position < buttons.Length))
                {
                    if (buttons[position].visible && buttons[position].response != null)
                    {
                        position += direction;
                    }
                    else
                    {
                        return position;
                    }
                }
            }
            return 5;
        }

        private void SetupTemplateButtonNavigation()
        {
            // Assumes buttons are active (since uses GetComponent), so call after activating panel.
            for (int i = 0; i < instantiatedButtons.Count; i++)
            {
                var button = instantiatedButtons[i].GetComponent<UnityUIResponseButton>().button;
                var above = (i == 0) ? null : instantiatedButtons[i - 1].GetComponent<UnityUIResponseButton>().button;
                var below = (i == instantiatedButtons.Count - 1) ? null : instantiatedButtons[i + 1].GetComponent<UnityUIResponseButton>().button;
                var navigation = new UnityEngine.UI.Navigation();

                navigation.mode = UnityEngine.UI.Navigation.Mode.Explicit;
                navigation.selectOnUp = above;
                navigation.selectOnLeft = above;
                navigation.selectOnDown = below;
                navigation.selectOnRight = below;
                button.navigation = navigation;
            }
        }

        public void DestroyInstantiatedButtons()
        {
            foreach (var instantiatedButton in instantiatedButtons)
            {
                GameObject.Destroy(instantiatedButton);
            }
            instantiatedButtons.Clear();
            NotifyContentChanged();
        }

        public void NotifyContentChanged()
        {
            onContentChanged.Invoke();
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name='timeout'>
        /// Timeout duration in seconds.
        /// </param>
        public override void StartTimer(float timeout)
        {
            if (timer != null)
            {
                if (unityUITimer == null)
                {
                    Tools.SetGameObjectActive(timer, true);
                    unityUITimer = timer.GetComponent<UnityUITimer>();
                    if (unityUITimer == null) unityUITimer = timer.gameObject.AddComponent<UnityUITimer>();
                    Tools.SetGameObjectActive(timer, false);
                }
                if (unityUITimer != null)
                {
                    Tools.SetGameObjectActive(timer, true);
                    unityUITimer.StartCountdown(timeout, OnTimeout);
                }
                else
                {
                    if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: No UnityUITimer component found on timer", DialogueDebug.Prefix));
                }
            }
        }

        /// <summary>
        /// This method is called if the timer runs out. It selects the first response.
        /// </summary>
        public virtual void OnTimeout()
        {
            if (TimeoutHandler != null)
            {
                TimeoutHandler.Invoke();
            }
            else
            {
                DefaultTimeoutHandler();
            }
        }

        public void DefaultTimeoutHandler()
        {
            if (selectCurrentOnTimeout)
            {
                var currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UnityUIResponseButton>();
                if (currentButton != null)
                {
                    currentButton.OnClick();
                    return;
                }
            }
            DialogueManager.Instance.SendMessage("OnConversationTimeout");
        }

        /// <summary>
        /// Auto-focuses the first response. Useful for gamepads.
        /// </summary>
        public void AutoFocus()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null) return;
            if (instantiatedButtons.Count > 0)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(instantiatedButtons[0].gameObject, null);
            }
            else
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].clickable)
                    {
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(buttons[i].gameObject, null);
                        return;
                    }
                }
            }
        }

        private bool CanTriggerAnimation(string triggerName)
        {
            return CanTriggerAnimations() && !string.IsNullOrEmpty(triggerName);
        }

        private bool CanTriggerAnimations()
        {
            if ((animator == null) && !lookedForAnimator)
            {
                lookedForAnimator = true;
                if (panel != null) animator = panel.GetComponentInParent<Animator>();
            }
            return (animator != null) && (animationTransitions != null);
        }

        private IEnumerator DisableAfterAnimation(UnityEngine.UI.Graphic panel)
        {
            isHiding = true;
            if (animator != null)
            {
                const float maxWaitDuration = 10;
                float timeout = Time.realtimeSinceStartup + maxWaitDuration;
                var oldHashId = UITools.GetAnimatorNameHash(animator.GetCurrentAnimatorStateInfo(0));
                while ((UITools.GetAnimatorNameHash(animator.GetCurrentAnimatorStateInfo(0)) == oldHashId) && (Time.realtimeSinceStartup < timeout))
                {
                    yield return null;
                }
                yield return DialogueManager.Instance.StartCoroutine(DialogueTime.WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length));
            }
            isHiding = false;
            if (panel != null) Tools.SetGameObjectActive(panel, false);
        }

    }

}
#endif