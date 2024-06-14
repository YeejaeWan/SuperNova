using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//test
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;
    public Spawner spawner;

    [Header("# Stages")]
    public GameObject[] stage1Tilemaps;
    public GameObject[] stage2Tilemaps;

    [Header("# UI Elements")]
    public GameObject stageSelectUI;
    public GameObject characterSelectUI;
    public GameObject backButton;
    public GameObject continueButton;
    public GameObject endButton;
    public GameObject settingMenu; // 설정 메뉴 오브젝트
    public GameObject Darkbackground;
    public GameObject healthop;

    private int selectedStage = 1; // 선택된 스테이지를 저장하는 변수

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);
        spawner.SetStageSpawnData(selectedStage); // 선택된 스테이지의 스폰 데이터를 설정
        Resume();
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }


    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        SceneManager.LoadScene(0); // 게임 시작 화면으로 돌아가기
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }

    public void ActivateStage(int stageNumber)
    {
        Debug.Log("Activating stage: " + stageNumber); // 디버그 로그 추가
        selectedStage = stageNumber; // 선택된 스테이지를 저장
        if (stageNumber == 1)
        {
            SetActiveTilemaps(stage1Tilemaps, true);
            SetActiveTilemaps(stage2Tilemaps, false);
        }
        else if (stageNumber == 2)
        {
            SetActiveTilemaps(stage1Tilemaps, false);
            SetActiveTilemaps(stage2Tilemaps, true);
        }
        spawner.SetStageSpawnData(stageNumber); // 선택된 스테이지의 스폰 데이터를 설정
    }

    private void SetActiveTilemaps(GameObject[] tilemaps, bool isActive)
    {
        foreach (GameObject tilemap in tilemaps)
        {
            tilemap.SetActive(isActive);
        }
    }

    public void ShowCharacterSelect()
    {
        stageSelectUI.SetActive(false);
        characterSelectUI.SetActive(true);
        backButton.SetActive(true);
    }

    public void ShowStageSelect()
    {
        characterSelectUI.SetActive(false);
        stageSelectUI.SetActive(true);
        backButton.SetActive(false);

    }

    public void ShowSettingMenu()
    {
        Darkbackground.SetActive(true);
        settingMenu.SetActive(true);
        continueButton.SetActive(true);
        endButton.SetActive(true);
        healthop.SetActive(false);
        Time.timeScale = 0; // 게임 일시 정지
    }

    public void HideSettingMenu()
    {
        Darkbackground.SetActive(false);
        settingMenu.SetActive(true);
        continueButton.SetActive(false);
        endButton.SetActive(false);
        healthop.SetActive(true);
        Time.timeScale = 1; // 게임 재개
    }

    void SetBackButtonSize()
    {
        if (backButton != null)
        {
            LayoutElement layoutElement = backButton.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = backButton.AddComponent<LayoutElement>();
            }
        }
    }
}
