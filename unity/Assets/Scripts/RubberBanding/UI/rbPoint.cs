namespace RubberBanding
{
    using UnityEngine;

    public class rbPoint : MonoBehaviour {

        public Vector2 Pos { get; private set; }
        public bool Removed { get; set; }

        private rbController rb_controller;

        // True if the point is deleted from the point set by the players
        public bool deleted;

        public rbPoint(Vector2 pos){
            Pos = pos;
            Removed = false;
        }
        private void Awake()
        {
            Pos = new Vector2(transform.position.x, transform.position.y);
            Removed = false;
            rb_controller = FindObjectOfType<rbController>();
        }

        private void OnMouseDown()
        {
            /*
            * TODO: 
            * 1) make this point dissappear
            * 2) Update CH, score appropriately
            */

            // Inform the controller that this point was clicked.
            if (!Removed) {
                rb_controller.rb_chosenPoint = this;
                Destroy(gameObject);
            }
        }

        private void OnMouseEnter()
        {
            /* 
            * TODO: provide overview of what surface would
            * be added to score if this object was selected.
            */      
        }

        private void OnMouseExit()
        {
            /*
            * TODO: stop drawing score update
            */
        }
    }
}


