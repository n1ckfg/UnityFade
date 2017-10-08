using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class FaderPp : MonoBehaviour {

    public enum StartMode { FADE_IN, FADE_OUT, NONE };
    public StartMode startMode = StartMode.FADE_IN;
	public enum FadeMode { FADE, TEMP, FADE_TEMP };
	public FadeMode fadeMode = FadeMode.FADE;
	public float fadeTime = 3f;
	//public float startDelay = 1f;
    public bool debug = false;
	public AudioSource audio;

	private float maxFade = 0f;
	private float minFade = -8f;
	private float maxTemp = 0f;
	private float minTemp = -85f;
	private float maxAudio = 1f;
	private float minAudio = 0f;

    private bool isBlocked = false;
    private PostProcessingProfile pp;
	private float fadeVal = 0f; 
	private float tempVal = 0f;
	private float audioVal = 0f;
	private float audioThresh = 0.0001f;

	private bool doAudio = false;

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
		if (audio != null) {
			doAudio = true;
			maxAudio = audio.volume;
			audio.Pause();
		}
			
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

		switch (fadeMode) {
			case (FadeMode.FADE):
		        doFadeSettings(maxFade);

		        while (fadeVal > minFade) {
					setNewFadeOut(_fadeTime);
		            doFadeSettings(fadeVal);
					if (doAudio) setNewAudioOut(_fadeTime);

		            yield return new WaitForSeconds(0);
		        }
				break;
			case (FadeMode.TEMP):
				doTempSettings(maxTemp);

				while (tempVal > minTemp) {
					setNewTempOut(_fadeTime);
					doTempSettings(tempVal);
					if (doAudio) setNewAudioOut(_fadeTime);

					yield return new WaitForSeconds(0);
				}
				break;
			case (FadeMode.FADE_TEMP):
			doFadeTempSettings(maxFade, maxTemp);

			while (fadeVal > minFade || tempVal > minTemp) {
				setNewFadeOut(_fadeTime);
				setNewTempOut(_fadeTime);
				doFadeTempSettings(fadeVal, tempVal);
				if (doAudio) setNewAudioOut(_fadeTime);

				yield return new WaitForSeconds(0);
			}
			break;
		}

		if (doAudio) audio.Pause();
        isBlocked = false;
    }

    private IEnumerator doFadeIn(float _fadeTime) {
        isBlocked = true;
		if (doAudio) audio.Play();

		switch (fadeMode) {
			case (FadeMode.FADE):
		        doFadeSettings(minFade);

		        while (fadeVal < maxFade) {
					setNewFadeIn(_fadeTime);
		            doFadeSettings(fadeVal);
					if (doAudio) setNewAudioIn(_fadeTime);

		            yield return new WaitForSeconds(0);
		        }
				break;
			case (FadeMode.TEMP):
				doTempSettings(minTemp);

				while (tempVal < maxTemp) {
					setNewTempIn(_fadeTime);
					doTempSettings(tempVal);
					if (doAudio) setNewAudioIn(_fadeTime);

					yield return new WaitForSeconds(0);
				}
				break;
			case (FadeMode.FADE_TEMP):
			doFadeTempSettings(minFade, minTemp);

			while (fadeVal < maxFade || tempVal < maxTemp) {
				setNewFadeIn(_fadeTime);
				setNewTempIn(_fadeTime);
				doFadeTempSettings(fadeVal, tempVal);
				if (doAudio) setNewAudioIn(_fadeTime);

				yield return new WaitForSeconds(0);
			}
			break;
		}

        isBlocked = false;
    }

	// ~ ~ ~

	private void setNewFadeIn(float _fadeTime) {
		fadeVal += getDelta(_fadeTime, maxFade, minFade);
		if (fadeVal > maxFade) fadeVal = maxFade;
	}

	private void setNewFadeOut(float _fadeTime) {
		fadeVal -= getDelta(_fadeTime, maxFade, minFade);
		if (fadeVal < minFade) fadeVal = minFade;
	}

	private void setNewTempIn(float _fadeTime) {
		tempVal += getDelta(_fadeTime, maxTemp, minTemp);
		if (tempVal > maxTemp) tempVal = maxTemp;	
	}

	private void setNewTempOut(float _fadeTime) {
		tempVal -= getDelta(_fadeTime, maxTemp, minTemp);
		if (tempVal < minTemp) tempVal = minTemp;
	}

	private void setNewAudioIn(float _fadeTime) {
		audioVal += getDelta(_fadeTime, maxAudio, minAudio);
		if (audioVal > maxAudio) audioVal = maxAudio;
		audio.volume = audioVal;
	}

	private void setNewAudioOut(float _fadeTime) {
		audioVal -= getDelta(_fadeTime, maxAudio, minAudio);
		if (audioVal < minAudio) audioVal = minAudio;
		audio.volume = audioVal;
	}

	// ~ ~ ~ 

    private float getDelta(float _time, float _max, float _min) {
        return Mathf.Abs(_max-_min) / (_time * (1f / Time.deltaTime));
    }

	// ~ ~ ~

    private void doFadeSettings(float exp) {
        fadeVal = exp;
        var colorGrading = pp.colorGrading.settings;
        colorGrading.basic.postExposure = exp;
        pp.colorGrading.settings = colorGrading;
    }

	private void doTempSettings(float temp) {
		tempVal = temp;
		var colorGrading = pp.colorGrading.settings;
		colorGrading.basic.temperature = temp;
		pp.colorGrading.settings = colorGrading;
	}

	private void doFadeTempSettings(float exp, float temp) {
		fadeVal = exp;
		tempVal = temp;
		var colorGrading = pp.colorGrading.settings;
		colorGrading.basic.postExposure = exp;
		colorGrading.basic.temperature = temp;
		pp.colorGrading.settings = colorGrading;
	}

}