namespace RubberBanding
{
    using UnityEngine;
    using UnityEngine.UI;

    public class rbScore : MonoBehaviour {

        public static int score;
        private rbController rb_controller;

        public GameObject rb_scoretext;

        private void Start()
        {
            rb_scoretext.GetComponent<Text>().text = "0";
            rb_controller = FindObjectOfType<rbController>();
            score = 0;
        }

        private void Update() {
            //SetScore(rb_controller.player.score);
        }
        public void SetScore(int newScore)
        {
            rb_scoretext.GetComponent<Text>().text = newScore.ToString("F0");
        }
    }
}


