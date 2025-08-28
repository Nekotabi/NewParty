using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region PL宣言系
    private enum State
    {
        Idle,
        Walking,
        Running,
        Jumping,
    }
    private State state;

    private Rigidbody rb;
    private Transform MyTrans;
    private Vector3 velocity;
    public float Speed;
    private float MoveSpeed = 0.0f, RotateGoal = 0.0f, MyRot = 0.0f;
    private bool IsJump = false, IsDash = false;
    private bool[] Moving = new bool[4] { false, false, false, false };
    #endregion

    void Start()
    {
        MyTrans = this.transform;
        MyRot = this.transform.rotation.y;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;    //velocity初期化

        if (Input.GetKey(KeyCode.LeftShift))//ダッシュ判定
        {
            IsDash = true;
            MoveSpeed = Speed * 1.30f * Time.deltaTime;
        }
        else
        {
            IsDash = false;
            MoveSpeed = Speed * Time.deltaTime;
        }

        //前後左右
        if (Input.GetKey(KeyCode.W))//前
        {
            velocity.z += MoveSpeed;
            Moving[0] = true;
        }
        else
            Moving[0] = false;
        if (Input.GetKey(KeyCode.S))//後
        {
            velocity.z -= MoveSpeed;
            Moving[1] = true;
        }
        else
            Moving[1] = false;
        if (Input.GetKey(KeyCode.A))//左
        {
            velocity.x -= MoveSpeed;
            Moving[2] = true;
        }
        else
            Moving[2] = false;
        if (Input.GetKey(KeyCode.D))//右
        {
            velocity.x += MoveSpeed;
            Moving[3] = true;
        }
        else
            Moving[3] = false;

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsJump = true;
        }

        //移動処理
        Debug.Log(RotateCheck());
        MyTrans.position += velocity;

        //アニメーション処理
        AnimCheck();
    }

    private void FixedUpdate()
    {
        if (IsJump)
            gravity();
    }

    /// <summary>
    ///実行するアニメーションを判定 
    /// </summary>
    private void AnimCheck()
    {
        if (IsJump)
            state = State.Jumping;
        else if (IsDash)
            state = State.Running;
        else
            state = State.Walking;
        if (!Input.anyKey)
        {
            state = State.Idle;
        }
    }

    /// <summary>
    /// 向きを返す。
    /// </summary>
    /// <param name="Movable"></param>
    private int RotateCheck()
    {
        int RotateGoal = 0;
        if (Moving[2])
            RotateGoal -= 90;
        if (Moving[3])
            RotateGoal += 90;
        if (!Moving[0] || !Moving[1])
        {
            if (Moving[0])
                RotateGoal /= 2;
            if (Moving[1])
            {
                if (Moving[2] || Moving[3])
                    RotateGoal += RotateGoal / 2;
                else
                    RotateGoal += 180;
            }
        }
        return RotateGoal;
    }

    private void gravity()
    {

    }
}
