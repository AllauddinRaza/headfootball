using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public TextMesh playerName;
    public SpriteRenderer playerFaceSp;
    public Transform playerFaceSelectedObj;
    private int playerFaceIndex;
    public int PlayerFaceIndex
    {
        set
        {
            playerFaceSelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerFaceSelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.playerSprite0.Length - 1));

            playerFaceSp.sprite = AssetManager.Use.playerSprite0[value];
            playerName.text = AssetManager.Use.playersName[value];

            playerFaceIndex = value;
        }
        get
        {
            return playerFaceIndex;
        }
    }

    public SpriteRenderer playerBodySp;
    public Transform playerBodySelectedObj;
    private int playerBodyIndex;
    public int PlayerBodyIndex
    {
        set
        {
            playerBodySelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerBodySelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.playerBodySprite.Length - 1));

            playerBodySp.sprite = AssetManager.Use.playerBodySprite[value];

            playerBodyIndex = value;
        }
        get
        {
            return playerBodyIndex;
        }
    }

    public SpriteRenderer playerShoesSp1, playerShoesSp2;
    public Transform playerShoesSelectedObj;
    private int playerShoesIndex;
    public int PlayerShoesIndex
    {
        set
        {
            playerShoesSelectedObj.GetChild(1).gameObject.SetActive(!(value == 0));
            playerShoesSelectedObj.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.shoesSprites.Length - 1));

            playerShoesSp1.sprite = playerShoesSp2.sprite = AssetManager.Use.shoesSprites[value];

            playerShoesIndex = value;
        }
        get
        {
            return playerShoesIndex;
        }
    }

    public SpriteRenderer ballSp;
    private int ballIndex;
    public int BallIndex
    {
        set
        {
            ballSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));
            ballSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.ballSprites.Length - 1));

            ballSp.sprite = AssetManager.Use.ballSprites[value];

            ballIndex = value;
        }
        get
        {
            return ballIndex;
        }
    }

    public SpriteRenderer groundSp;
    private int groundIndex;
    public int GroundIndex
    {
        set
        {
            groundSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));

            groundSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.groundIconSprites.Length - 1));
            groundSp.sprite = AssetManager.Use.groundIconSprites[value];

            groundIndex = value;
        }
        get
        {
            return groundIndex;
        }
    }

    public SpriteRenderer dayAndNightSp;
    private int dayAndNightIndex;
    public int DayAndNightIndex
    {
        set
        {
            dayAndNightSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));
            dayAndNightSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.dayAndNightSprites.Length - 1));

            dayAndNightSp.sprite = AssetManager.Use.dayAndNightSprites[value];

            dayAndNightIndex = value;
        }
        get
        {
            return dayAndNightIndex;
        }
    }

    public SpriteRenderer weatherSp;
    private int weatherIndex;
    public int WeatherIndex
    {
        set
        {
            weatherSp.transform.GetChild(1).gameObject.SetActive(!(value == 0));
            weatherSp.transform.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.WeatherSprites.Length - 1));

            weatherSp.sprite = AssetManager.Use.WeatherSprites[value];

            weatherIndex = value;
        }
        get
        {
            return weatherIndex;
        }
    }

    public TextMesh gameTimeText;
    private int gameTimeIndex;
    public int GameTimeIndex
    {
        set
        {
            gameTimeText.transform.parent.GetChild(1).gameObject.SetActive(!(value == 0));
            gameTimeText.transform.parent.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.timesArray.Length - 1));

            gameTimeText.text = AssetManager.Use.timesArray[value].ToString();

            gameTimeIndex = value;
        }
        get
        {
            return gameTimeIndex;
        }
    }

    public SpriteRenderer aiLevelSp;
    private int aiLevelIndex;
    public int AiLevelIndex
    {
        set
        {
            aiLevelSp.transform.parent.GetChild(1).gameObject.SetActive(!(value == 0));
            aiLevelSp.transform.parent.GetChild(0).gameObject.SetActive(!(value >= AssetManager.Use.aiLevelSprites.Length - 1));

            aiLevelSp.sprite = AssetManager.Use.aiLevelSprites[value];

            aiLevelIndex = value;
        }
        get
        {
            return aiLevelIndex;
        }
    }

    private Transform tran;

    void Start ()
	{
        Init();
	}

    void Init()
    {
        int playerNum = PlayerPrefs.GetInt(VariablesName.PlayerNumber, 0);
        int playerBodyNum = PlayerPrefs.GetInt(VariablesName.PlayerBodyNumber, 0);
        int shoesNum = PlayerPrefs.GetInt(VariablesName.ShoesNumber, 0);

        int ballNum = PlayerPrefs.GetInt(VariablesName.BallNumber, 0);
        int groundNum = PlayerPrefs.GetInt(VariablesName.GroundNum, 0);

        int dayAndNightNum = PlayerPrefs.GetInt(VariablesName.DayAndNight, 0);
        int weatherNum = PlayerPrefs.GetInt(VariablesName.Weather, 0);

        int timeNum = PlayerPrefs.GetInt(VariablesName.Time, 0);
        int aiLevel = PlayerPrefs.GetInt(VariablesName.AILevel, 0);

        PlayerFaceIndex = playerNum;
        PlayerBodyIndex = playerBodyNum;
        PlayerShoesIndex = shoesNum;
        BallIndex = ballNum;
        GroundIndex = groundNum;
        DayAndNightIndex = dayAndNightNum;
        WeatherIndex = weatherNum;
        GameTimeIndex = timeNum;
        AiLevelIndex = aiLevel;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.Contains("PlayerFaceArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerFaceIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("PlayerFaceArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerFaceIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("PlayerBodyArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerBodyIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("PlayerBodyArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerBodyIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("ShoesArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerShoesIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("ShoesArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    PlayerShoesIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("BallsArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BallIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("BallsArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    BallIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("DayAndNightArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    DayAndNightIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("DayAndNightArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    DayAndNightIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("WeatherArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    WeatherIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("WeatherArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    WeatherIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("GroundArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GroundIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("GroundArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GroundIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("AiLevelArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    AiLevelIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("AiLevelArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    AiLevelIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("GameTimeArrowRight"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GameTimeIndex++;
                }
                else if (hit.collider.gameObject.name.Contains("GameTimeArrowLeft"))
                {
                    tran = hit.transform;
                    AnimDown();
                    GameTimeIndex--;
                }
                else if (hit.collider.gameObject.name.Contains("PlayGame"))
                {
                    tran = hit.transform;
                    AnimDown();

                    SaveAndStart();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            AnimUp();
        }
    }

    void AnimDown()
    {
        tran.localScale = new Vector3(0.9f, 0.9f, 1);
        AssetManager.Use.PlaySound(7);
    }

    void AnimUp()
    {
        if (tran != null)
        {
            tran.localScale = new Vector3(1, 1, 1);
        }
    }

    void SaveSetting()
    {
        PlayerPrefs.SetInt(VariablesName.PlayerNumber, PlayerFaceIndex);
        PlayerPrefs.SetInt(VariablesName.PlayerBodyNumber, PlayerBodyIndex);
        PlayerPrefs.SetInt(VariablesName.ShoesNumber, PlayerShoesIndex);

        PlayerPrefs.SetInt(VariablesName.BallNumber, BallIndex);

        PlayerPrefs.SetInt(VariablesName.GroundNum, GroundIndex);

        PlayerPrefs.SetInt(VariablesName.DayAndNight, DayAndNightIndex);
        PlayerPrefs.SetInt(VariablesName.Weather, WeatherIndex);

        PlayerPrefs.SetInt(VariablesName.Time, GameTimeIndex);
        PlayerPrefs.SetInt(VariablesName.AILevel, AiLevelIndex);

        PlayerPrefs.Save();
    }

    void SaveAndStart()
    {
        SaveSetting();

        SceneManager.LoadScene("Game");
    }
}
