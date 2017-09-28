using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour {

	public Renderer ren;
	public bool fadeOnStart = true;
	public float fadeTime = 3f;
	public bool debug = false;

	private bool isBlocked = false;
	private float alphaVal = 1f;

	private void Start() {
		if (fadeOnStart) fadeIn();
	}

	private void Update() {
		if (debug) {
			if (Input.GetKeyDown(KeyCode.F)) {
				fadeIn();
			} else if (Input.GetKeyDown(KeyCode.G)) {
				fadeOut();
			}
		}
	}

	public void fadeIn() {
		if (!isBlocked) StartCoroutine(doFadeIn(fadeTime));
	}

	public void fadeIn(float _fadeTime) {
		if (!isBlocked) StartCoroutine(doFadeIn(_fadeTime));
	}

	public void fadeOut() {
		if (!isBlocked) StartCoroutine(doFadeOut(fadeTime));
	}

	public void fadeOut(float _fadeTime) {
		if (!isBlocked) StartCoroutine(doFadeOut(_fadeTime));
	}

	private IEnumerator doFadeIn(float _fadeTime) {
		isBlocked = true;
			
		while (alphaVal > 0f) {
			alphaVal -= getAlphaDelta(_fadeTime);
			if (alphaVal < 0f) alphaVal = 0f;
			ren.material.SetColor("_Color", new Color(0f, 0f, 0f, alphaVal)); 
			yield return new WaitForSeconds(0);
		}

		ren.enabled = false;
		isBlocked = false;		
	}

	private IEnumerator doFadeOut(float _fadeTime) {
		ren.enabled = true;
		isBlocked = true;

		while (alphaVal < 1f) {
			alphaVal += getAlphaDelta(_fadeTime);
			if (alphaVal > 1f) alphaVal = 1f;
			ren.material.SetColor("_Color", new Color(0f, 0f, 0f, alphaVal)); 
			yield return new WaitForSeconds(0);
		}

		isBlocked = false;	
	}

	private float getAlphaDelta(float _fadeTime) {
		return 1f/(_fadeTime * (1f/Time.deltaTime));
	}


}
