using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapMakeTest : MonoBehaviour
{
    public int StageNum;//マップの高さ
    public List<GameObject> Grounds = new();//オブジェクト保管
    private List<TextAsset> CSVList = new();//CSVの保管用
    private List<List<List<string>>> FieldFile = new();//生成パターン保存用

    void Start()
    {
        if (StageNum != 0)
        {
            for (int i = 0; i < StageNum; i++)
            {
                TextAsset Map = Resources.Load<TextAsset>("Map" + i);
                if (Map != null)
                    FieldFile.Add(CSVRead(Map));
                else
                    Debug.LogError("Map" + i + "が見つかりませんでした");
            }
            if(FieldFile.Count != StageNum)
                Debug.Log("読み忘れがある可能性があります");
        }
        else
            Debug.Log("ステージの数を設定してください");
    }

    /// <summary>
    /// CSVファイルを2次元リストに変換する。
    /// </summary>
    /// <param name="file">InputFile</param>
    /// <returns>List(TwoDimenshonal)</returns>
    private List<List<string>> CSVRead(TextAsset file)
    {
        List<List<string>> MapDatas = new();//FieldFileに渡す用
        var StringReader = new StringReader(file.text); //TextAssetをTextに変換
        string line = StringReader.ReadLine();  //1行読む

        while (line != null) //lineをコンマ分けにしてMapDataに入れる
        {
            string[] column = line.Split(",");
            List<string> KariData = new List<string>(column);
            MapDatas.Add(KariData);

            line = StringReader.ReadLine();
        }
        
        return MapDatas;
    }
}
