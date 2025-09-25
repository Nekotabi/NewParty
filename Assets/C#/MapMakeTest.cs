using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapMakeTest : MonoBehaviour
{
    #region Public宣言
    [Tooltip("マップの階層")] public int StageNum;
    [Tooltip("地面")] public List<GameObject> Grounds;
    [Tooltip("壁・天井")] public List<GameObject> Walls;
    [Tooltip("トンネル")] public List<GameObject> Tunnels;

    #endregion
    #region Private宣言
    private List<TextAsset> CSVList = new();//CSVの保管用
    private List<List<List<string>>> FieldFile = new(); //生成パターン保存用
    private List<List<List<int>>> MakeIt = new();   //生成補助パターン保存用
    #endregion

    void Awake()
    {
        //CSV系
        if (StageNum != 0)
        {
            for (int i = 0; i < StageNum; i++)
            {
                TextAsset Map = Resources.Load<TextAsset>("Map" + i);
                if (Map != null)
                {
                    FieldFile.Add(CSVRead(Map));
                }
                else
                    Debug.LogError("Map" + i + "が見つかりませんでした");
            }
        }
        else
            Debug.Log("ステージの数を設定してください");
    }

    private void Start()
    {
        for (int y = 0; y < FieldFile.Count; y++)
        {
            for (int x = 0; x < FieldFile[y].Count; x++)
            {
                for (int z = 0; z < FieldFile[y][x].Count; z++)
                {
                    Debug.Log("(" + y + "," + x + "," + z + ")");
                    BuildMap(FieldFile[y][x][z], new Vector3Int(x, y, z));
                }
            }
        }
    }

    /// <summary>
    /// CSVファイルを2次元リストに変換する。
    /// </summary>
    /// <param name="file">InputFile</param>
    /// <returns>List(TwoDimenshonal)</returns>
    private List<List<string>> CSVRead(TextAsset file)
    {
        List<List<string>> MapDatas = new();//FieldFileに渡す用
        List<List<int>> LogNums = new();//生成判定用
        var StringReader = new StringReader(file.text); //TextAssetをTextに変換
        string line = StringReader.ReadLine();  //1行読む
        int LogNumber = 0;//LogNumsの構築用

        while (line != null) //lineをコンマ分けにしてMapDataに入れる
        {
            string[] column = line.Split(",");
            List<string> KariData = new List<string>(column);
            MapDatas.Add(KariData);

            //columnと同じ要素数の列を用意させる
            List<int> KariNum = new();
            for (int i = 0; i < KariData.Count; i++)
                KariNum.Add(0);

            //次の準備
            LogNums.Add(KariNum);
            line = StringReader.ReadLine();
            LogNumber++;
        }

        //隣のものに応じて状態を保存する
        for (int i = 0; i < MapDatas.Count; i++)
        {

            for (int j = 0; j < MapDatas[i].Count; j++)
            {
                Debug.Log(i + "_" + j);
                if (i != 0)
                {
                    if (MapDatas[i][j] == MapDatas[i - 1][j])
                    {
                        LogNums[i][j] += 1;
                        LogNums[i - 1][j] += 4;
                    }
                }
                if (j != 0)
                {
                    if (MapDatas[i][j] == MapDatas[i][j - 1])
                    {
                        LogNums[i][j] += 2;
                        LogNums[i][j - 1] += 8;
                    }
                }
            }
        }
        MakeIt.Add(LogNums);

        return MapDatas;
    }

    /// <summary>
    /// オブジェクトユニットを生成する。
    /// </summary>
    /// <param name="MakeNum"></param>
    /// <param name="MakePos"></param>
    private void BuildMap(string MakeNum, Vector3Int FilePass)
    {
        string[] StrNums = MakeNum.Split("_");//"_"で主番号と付番号を分解
        int LogNum = MakeIt[FilePass.y][FilePass.x][FilePass.z];//判定する数値を格納
        string[] DirectObjNum = DirectDistinction(LogNum, FilePass);//周囲のオブジェクトを検索
        int MobNum = 0;//オブジェの数値を保管する。0は何もなし。

        if (StrNums.Length == 2)
            MobNum = int.Parse(StrNums[1]);

        MakeObject(int.Parse(StrNums[0]), MobNum, MakeNum, FilePass, DirectObjNum);
    }

    /// <summary>
    /// 上下左右の順の隣り合ったデータを返す。
    /// </summary>
    /// <param name="LogNum"></param>
    /// <returns></returns>
    private string[] DirectDistinction(int DirectLogNum, Vector3Int FilePass)
    {
        string[] Objs = new string[4] { "null", "null", "null", "null" };
        int x = FilePass.x, y = FilePass.y, z = FilePass.z;
        if (DirectLogNum >= 8)//右方向の確認
        {
            Objs[3] = FieldFile[y][x][z + 1];
            DirectLogNum -= 8;
        }
        if (DirectLogNum >= 4)//下方向の確認
        {
            Objs[1] = FieldFile[y][x + 1][z];
            DirectLogNum -= 4;
        }
        if (DirectLogNum >= 2)//左方向の確認
        {
            Objs[2] = FieldFile[y][x][z - 1];
            DirectLogNum -= 2;
        }
        if (DirectLogNum >= 1)//上方向の確認
        {
            Objs[0] = FieldFile[y][x - 1][z];
            DirectLogNum -= 1;
        }

        return Objs;
    }

    /// <summary>
    /// 生成を実行する。
    /// </summary>
    /// <param name="CreateNum">生成するユニット(1:Room, 2:XDirextWay, 3:ZDirectWay, 4:RoomSub)</param>
    /// <param name="CreateMob">生成する宝・敵</param>
    /// <param name="OriginNum">パースする前の文字列(Directの比較用)</param>
    /// <param name="MakePos">生成位置</param>
    /// <param name="Direct">方向制御用</param>
    private void MakeObject(int CreateNum, int CreateMob, string OriginNum, Vector3Int MakePos, string[] Direct)
    {
        int x = MakePos.x, y = MakePos.y, z = MakePos.z;
        switch (CreateNum)  //ユニット生成
        {
            case 0://null
                break;
            case 1://ルーム
                break;
            case 2://X軸トンネル

                if (Direct[2] == OriginNum)
                { break; }
                else if (Direct[3] == OriginNum)
                {
                    Instantiate(Tunnels[0], new Vector3(x, y, z), Quaternion.Euler(0, 90.0f, 0));
                    break;
                }
                else
                {
                    Instantiate(Tunnels[1], new Vector3(x, y, z), Quaternion.Euler(0, 90.0f, 0));
                    break;
                }

            case 3://Z軸トンネル
                if (Direct[0] == OriginNum)
                { break; }
                else if (Direct[2] == OriginNum)
                {
                    Instantiate(Tunnels[0], new Vector3(x, y, z), Quaternion.identity);
                    break;
                }
                else
                {
                    Instantiate(Tunnels[1], new Vector3(x, y, z), Quaternion.identity);
                    break;
                }

            case 4://ルームSub
                break;
        }

        //switch (CreateMob)   //オブジェ生成
        //{

        //}
    }
}
