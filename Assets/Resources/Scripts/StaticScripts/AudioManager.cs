using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Bgm 플레이 여부")]
    public bool isPlayBgm = true;
    [Header("Sfx 플레이 여부")]
    public bool isPlaySfx = true;

    //Bgm 플레이어
    AudioSource bgmPlayer;

    [Header("만들 Sfx 채널의 개수")]
    public int channels;//SFX 플레이어의 갯수
    int curIndex;//현재 실행 중 인 플레이어 번호
    AudioSource[] sfxPlayers;//Sfx 플레이어

    public enum BgmStatic { Home }
    [Header("스태틱 Bgm")]
    public AudioClip[] staticBgmClips;
    public enum BgmSingle { SingleSel, Train, StarFall, BlockCrash, Fly }
    [Header("싱글 Bgm")]
    public AudioClip[] singleBgmClips;
    public enum BgmMulty { Lobby, PvP }
    [Header("멀티 Bgm")]
    public AudioClip[] multyBgmClips;


    public enum Sfx { PowerUp, RankUp, Damage, Heal, Door, Paper, Broken, TimeOver, Summon, Block, Warn, Typing, Wind }

    [Header("파워 업 Sfx")]
    public AudioClip[] powerUpSfxClips;
    [Header("랭크 업 Sfx")]
    public AudioClip[] rankUpSfxClips;
    [Header("피해 Sfx")]
    public AudioClip[] damageSfxClips;
    [Header("회복 Sfx")]
    public AudioClip[] healSfxClips;
    [Header("문 Sfx")]
    public AudioClip[] doorSfxClips;
    [Header("종이 Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("무기 깨지는 Sfx")]
    public AudioClip[] brokenSfxClips;
    [Header("시간 종료 Sfx")]
    public AudioClip[] timeOverSfxClips;
    [Header("소환 Sfx")]
    public AudioClip[] summonSfxClips;
    [Header("벽 Sfx")]
    public AudioClip[] blockSfxClips;
    [Header("경고 Sfx")]
    public AudioClip[] warnSfxClips;
    [Header("타이핑 Sfx")]
    public AudioClip[] typingSfxClips;
    [Header("바람 Sfx")]
    public AudioClip[] windSfxClips;


    BattleUIManager battleUIManager;
    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;

        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();//bgmPlayer에 저장하면서 동시에 컴포넌트 삽입
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayers");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];//Audio Source 배열 초기화
        for (int index = 0; index < channels; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }
    }

    private void Start()
    {
        isPlayBgm = battleUIManager.jsonManager.customJSON.isPlayBgm;
        isPlaySfx = battleUIManager.jsonManager.customJSON.isPlaySfx;
    }

    public void StopBgm() //배경 음악 멈추기
    {
        bgmPlayer.Stop();
    }

    public void PlayBgm(BgmStatic _bgm)//스태틱 BGM 재생
    {
        if (!isPlayBgm) return;

        bgmPlayer.Stop();
        switch (_bgm)
        {
            case BgmStatic.Home:
                bgmPlayer.clip = staticBgmClips[0];
                //bgmPlayer.volume = 0.5f;
                break;
        }
        bgmPlayer.Play();
    }

    public void PlayBgm(BgmSingle _bgm)
    {
        if (!isPlayBgm) return;

        bgmPlayer.Stop();
        switch (_bgm)
        {
            case BgmSingle.SingleSel:
                bgmPlayer.clip = singleBgmClips[0];
                break;
            case BgmSingle.Train:
                bgmPlayer.clip = singleBgmClips[1];
                break;
            case BgmSingle.StarFall:
                bgmPlayer.clip = singleBgmClips[2];
                break;
            case BgmSingle.BlockCrash:
                bgmPlayer.clip = singleBgmClips[3];
                break;
            case BgmSingle.Fly:
                bgmPlayer.clip = singleBgmClips[4];
                break;
        }
        bgmPlayer.Play();
    }

    public void PlayBgm(BgmMulty _bgm)
    {
        if (!isPlayBgm) return;

        bgmPlayer.Stop();
        switch (_bgm)
        {
            case BgmMulty.Lobby:
                bgmPlayer.clip = multyBgmClips[0];
                break;
            case BgmMulty.PvP:
                bgmPlayer.clip = multyBgmClips[1];
                break;
        }
        bgmPlayer.Play();
    }


    #region SFX 재생
    public void PlaySfx(Sfx sfx)
    {
        if (!isPlaySfx) return;

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + curIndex) % sfxPlayers.Length;//최근에 사용한 인덱스에서 0부터 증가해가며 가능한 것 탐색
            if (sfxPlayers[loopIndex].isPlaying) continue;//실행중이라면 continue

            AudioClip[] tmpSfxClips = null;
            switch (sfx)
            {
                case Sfx.PowerUp:
                    tmpSfxClips = powerUpSfxClips;
                    break;
                case Sfx.RankUp:
                    tmpSfxClips = rankUpSfxClips;
                    break;
                case Sfx.Damage:
                    tmpSfxClips = damageSfxClips;
                    break;
                case Sfx.Heal:
                    tmpSfxClips = healSfxClips;
                    break;
                case Sfx.Door:
                    tmpSfxClips = doorSfxClips;
                    break;
                case Sfx.Paper:
                    tmpSfxClips = paperSfxClips;
                    break;
                case Sfx.Broken:
                    tmpSfxClips = brokenSfxClips;
                    break;
                case Sfx.TimeOver:
                    tmpSfxClips = timeOverSfxClips;
                    break;
                case Sfx.Summon:
                    tmpSfxClips = summonSfxClips;
                    break;
                case Sfx.Block:
                    tmpSfxClips = blockSfxClips;
                    break;
                case Sfx.Warn:
                    tmpSfxClips = warnSfxClips;
                    break;
                case Sfx.Typing:
                    tmpSfxClips = typingSfxClips;
                    break;
                case Sfx.Wind:
                    tmpSfxClips = windSfxClips;
                    break;
            }

            int sfxIndex = Random.Range(0, tmpSfxClips.Length);

            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    #endregion
}
