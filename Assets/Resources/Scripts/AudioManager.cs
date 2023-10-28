using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Bgm")]
    public AudioClip[] bgmClips;
    AudioSource bgmPlayer;

    [Header("Sfx")]
    public AudioClip[] powerUpSfxClips;
    public AudioClip[] bigPowerUpSfxClips;
    public int channels;//만들 채널의 개수
    int curIndex;//현재 실행 중 인 플레이어 번호
    AudioSource[] sfxPlayers;

    public enum Bgm { Auth, Lobby, Entrance, Chapter1, Chapter1_BossA, Chapter2, Chapter2_BossB }//random으로 활용 가능함
    public enum Sfx {PowerUp, BigPowerUp }

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
                case Sfx.BigPowerUp:
                    tmpSfxClips = bigPowerUpSfxClips;
                    break;
            }

            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = tmpSfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    /*
    //효과음 재생
    public void PlaySfx(Sfx sfx, bool isUseRan)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + curIndex) % sfxPlayers.Length;//최근에 사용한 인덱스에서 0부터 증가해가며 가능한 것 탐색
            if (sfxPlayers[loopIndex].isPlaying) continue;//실행중이라면 continue

            switch (sfx)
            {

                case Sfx.PlayerBulletA:
                case Sfx.Paper:
                case Sfx.DoorDrag:
                case Sfx.Step:
                case Sfx.BossA:
                    sfxPlayers[loopIndex].volume = 0.8f;
                    break;
                case Sfx.BossB:
                case Sfx.DoorOpen:
                case Sfx.Impact:
                    sfxPlayers[loopIndex].volume = 0.5f;
                    break;
            }

            int ranIndex = 0;
            if (isUseRan)
            {
                int maxRanIndex = 1;//랜덤 최대치
                switch (sfx)
                {
                    case Sfx.DoorOpen:
                    case Sfx.Impact:
                    case Sfx.PlayerBulletA:
                        maxRanIndex = 6;
                        break;

                    case Sfx.Paper:
                        maxRanIndex = 5;
                        break;

                    case Sfx.DoorDrag:
                    case Sfx.Step:
                    case Sfx.BossB:
                        maxRanIndex = 3;
                        break;
                    case Sfx.BossA:
                        maxRanIndex = 2;
                        break;
                }
                ranIndex = Random.Range(0, maxRanIndex);//효과음 랜덤을 위함
            }

            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    */
}
