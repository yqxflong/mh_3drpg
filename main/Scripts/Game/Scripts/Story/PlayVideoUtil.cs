using UnityEngine;
using System.Collections;

public class PlayVideoUtil : MonoBehaviour {

    //电影纹理
#if UNITY_EDITOR
    public MovieTexture movTexture;

    public bool Play;
#endif
    private static PlayVideoUtil m_Instance;
    public static PlayVideoUtil Instance {
        get {
            if (m_Instance == null) {
                EB.Debug.LogError("PlayVideoUtil  didnt Init");
            }
            return m_Instance;
        }
    }

    void Awake() {
        m_Instance = this;
    }

    public IEnumerator PlayVideo(System.Action callback)
    {
#if UNITY_EDITOR
        Play = true;
        while (true)
        {
            if (!Play)
            {
                var rc = callback;
                rc();
                yield break;
            }
            yield return null;
        }
#else
        //StartVideo.mp4不再需要播放
        var rc = callback;
        EB.Debug.Log("PlayVideoCoroutine: platform {0}", Application.platform);
        rc();
        yield break;

        //if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android) {
        //    EB.Debug.Log("PlayVideoCoroutine: not support for platform {0}", Application.platform);
        //        rc();
        //    yield break;
        //}

        //if (!Handheld.PlayFullScreenMovie("StartVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput)) {
        //    EB.Debug.LogWarning("PlayVideoCoroutine: failed to play StartVideo.mp4");
        //        rc();
        //    yield break;
        //}
        //yield return new WaitForEndOfFrame();
        //EB.Debug.Log("PlayVideoCoroutine: application resumed by video playback completed");
        //rc();
#endif
    }
#if UNITY_EDITOR
    void OnGUI() {
        //绘制电影纹理
        if (Play)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movTexture, ScaleMode.StretchToFill);
            movTexture.Play();
            if (GUILayout.Button(EB.Localizer.GetString("ID_codefont_in_PlayVideoUtil_1868"),GUILayout.Height(110)))
            {
                //停止播放
                Play = false;
                movTexture.Stop();
            }
            if (!movTexture.isPlaying)
            {
                Play = false;
                movTexture.Stop();
            }
        }
    }
#endif
}