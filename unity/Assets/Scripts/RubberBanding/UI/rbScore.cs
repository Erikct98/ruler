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
            score = 0;
        }
        public void SetScore(int newScore)
        {
            rb_scoretext.GetComponent<Text>().text = newScore.ToString("F0");
        }
    }
}


