using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class FaderPp : MonoBehaviour {

    public enum StartMode { FADE_IN, FADE_OUT, NONE };
    public StartMode startMode = StartMode.FADE_IN;
    public float fadeTime = 3f;
    public bool debug = false;

    private bool isBlocked = false;
    private float expVal = 0f;
    private PostProcessingProfile pp;
    private float maxExp = 0f;
    private float minExp = -8f;

    private void OnEnable() {
        var behavior = GetComponent<PostProcessingBehaviour>();

        if (behavior.profile == null) {
            enabled = false;
            return;
        }

        pp = Instantiate(behavior.profile);
        behavior.profile = pp;
    }

    private void Start() {
        if (startMode == StartMode.FADE_IN) {
            fadeIn();
        } else if (startMode == StartMode.FADE_OUT) {
            fadeOut();
        }
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

    private IEnumerator doFadeOut(float _fadeTime) {
        isBlocked = true;
        doExpSettings(maxExp);

        while (expVal > minExp) {
            expVal -= getExpDelta(_fadeTime);
            if (expVal < minExp) expVal = minExp;
            doExpSettings(expVal);

            yield return new WaitForSeconds(0);
        }

        isBlocked = false;
    }

    private IEnumerator doFadeIn(float _fadeTime) {
        isBlocked = true;
        doExpSettings(minExp);

        while (expVal < maxExp) {
            expVal += getExpDelta(_fadeTime);
            if (expVal > maxExp) expVal = maxExp;
            doExpSettings(expVal);

            yield return new WaitForSeconds(0);
        }

        isBlocked = false;
    }

    private float getExpDelta(float _fadeTime) {
        return Mathf.Abs(maxExp-minExp) / (_fadeTime * (1f / Time.deltaTime));
    }


    private void doExpSettings(float exp) {
        expVal = exp;
        var colorGrading = pp.colorGrading.settings;
        colorGrading.basic.postExposure = exp;
        pp.colorGrading.settings = colorGrading;
    }

}