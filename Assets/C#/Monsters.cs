using UnityEngine;

public class Monsters : MonoBehaviour
{
    //基本情報
    public string Name;//名前
    public int EnemyHP;//HP
    public int DropCoin;//落とすコインの量
    public Vector3 MyStartPos;//自分の初期位置
    public Vector3 MyNowPos;//Updateで貰ってくる。
    public float FindLength;//発見する範囲

    private GameObject[] CoinObj;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public Transform PlayerTransTurn()
    {
        Transform Obj = GameObject.Find("Player").transform;
        return Obj;
    }
    public virtual void EnemyDeath(int CoinCount)
    {
        int[] basic = new int[3] { 10, 5, 1 };
        for (int i = 0; i < basic.Length; i++)
        {
            while (CoinCount >= basic[i])
            {
                Vector3 InstantPos = MyNowPos + (Random.insideUnitSphere * 0.1f);
                Instantiate(CoinObj[i], this.transform.position, Quaternion.identity);
                CoinCount -= basic[i];
            }
        }
        //死亡アニメーション
        if(true)//死亡アニメーションが終わったら
            Destroy(this.gameObject);
    }

    public void EnemyDamaged(int DamagePoint)
    {
        EnemyHP -= DamagePoint;
    }
}
