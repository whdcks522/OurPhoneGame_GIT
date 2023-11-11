using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Bgm")]
    public AudioClip[] bgmClips;
    AudioSource bgmPlayer;

    [Header("�Ŀ� �� Sfx")]
    public AudioClip[] powerUpSfxClips;
    [Header("��ũ �� Sfx")]
    public AudioClip[] rankUpSfxClips;
    [Header("���� Sfx")]
    public AudioClip[] damageSfxClips;
    [Header("ȸ�� Sfx")]
    public AudioClip[] healSfxClips;
    [Header("�� Sfx")]
    public AudioClip[] doorSfxClips;
    [Header("���� Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("���� ������ Sfx")]
    public AudioClip[] brokenSfxClips;
    [Header("�ð� ���� Sfx")]
    public AudioClip[] timeOverSfxClips;
    [Header("��ȯ Sfx")]
    public AudioClip[] summonSfxClips;
    [Header("�� Sfx")]
    public AudioClip[] blockSfxClips;
    [Header("��� Sfx")]
    public AudioClip[] warnSfxClips;
    [Header("Ÿ���� Sfx")]
    public AudioClip[] typingSfxClips;


    [Header("���� Sfx ä���� ����")]
    public int channels;//

    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ
    AudioSource[] sfxPlayers;

    public enum Bgm { Auth, Lobby, Entrance, Chapter1, Chapter1_BossA, Chapter2, Chapter2_BossB }//random���� Ȱ�� ������
    public enum Sfx {PowerUp, RankUp, Damage, Heal, Door, Paper, Broken, TimeOver, Summon, Block, Warn, Typing}

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
