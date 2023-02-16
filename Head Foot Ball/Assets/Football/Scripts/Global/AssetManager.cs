using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Use;

    public string[] playersName;
    public Sprite[] playerSprite0, playerSprite1, playerBodySprite, shoesSprites;

    public Sprite[] ballSprites, groundSprites, groundIconSprites;

    public Sprite[] dayAndNightSprites, WeatherSprites;
    public int[] timesArray;
    public Sprite[] aiLevelSprites;

    /// In Game
    public Sprite[] standsSprites;

    /// Audio
    public AudioClip ram_hit, bounce1, bounce2, net_sound;
    public AudioClip clap1, clap2, clap3, clap4;
    public AudioClip clickSound, startGameSound, endGameSound;
    public AudioClip[] weatherSound;

    private float volume = 1.0f;
    private AudioSource backgroundMusic, backgroundMusic2, backgroundEffect, sfxSound;

    void Awake()
    {
        //PlayerPrefs.DeleteAll();

        DontDestroyOnLoad(gameObject);

        if (Use == null)
        {
            Use = this;
            SceneManager.activeSceneChanged += SceneLoaded; // subscribe

            backgroundMusic = transform.GetChild(0).GetComponent<AudioSource>();
            backgroundMusic2 = transform.GetChild(1).GetComponent<AudioSource>();
            backgroundEffect = transform.GetChild(2).GetComponent<AudioSource>();
            sfxSound = transform.GetChild(3).GetComponent<AudioSource>();

            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        backgroundMusic.mute = !(PlayerPrefs.GetInt(VariablesName.Music, 1) == 1);
    }

    public void SetSoundVolume()
    {
        sfxSound.mute = !(PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1);

        backgroundMusic2.mute = !((SceneManager.GetActiveScene().name == "Game") && (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1));
        backgroundEffect.mute = !((SceneManager.GetActiveScene().name == "Game") && (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1));
    }

    public void BackgroundEffectSound()
    {
        backgroundEffect.clip = weatherSound[PlayerPrefs.GetInt(VariablesName.Weather, 0)];
        backgroundEffect.Play();
    }

    //////////////////////////////////////

    /// <summary>
    /// 1 : netSound -- 
    /// 2,3 : ram --
    /// 4,5 : bounce ground wall --
    /// 6 : random clap --
    /// 7 : click -- 
    /// 8 : startGame -- 
    /// 9 : endGame
    /// </summary>
    public void PlaySound(int id)
    {
        switch (id)
        {
            case 1: // netSound
                sfxSound.PlayOneShot(net_sound, volume);
                break;

            case 2: // ram
                sfxSound.PlayOneShot(ram_hit, volume);
                break;

            case 3: // ram
                sfxSound.PlayOneShot(ram_hit, volume / 2);
                break;

            case 4: // bounce ground wall
                if (Random.Range(0, 2.1f) > 1)
                    sfxSound.PlayOneShot(bounce1, volume);
                else
                    sfxSound.PlayOneShot(bounce2, volume);
                break;

            case 5: // bounce ground wall
                if (Random.Range(0, 2.1f) > 1)
                    sfxSound.PlayOneShot(bounce1, volume / 2);
                else
                    sfxSound.PlayOneShot(bounce2, volume / 2);
                break;

            case 6: // clap
                switch (Random.Range(0, 4))
                {
                    case 0:
                        sfxSound.PlayOneShot(clap1, volume);
                        break;

                    case 1:
                        sfxSound.PlayOneShot(clap2, volume);
                        break;

                    case 2:
                        sfxSound.PlayOneShot(clap3, volume);
                        break;

                    case 3:
                        sfxSound.PlayOneShot(clap4, volume);
                        break;
                }
                break;

            case 7: // click
                sfxSound.PlayOneShot(clickSound, volume);
                break;

            case 8: // startGame
                sfxSound.PlayOneShot(startGameSound, volume);
                break;

            case 9: // endGame
                sfxSound.PlayOneShot(endGameSound, volume);
                break;
        }
    }

    void SceneLoaded(Scene previousScene, Scene newScene)
    {
        if (Use != null)
        {
            backgroundMusic2.mute = !((newScene.name == "Game") && (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1));
            backgroundEffect.mute = !((newScene.name == "Game") && (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1));
        }
    }
}

public enum GameMode
{
    AWAKE,
    START,
    PAUSE,
    SLOWMOTION,
    PLAY,
    END
};