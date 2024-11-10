using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            GetComponent<AudioSource>().Play();
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
