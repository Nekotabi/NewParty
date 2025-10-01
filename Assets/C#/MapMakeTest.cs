using System.Collections;
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
    [Tooltip("その他オブジェクト")]public List<GameObject> Mobs;

    public bool SuccessMakeMap;
    #endregion
    #region Private宣言
    private List<TextAsset> CSVList = new();//CSVの保管用
    private List<List<List<string>>> FieldFile = new(); //生成パターン保存用
    private List<List<List<int>>> MakeIt = new();   //生成補助パターン保存用
    private int FloorNum = 0;//地面構造を変える用(2パターン)
    private Vector3 Cor = new Vector3(4.2f, 4.4f, 4.2f); //生成の補正
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
        StartCoroutine(Create());
    }

    IEnumerator Create()
    {
        //ステージ製作にあたって、処理軽減目的で階層が変わった時か指定個数を作り終わった時に1フレーム待たせます。
        for (int y = 0; y < FieldFile.Count; y++)
        {
            FloorNum = y % 2;
            for (int x = 0; x < FieldFile[y].Count; x++)
            {
                for (int z = 0; z < FieldFile[y][x].Count; z++)
                {
                    BuildMap(FieldFile[y][x][z], new Vector3Int(x, y, z));
                    yield return null;
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
                if (i != 0)
                {
                    LogNums[i][j] += 1;
                    LogNums[i - 1][j] += 4;
                }
                if (j != 0)
                {
                    LogNums[i][j] += 2;
                    LogNums[i][j - 1] += 8;
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
    private void BuildMap(string OriginNum, Vector3Int FilePass)
    {
        string[] StrNums = OriginNum.Split("_");//"_"で主番号と付番号を分解
        int LogNum = MakeIt[FilePass.y][FilePass.x][FilePass.z];//生成中のパスを格納
        string[] Direct = DirectDistinction(LogNum, FilePass);//↑の周囲のオブジェクトを検索

        int MobNum = 0;//オブジェの数値を保管する。0は何もなし。
        if (StrNums.Length == 2)
            MobNum = int.Parse(StrNums[1]);

        Vector3 CreatePos = new Vector3(FilePass.z * Cor.z, FilePass.y * Cor.y, FilePass.x * -Cor.x);
        Transform FloorTr = null;

        //生成実行
        int CreateNum = int.Parse(StrNums[0]);
        string Dir = "";
        GameObject FloorObj = null;
        switch (CreateNum)  //ユニット生成
        {
            case 0://null
                break;
            case 1://ルーム
                FloorTr = Instantiate(Grounds[FloorNum], CreatePos, Quaternion.identity).transform;//床生成
                for (int i = 0; i < Direct.Length; i++)
                {
                    Dir = Direct[i].Substring(0, 1);//nullはnに、それ以外は主数字になる。

                    if (Direct[i] != OriginNum)//同じフロアが面する部分は壁生成から弾く。
                    {
                        if (Dir == "n" || Dir == "0")//nullか0なら並壁
                        {
                            CreateWall(FloorTr, CreatePos, i, true);
                        }
                        else if ((i < 2 && Dir == "3") || (i >= 2 && Dir == "4"))//上下に横軸が付くか、左右に縦軸が付いたら並壁
                        {
                            CreateWall(FloorTr, CreatePos, i, false);
                        }
                        else//それ以外は穴あき壁
                        {
                            CreateWall(FloorTr, CreatePos, i, false);
                        }
                    }
                }
                FloorObj = Instantiate(Walls[0], new Vector3(CreatePos.x, CreatePos.y += Cor.y - 0.2f, CreatePos.z + (Cor.z / 2)), Quaternion.Euler(new Vector3(-90, 0, 0)));
                FloorObj.transform.SetParent(FloorTr);
                break;

            case 2://ルームSub
                FloorTr = Instantiate(Grounds[FloorNum], CreatePos, Quaternion.identity).transform;
                for (int i = 0; i < Direct.Length; i++)
                {
                    Dir = Direct[i].Substring(0, 1);//nullはnに、それ以外は主数字になる。

                    if (Direct[i] != OriginNum)
                    {
                        if (Dir == "n" || Dir == "0")
                        {
                            CreateWall(FloorTr, CreatePos, i, false);
                        }
                        else if ((i < 2 && Dir == "3") || (i >= 2 && Dir == "4"))
                        {
                            CreateWall(FloorTr, CreatePos, i, false);
                        }
                        else
                        {
                            CreateWall(FloorTr, CreatePos, i, true);
                        }
                    }
                }
                FloorObj = Instantiate(Walls[0], new Vector3(CreatePos.x, CreatePos.y += Cor.y - 0.2f, CreatePos.z + (Cor.z / 2)), Quaternion.Euler(new Vector3(-90, 0, 0)));
                FloorObj.transform.SetParent(FloorTr);
                break;

            case 3://X軸トンネル
                CreatePos.y += 1.0f;
                FloorTr = Instantiate(Tunnels[1], CreatePos, Quaternion.Euler(0.0f, 90.0f, 0.0f)).transform;
                FloorTr.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;

            case 4://Z軸トンネル
                CreatePos.y += 1.0f;
                FloorTr = Instantiate(Tunnels[1], CreatePos, Quaternion.identity).transform;
                FloorTr.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }

        switch (MobNum)//生成物調整
        {
            case 0://null
                break;
            case 1://アイテムポイント
                Instantiate(Mobs[0], CreatePos, Quaternion.identity).transform.SetParent(FloorTr);
                break;
            case 2://敵のスポーン位置
                Instantiate(Mobs[0], CreatePos, Quaternion.identity).transform.SetParent(FloorTr);
                break;
            case 3://Playerの初期位置

                break;
            case 4://出口
                Instantiate(Mobs[0], CreatePos, Quaternion.identity).transform.SetParent(FloorTr);
                break;
        }
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

    private void CreateWall(Transform FloorTr, Vector3 _CreatePos, int CreateDirect, bool isNull)
    {
        GameObject MakeWall;
        Vector3 WallDirection = Vector3.zero;
        if (isNull)
            MakeWall = Walls[1];
        else
            MakeWall = Walls[2];

        switch (CreateDirect)
        {
            case 0:
                _CreatePos.z += Cor.z / 2;
                //向きは変えない。
                break;
            case 1:
                _CreatePos.z -= Cor.z / 2;
                WallDirection.y = 180.0f;
                break;
            case 2:
                _CreatePos.x -= Cor.x / 2;
                WallDirection.y = 270.0f;
                break;
            case 3:
                _CreatePos.x += Cor.x / 2;
                WallDirection.y = 90.0f;
                break;
        }

        GameObject WallObj = Instantiate(MakeWall, _CreatePos, Quaternion.Euler(WallDirection));
        WallObj.transform.SetParent(FloorTr);
    }

    /// <summary>
    /// トンネルが曲がるかどうかを判別する。0はfalse, 1は上か左へ、2は下か右へ。
    /// </summary>
    /// <param name="Directs">InDirect</param>
    /// <param name="IsX">判定はX軸か？(falseはZ軸になる)</param>
    /// <returns></returns>
    /*private int CheckCorner(string[] Directs, bool IsX)
    {
        int TurnNum = 0;//特に無ければ0が帰る。

        for(int i = 0; i < Directs.Length; i++)
        {
            int CheckNum = int.Parse(Directs[i].Substring(0,1));
            if (IsX && i < 2)//X軸かつ上下判定中か？
            {
                if(CheckNum == 4)//Z軸トンネルが側面に当たっているなら曲げる判定を返す。
                    TurnNum = i;
            }
            else if(i >= 2)//Z軸確定かつ左右判定中か？
            {
                if(CheckNum == 3)//X軸トンネルが側面に当たっているなら曲げる判定を返す。
                    TurnNum = i-2;
            }
        }

        return TurnNum;
    }
    */
}
