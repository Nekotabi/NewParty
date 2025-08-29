using System.Linq;
using Unity.VisualScripting;
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
    private float MoveSpeed = 0.0f, MyRot = 0.0f, Speed = 10.0f;
    private bool IsJump = false, IsDash = false;
    public bool MoveFreeze = false;
    public float JumpPower = 0.0f, weight;
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
            MoveSpeed = Speed * 1.50f * Time.deltaTime;
        }
        else
        {
            IsDash = false;
            MoveSpeed = Speed * Time.deltaTime;
        }

        //前後左右
        if (Input.GetKey(KeyCode.W))//前
        {
            velocity.x += MoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))//後
        {
            velocity.x -= MoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))//左
        {
            velocity.z += MoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))//右
        {
            velocity.z -= MoveSpeed;
        }
        //ジャンプ
        if (Input.GetKey(KeyCode.Space))
        {
            if (!IsJump)
            {
                IsJump = true;
                rb.AddForce(0, JumpPower, 0);
            }
        }

        //移動処理
        if(velocity != Vector3.zero)
        {
            MyTrans.position += transform.rotation * velocity;
        }

        //方向処理
        MyRot = Input.mousePosition.x;
        MyTrans.eulerAngles = new Vector3(0, MyRot, 0);

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

    private void gravity()
    {
        if(MyTrans.position.y < -50.0f)//保険
        {
            MyTrans.position = new Vector3(0, 20, 0);
        }

        if (IsJump)
        {
            rb.AddForce(0, -weight, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                IsJump = false;
                break;
        }
    }
}
