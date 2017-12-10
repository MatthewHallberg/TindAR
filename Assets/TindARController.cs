using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.XR.iOS {
	public class TindARController : MonoBehaviour {

		public GameObject splashCanvas;
		public GameObject toggleCanvas;

		public AnimationClip leftSwipe;
		public AnimationClip rightSwipe;

		public UnityARHitTestExample hitTest;

		private bool DetectSwipe = false;

		private bool swiping = false;
		private Vector2 lastPosition;
		private Transform currentCard;
		private bool canSwipe = true;

		private Vector3 startScale;
		private Vector3 startPosition;
		private Quaternion startRotation;

		public enum SwipeDirection{
			Right,
			Left
		}

		public SwipeDirection swipe;
	
		// Use this for initialization
		void Start () {

			//set defaults with first card
			GameObject initialCard = transform.GetChild (0).gameObject;
			startScale = initialCard.transform.localScale;
			startPosition = initialCard.transform.localPosition;
			startRotation = initialCard.transform.rotation;
			//set first card active
			initialCard.SetActive (true);
			//save as current card
			currentCard = initialCard.transform;
			//show splash screen for a few seconds
			StartCoroutine (DelaySplashScreen ());
		}

		IEnumerator DelaySplashScreen(){
			splashCanvas.SetActive (true);
			toggleCanvas.SetActive (false);
			yield return new WaitForSeconds (3f);
			splashCanvas.SetActive (false);
			toggleCanvas.SetActive (true);
		}

		public void ToggleSwipe(){
			if (DetectSwipe) {
				DetectSwipe = false;
				hitTest.DetectHitTouch = true;
			} else {
				DetectSwipe = true;
				hitTest.DetectHitTouch = false;
			}
		}

		void Update () {

			if (canSwipe && DetectSwipe && Input.touchCount > 0){
				if (swiping == false){
					swiping = true;
					lastPosition = Input.GetTouch(0).position;
					return;
				} else {
					Vector2 direction = Input.GetTouch(0).position - lastPosition;

					if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
						if (direction.x > 0) {
							SwipeDetected(SwipeDirection.Right);
							StartCoroutine (CoolDownSwipe ());
						} else { 
							SwipeDetected(SwipeDirection.Left);
							StartCoroutine (CoolDownSwipe ());
						}
					}
				}
			} else {
				swiping = false;
			}
		}

		IEnumerator CoolDownSwipe(){
			canSwipe = false;
			yield return new WaitForSeconds (.5f);
			canSwipe = true;
		}

		void ResetCards(){
			foreach (Transform child in this.transform) {
				child.localPosition = startPosition;
				child.localScale = startScale;
				child.rotation = startRotation;
				child.gameObject.SetActive (false);
			}
		}

		void SwipeDetected(SwipeDirection swipe){

			int currentChildIndex = currentCard.GetSiblingIndex ();

			if (swipe == SwipeDirection.Left) {

				currentCard.GetComponent<Animation>().clip = leftSwipe; 
				currentCard.GetComponent<Animation> ().Play ();

			} else if (swipe == SwipeDirection.Right){

				currentCard.GetComponent<Animation>().clip = rightSwipe; 
				currentCard.GetComponent<Animation> ().Play ();
			}

			if (currentChildIndex == transform.childCount - 1) {
				ResetCards ();
				transform.GetChild (0).gameObject.SetActive (true);
				currentCard = transform.GetChild (0);
			} else {
				transform.GetChild (currentChildIndex + 1).gameObject.SetActive (true);
				currentCard = transform.GetChild (currentChildIndex + 1);
			}
		}
	}
}
