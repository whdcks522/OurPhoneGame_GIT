using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Bgm")]
    public AudioClip[] bgmClips;
    AudioSource bgmPlayer;

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


    [Header("만들 Sfx 채널의 개수")]
    public int channels;//

    int curIndex;//현재 실행 중 인 플레이어 번호
    AudioSource[] sfxPlayers;

    public enum Bgm { Auth, Lobby, Entrance, Chapter1, Chapter1_BossA, Chapter2, Chapter2_BossB }//random으로 활용 가능함
    public enum Sfx {PowerUp, RankUp, Damage, Heal, Door, Paper, Broken, TimeOver, Summon, Block, Warn, Typing}

    private void Awake()
    {
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

    public void PlayBgm(Bgm bgm)
    {
        bgmPlayer.Stop();
        switch (bgm)
        {
            case Bgm.Auth:
                bgmPlayer.clip = bgmClips[0];
                bgmPlayer.volume = 0.5f;
                break;
            case Bgm.Lobby:
                bgmPlayer.clip = bgmClips[1];
                bgmPlayer.volume = 1f;
                break;
            case Bgm.Entrance:
                bgmPlayer.clip = bgmClips[2];
                bgmPlayer.volume = 0.5f;
                break;
            case Bgm.Chapter1:
                bgmPlayer.clip = bgmClips[3];
                bgmPlayer.volume = 0.5f;
                break;
            case Bgm.Chapter1_BossA:
                bgmPlayer.clip = bgmClips[4];
                bgmPlayer.volume = 0.5f;
                break;
            case Bgm.Chapter2:
                bgmPlayer.clip = bgmClips[5];
                bgmPlayer.volume = 1f;
                break;
            case Bgm.Chapter2_BossB:
                bgmPlayer.clip = bgmClips[6];
                bgmPlayer.volume = 1f;
                break;
        }
        bgmPlayer.Play();
    }

    //효과음 재생
    public void PlaySfx(Sfx sfx)
    {
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
            }

            int sfxIndex = Random.Range(0, tmpSfxClips.Length);

            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
