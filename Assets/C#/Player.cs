using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Assertions.Must;

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

    private enum MoveDir
    {
        Flont = 0,
        FlontRight = 45,
        Right = 90,
        BackRight = 135,
        Back = 180,
        BackLeft = 225,
        Left = 270,
        FlontLeft = 315
    }
    private MoveDir moveDir;

    private Rigidbody rb;
    private Transform MyTrans;
    private Vector3 velocity;
    public float Speed;
    private float MoveSpeed = 0.0f, RotateGoal = 0.0f;
    private bool IsJump = false, IsDash = false;
    private bool[] Moving = new bool[4] { false, false, false, false };//W, S, A, D

    #endregion

    void Start()
    {
        MyTrans = this.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;    //velocity初期化

        if (Input.GetKey(KeyCode.LeftShift))//ダッシュ判定
        {
            IsDash = true;
            MoveSpeed = Speed * 1.30f;
        }
        else
        {
            IsDash = false;
            MoveSpeed = Speed;
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
        RotateCheck();
        MyTrans.position += velocity;

        //アニメーション処理
        AnimCheck();
        Debug.Log(state);
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

    private void RotateCheck()
    {
        if (Moving[0])
        {
            if (Moving[2])
                moveDir = MoveDir.FlontLeft;
            else if (Moving[3])
                moveDir = MoveDir.FlontRight;
            else
                moveDir = MoveDir.Flont;
        }
        if (Moving[1])
        {
            if (Moving[2])
                moveDir = MoveDir.BackLeft;
            else if (Moving[3])
                moveDir = MoveDir.BackRight;
            else
                moveDir = MoveDir.Back;
        }
    }

    private void gravity()
    {

    }
}
