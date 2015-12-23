using UnityEngine;

namespace LivingValkyrie.ActionBar {

    /// <summary>
    /// Author: Matt Gipson
    /// Contact: Deadwynn@gmail.com
    /// Domain: www.livingvalkyrie.com
    /// 
    /// Description: ActionBarController 
    /// </summary>
    public class ActionBarController : MonoBehaviour {
        #region Fields

        public float secondsToMove;
        public RectTransform actionBarPanel;
        public string activationKey;
        public Vector2 hiddenPos, visiblePos;
        public ActionBar actionBar;

        bool movingIn, movingOut;
        float lerpPercent = 0f;

        #endregion

        void Start() {
            //temp, may change to state machine
            movingIn = false;
            movingOut = true;
            actionBar = actionBarPanel.gameObject.GetComponent<ActionBar>();
        }

        void Update() {
            if (Input.GetButton(activationKey)) {
                movingIn = true;
                movingOut = false;
            } else {
                movingIn = false;
                movingOut = true;
            }
            MoveActionBar();
            //activate/deactivate bar
            actionBar.isActive = movingIn;
        }

        void MoveActionBar() {
            if (movingIn) {
                lerpPercent += Time.deltaTime;
                if (lerpPercent > secondsToMove) {
                    lerpPercent = secondsToMove;
                }
                float perc = lerpPercent / secondsToMove;

                actionBarPanel.anchoredPosition = Vector2.Lerp(hiddenPos, visiblePos, perc);
            } else if (movingOut) {
                lerpPercent -= Time.deltaTime;
                if (lerpPercent < 0) {
                    lerpPercent = 0;
                }
                float perc = lerpPercent / secondsToMove;

                actionBarPanel.anchoredPosition = Vector2.Lerp(hiddenPos, visiblePos, perc);
            }
        }

    }
}