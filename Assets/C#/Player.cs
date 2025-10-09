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
        Landing,
        Wince,
        Damaged,
        Death,
    }
    private State state;

    private Rigidbody rb;
    private Transform MyTrans;
    private Vector3 velocity, FirstMousePos, Angle;
    private Animator anim;

    private float MyRot = 0.0f, JumpPower = 500.0f, weight = 20.0f, MoveSpeed = 3.0f, RotationSpeed = 360.0f;
    private float[] Speeds = new float[3] { 2.0f, 2.0f, 4.0f };   //MoveSpeed, MinSpeed, MaxSpeed
    private bool IsJump = false, IsDash = false;
    public bool MoveFreeze = false;
    #endregion

    void Start()
    {
        MyTrans = this.transform;
        MyRot = this.transform.rotation.y;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        FirstMousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;    //velocity初期化
        Angle = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Escape))   //ポーズ用(仮)
        {
            if (MoveFreeze)
                MoveFreeze = false;
            else
                MoveFreeze = true;
        }

        if (!MoveFreeze || state != State.Death)
        {
            if (IsDash)//ダッシュ判定
            {
                if (Speeds[0] < Speeds[2])
                    Speeds[0] += 0.5f * Time.deltaTime;
            }
            else
            {
                if (Speeds[0] > Speeds[1])
                    Speeds[0] -= 0.5f * Time.deltaTime;
            }

            float MoveSpeed = Speeds[0] / 100;

            //前後左右
            if (Input.GetKey(KeyCode.W))//前
            {
                velocity.z += MoveSpeed;
            }
            if (Input.GetKey(KeyCode.S))//後
            {
                velocity.z -= MoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))//左
            {
                velocity.x -= MoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))//右
            {
                velocity.x += MoveSpeed;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!IsDash)
                    IsDash = true;
                else
                    IsDash = false;
            }

            //ジャンプ
            if (Input.GetKey(KeyCode.Space))
            {
                if (!IsJump)
                {
                    IsJump = true;
                    rb.AddForce(0, JumpPower, 0);
                    state = State.Jumping;
                }
            }
        }

        //移動処理
        if (velocity != Vector3.zero)
        {
            Vector3 direct = velocity.normalized;
            float Ydiff = Mathf.DeltaAngle(0, Mathf.Atan2(direct.x, direct.z) * Mathf.Rad2Deg);//移動方向をY値の回転値で割り出す
            Quaternion NowRot = this.transform.rotation;
            Quaternion TargetRot = Quaternion.Euler(0, Ydiff, 0);
            MyTrans.rotation = Quaternion.RotateTowards(NowRot, TargetRot, RotationSpeed * Time.deltaTime);
            MyTrans.position += direct * MoveSpeed * Time.deltaTime;//position更新

            if (IsDash)
                state = State.Running;
            else
                state = State.Walking;
        }

        //アニメーション系
        if (!Input.anyKey)//待機
        {
            state = State.Idle;
        }

        if (state == State.Wince)//ひるみ
        {
            Wince();
        }
        AnimChanger(state);
    }

    private void FixedUpdate()
    {
        if (IsJump)
            gravity();
    }
    /// <summary>
    /// Stateでアニメーションを切り替える
    /// </summary>
    /// <param name="state">切り替えるもの</param>
    private void AnimChanger(State state)
    {
        anim.SetBool("Walking", state == State.Walking);
        anim.SetBool("Running", state == State.Running);
        anim.SetBool("Death", state == State.Death);
    }
    /// <summary>
    /// 重力処理
    /// </summary>
    private void gravity()
    {
        if (MyTrans.position.y < -50.0f)//保険
        {
            MyTrans.position = new Vector3(0, 20, 0);
        }

        if (IsJump)
        {
            rb.AddForce(0, -weight, 0);
        }
    }

    private void Wince()
    {
        velocity *= -1;
        MoveFreeze = true;  //動きを止める

        for (float i = 0.0f; i < 0.4f;)  //アニメーション終わるまで待つ
        {
            i += Time.deltaTime;
        }
        MoveFreeze = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                IsJump = false;
                state = State.Landing;
                break;
            case "NormalWall":
                state = State.Wince;
                break;
            case "Enemy":
                state = State.Death;
                break;
        }
    }
}
