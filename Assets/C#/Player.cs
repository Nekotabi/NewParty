using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region PL�錾�n
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

    private float MyRot = 0.0f, JumpPower = 500.0f, weight = 20.0f;
    private float[] Speeds = new float[3] {2.0f, 2.0f, 4.0f};   //MoveSpeed, MinSpeed, MaxSpeed
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
        velocity = Vector3.zero;    //velocity������
        Angle = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Escape))   //�|�[�Y�p(��)
        {
            if (MoveFreeze)
                MoveFreeze = false;
            else
                MoveFreeze = true;
        }

        if (!MoveFreeze)
        {
            float RotateSpeed;
            if (Input.GetKey(KeyCode.LeftShift))//�_�b�V������
            {
                IsDash = true;
                RotateSpeed = 10.0f * Time.deltaTime;
                if (Speeds[0] < Speeds[2])
                    Speeds[0] += 0.5f * Time.deltaTime;
            }
            else
            {
                IsDash = false;
                RotateSpeed = 5.0f * Time.deltaTime;
                if (Speeds[0] > Speeds[1])
                    Speeds[0] -= 0.5f * Time.deltaTime;
            }

            float MoveSpeed = Speeds[0] / 100;

            //�O�㍶�E
            if (Input.GetKey(KeyCode.W))//�O
            {
                velocity.z += MoveSpeed;
            }
            if (Input.GetKey(KeyCode.S))//��
            {
                velocity.z -= MoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))//��
            {
                Angle.y -= RotateSpeed;
            }
            if (Input.GetKey(KeyCode.D))//�E
            {
                Angle.y += RotateSpeed;
            }

            //�W�����v
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

        //�ړ�����
            MyTrans.position += transform.rotation * velocity;
            if (IsDash)
                state = State.Running;
            else
                state = State.Walking;

        //��������
        MyTrans.eulerAngles += Angle; 

        //�A�j���[�V�����n
        if (!Input.anyKey)//�ҋ@
        {
            state = State.Idle;
        }

        if (state == State.Wince)//�Ђ��
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
    /// State�ŃA�j���[�V������؂�ւ���
    /// </summary>
    /// <param name="state">�؂�ւ������</param>
    private void AnimChanger(State state)
    {
        anim.SetBool("Walking", state == State.Walking);
        anim.SetBool("Running", state == State.Running);
    }
    /// <summary>
    /// �d�͏���
    /// </summary>
    private void gravity()
    {
        if(MyTrans.position.y < -50.0f)//�ی�
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
        MoveFreeze = true;  //�������~�߂�

        for(float i = 0.0f; i < 0.4f;)  //�A�j���[�V�����I���܂ő҂�
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
        }
    }
}
