using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.DebugUI.Table;
using static SingleInfo;

public class JSONManager : MonoBehaviour
{
    [System.Serializable]
    public class SingleScore
    {

        public int[] TrainMaxScore = new int[3];
        public int[] StarFallMaxScore = new int[3];
        public int[] BlockCrashMaxScore = new int[3];
        public int[] FlyMaxScore = new int[3];

        //�̱� ���� �ҷ�����
        public int LoadScore(SingleInfo.SingleType _singleType, int _index) 
        {
            int value = 0;
            switch (_singleType) 
            {
                case SingleInfo.SingleType.Train:
                    value = TrainMaxScore[_index];
                    break;
                case SingleInfo.SingleType.StarFall:
                    value = StarFallMaxScore[_index];
                    break;
                case SingleInfo.SingleType.BlockCrash:
                    value = BlockCrashMaxScore[_index];
                    break;
                case SingleInfo.SingleType.Fly:
                    value = FlyMaxScore[_index];
                    break;
            }

            return value;
        }

        //�̱� ���� �����ϱ�
        public void UpdateScore(SingleInfo.SingleType _singleType, int _index, int _score)
        {
            switch (_singleType)//3���� ����
            {
                case SingleInfo.SingleType.Train:
                    TrainMaxScore[_index] = Mathf.Max(_score, TrainMaxScore[_index]);
                    break;
                case SingleInfo.SingleType.StarFall:
                    StarFallMaxScore[_index] = Mathf.Max(_score, StarFallMaxScore[_index]);
                    break;
                case SingleInfo.SingleType.BlockCrash:
                    BlockCrashMaxScore[_index] = Mathf.Max(_score, BlockCrashMaxScore[_index]);
                    break;
                case SingleInfo.SingleType.Fly:
                    FlyMaxScore[_index] = Mathf.Max(_score, FlyMaxScore[_index]);
                    break;
            }
        }
    }

    public SingleScore singleScore = new SingleScore();
    public string path;

    private void Awake()
    {
        //�ڵ� �����ϴ� �ּ�(debug�ϸ� �ּ� ����)
        path = Application.persistentDataPath + "/save";
        LoadData();
    }

    public void LoadData()//�ҷ�����
    {
        string data = File.ReadAllText(path);//JSON -> ������ ��ȯ
        singleScore = JsonUtility.FromJson<SingleScore>(data);//����
    }

    public void SaveData(SingleInfo.SingleType _singleType, int _index, int _score) //�����ϱ�
    {
        singleScore.UpdateScore(_singleType, _index, _score);

        string data = JsonUtility.ToJson(singleScore);//������ -> JSON ��ȯ
        File.WriteAllText(path, data);//path/save�� �������� data ����
    }

    public void DataClear(int _value)//�ǵ������� Ŭ����
    {
        if (_value == 0)//�ʱ�ȭ
        {
            singleScore = new SingleScore();
        }
        else //����
        {
            foreach (SingleType _type in System.Enum.GetValues(typeof(SingleType)))
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
