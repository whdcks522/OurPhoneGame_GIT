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
    public int channels;//���� ä���� ����
    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ
    AudioSource[] sfxPlayers;

    public enum Bgm { Auth, Lobby, Entrance, Chapter1, Chapter1_BossA, Chapter2, Chapter2_BossB }//random���� Ȱ�� ������
    public enum Sfx {PowerUp, BigPowerUp }

    private void Awake()
    {
        //����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();//bgmPlayer�� �����ϸ鼭 ���ÿ� ������Ʈ ����
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        //ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayers");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];//Audio Source �迭 �ʱ�ȭ
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

    //ȿ���� ���
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + curIndex) % sfxPlayers.Length;//�ֱٿ� ����� �ε������� 0���� �����ذ��� ������ �� Ž��
            if (sfxPlayers[loopIndex].isPlaying) continue;//�������̶�� continue

          

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
    //ȿ���� ���
    public void PlaySfx(Sfx sfx, bool isUseRan)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + curIndex) % sfxPlayers.Length;//�ֱٿ� ����� �ε������� 0���� �����ذ��� ������ �� Ž��
            if (sfxPlayers[loopIndex].isPlaying) continue;//�������̶�� continue

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
                int maxRanIndex = 1;//���� �ִ�ġ
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
                ranIndex = Random.Range(0, maxRanIndex);//ȿ���� ������ ����
            }

            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    */
}
