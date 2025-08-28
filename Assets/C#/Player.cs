using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Player : MonoBehaviour
{
    #region PL�錾�n
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
        velocity = Vector3.zero;    //velocity������

        if (Input.GetKey(KeyCode.LeftShift))//�_�b�V������
        {
            IsDash = true;
            MoveSpeed = Speed * 1.30f;
        }
        else
        {
            IsDash = false;
            MoveSpeed = Speed;
        }

        //�O�㍶�E
        if (Input.GetKey(KeyCode.W))//�O
        {
            velocity.z += MoveSpeed;
            Moving[0] = true;
        }
        else
            Moving[0] = false;
        if (Input.GetKey(KeyCode.S))//��
        {
            velocity.z -= MoveSpeed;
            Moving[1] = true;
        }
        else
            Moving[1] = false;
        if (Input.GetKey(KeyCode.A))//��
        {
            velocity.x -= MoveSpeed;
            Moving[2] = true;
        }
        else
            Moving[2] = false;
        if (Input.GetKey(KeyCode.D))//�E
        {
            velocity.x += MoveSpeed;
            Moving[3] = true;
        }
        else
            Moving[3] = false;

        //�W�����v
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsJump = true;
        }

        //�ړ�����
        RotateCheck();
        MyTrans.position += velocity;

        //�A�j���[�V��������
        AnimCheck();
        Debug.Log(state);
    }

    private void FixedUpdate()
    {
        if (IsJump)
            gravity();
    }

    /// <summary>
    ///���s����A�j���[�V�����𔻒� 
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
