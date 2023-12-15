using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.DebugUI.Table;
using static SingleInfo;

public class JSONManager : MonoBehaviour
{
    [System.Serializable]//�ۿ��� �ĺ� �����ϵ���, public �ʿ� ����
    public class SingleScore
    {
        public enum SettingType {Bgm, Sfx }

        public bool isPlayBgm = true;
        public bool isPlaySfx = true;

        public int[] TrainMaxScore = new int[3];
        public int[] StarFallMaxScore = new int[3];
        public int[] BlockMaxScore = new int[3];
        public int[] FlyMaxScore = new int[3];
        public int[] DogMaxScore = new int[3];

        #region �̱� ���� �ҷ�����
        public int LoadScore(SingleInfoData.SingleInfoType _singleInfoType, int _index) 
        {
            int value = 0;
            switch (_singleInfoType) 
            {
                case SingleInfoData.SingleInfoType.Train:
                    value = TrainMaxScore[_index];
                    break;
                case SingleInfoData.SingleInfoType.StarFall:
                    value = StarFallMaxScore[_index];
                    break;
                case SingleInfoData.SingleInfoType.Block:
                    value = BlockMaxScore[_index];
                    break;
                case SingleInfoData.SingleInfoType.Fly:
                    value = FlyMaxScore[_index];
                    break;
                case SingleInfoData.SingleInfoType.Dog:
                    value = DogMaxScore[_index];
                    break;
            }
            return value;
        }
        #endregion

        #region �̱� ���� �����ϱ� 
        public void UpdateScore(SingleInfoData.SingleInfoType _singleType, int _index, int _score)
        {
            switch (_singleType)//3���� ����
            {
                case SingleInfoData.SingleInfoType.Train:
                    TrainMaxScore[_index] = Mathf.Max(_score, TrainMaxScore[_index]);
                    break;
                case SingleInfoData.SingleInfoType.StarFall:
                    StarFallMaxScore[_index] = Mathf.Max(_score, StarFallMaxScore[_index]);
                    break;
                case SingleInfoData.SingleInfoType.Block:
                    BlockMaxScore[_index] = Mathf.Max(_score, BlockMaxScore[_index]);
                    break;
                case SingleInfoData.SingleInfoType.Fly:
                    FlyMaxScore[_index] = Mathf.Max(_score, FlyMaxScore[_index]);
                    break;
                case SingleInfoData.SingleInfoType.Dog:
                    DogMaxScore[_index] = Mathf.Max(_score, DogMaxScore[_index]);
                    break;
            }
        }
        #endregion

        #region ������ ����
        public void UpdateOther(SettingType _settingType, int _index)
        {
            switch (_settingType)
            {
                case SettingType.Bgm:
                    isPlayBgm = _index == 1 ? true : false;
                    break;
                case SettingType.Sfx:
                    isPlaySfx = _index == 1 ? true : false;
                    break;
            }
        }
        #endregion
    }

    public SingleScore singleScore = new SingleScore();
    string path;

    private void Awake()
    {
        //�ڵ� �����ϴ� �ּ�(debug�ϸ� �ּ� ����)
        path = Application.persistentDataPath + "/save";

        if (File.Exists(path))//������ �����Ѵٸ�
            LoadData();
    }

    public void LoadData()//�ҷ�����
    {
        string data = File.ReadAllText(path);//JSON -> ������ ��ȯ
        singleScore = JsonUtility.FromJson<SingleScore>(data);//����
    }

    public void SaveData(SingleInfoData.SingleInfoType _singleInfoType, int _index, int _score) //���� �����ϱ�
    {
        //�ִ� ����� �Ѵ� ��� �ʱ�ȭ
        singleScore.UpdateScore(_singleInfoType, _index, _score);

        //����
        SaveData();
    }

    public void SaveData() //�׳� �����ϱ�
    {
        string data = JsonUtility.ToJson(singleScore);//������ -> JSON ��ȯ
        File.WriteAllText(path, data);//path/save�� �������� data ����
    }

    public void DataClear(int _value)//�ǵ������� Ŭ����
    {
        if (_value == 0)//���� �ʱ�ȭ
        {
            singleScore = new SingleScore();
        }
        else //����
        {
            foreach (SingleInfoData.SingleInfoType _type in System.Enum.GetValues(typeof(SingleInfoData.SingleInfoType)))//��� �͵��� ������ ����
            {
                for (int i = 0; i <= 2; i++)
                {
                    singleScore.UpdateScore(_type, i, _value);
                }
            }
        }

        string data = JsonUtility.ToJson(singleScore);//������ -> JSON ��ȯ
        File.WriteAllText(path, data);
    }
}
