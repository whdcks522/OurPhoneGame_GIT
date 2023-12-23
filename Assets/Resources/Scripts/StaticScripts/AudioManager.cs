using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Bgm �÷��� ����")]
    public bool isPlayBgm = true;
    [Header("Sfx �÷��� ����")]
    public bool isPlaySfx = true;

    //Bgm �÷��̾�
    AudioSource bgmPlayer;

    [Header("���� Sfx ä���� ����")]
    public int channels;//SFX �÷��̾��� ����
    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ
    AudioSource[] sfxPlayers;//Sfx �÷��̾�

    public enum BgmStatic { Home }
    [Header("����ƽ Bgm")]
    public AudioClip[] staticBgmClips;
    public enum BgmSingle { SingleSel, Train, StarFall, BlockCrash, Fly }
    [Header("�̱� Bgm")]
    public AudioClip[] singleBgmClips;
    public enum BgmMulty { Lobby, PvP }
    [Header("��Ƽ Bgm")]
    public AudioClip[] multyBgmClips;


    public enum Sfx { PowerUp, RankUp, Damage, Heal, Door, Paper, Broken, TimeOver, Summon, Block, Warn, Typing, Wind }

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
    [Header("�ٶ� Sfx")]
    public AudioClip[] windSfxClips;


    BattleUIManager battleUIManager;
    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;

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

    private void Start()
    {
        isPlayBgm = battleUIManager.jsonManager.customJSON.isPlayBgm;
        isPlaySfx = battleUIManager.jsonManager.customJSON.isPlaySfx;
    }

    public void StopBgm() //��� ���� ���߱�
    {
        bgmPlayer.Stop();
    }

    public void PlayBgm(BgmStatic _bgm)//����ƽ BGM ���
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


    #region SFX ���
    public void PlaySfx(Sfx sfx)
    {
        if (!isPlaySfx) return;

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
