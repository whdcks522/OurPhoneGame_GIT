using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.DebugUI.Table;
using static SingleInfo;

public class JSONManager : MonoBehaviour
{
    [System.Serializable]//밖에서 식별 가능하도록, public 필요 없음
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

        #region 싱글 점수 불러오기
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

        #region 싱글 점수 갱신하기 
        public void UpdateScore(SingleInfoData.SingleInfoType _singleType, int _index, int _score)
        {
            switch (_singleType)//3곳씩 수정
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

        #region 나머지 갱신
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
        //자동 저장하는 주소(debug하면 주소 나옴)
        path = Application.persistentDataPath + "/save";

        if (File.Exists(path))//파일이 존재한다면
            LoadData();
    }

    public void LoadData()//불러오기
    {
        string data = File.ReadAllText(path);//JSON -> 데이터 변환
        singleScore = JsonUtility.FromJson<SingleScore>(data);//삽입
    }

    public void SaveData(SingleInfoData.SingleInfoType _singleInfoType, int _index, int _score) //점수 저장하기
    {
        //최대 기록을 넘는 경우 초기화
        singleScore.UpdateScore(_singleInfoType, _index, _score);

        //저장
        SaveData();
    }

    public void SaveData() //그냥 저장하기
    {
        string data = JsonUtility.ToJson(singleScore);//데이터 -> JSON 변환
        File.WriteAllText(path, data);//path/save의 형식으로 data 저장
    }

    public void DataClear(int _value)//의도적으로 클리어
    {
        if (_value == 0)//완전 초기화
        {
            singleScore = new SingleScore();
        }
        else //갱신
        {
            foreach (SingleInfoData.SingleInfoType _type in System.Enum.GetValues(typeof(SingleInfoData.SingleInfoType)))//모든 것들의 점수를 수정
            {
                for (int i = 0; i <= 2; i++)
                {
                    singleScore.UpdateScore(_type, i, _value);
                }
            }
        }

        string data = JsonUtility.ToJson(singleScore);//데이터 -> JSON 변환
        File.WriteAllText(path, data);
    }
}
