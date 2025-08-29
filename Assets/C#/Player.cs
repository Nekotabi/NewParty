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
        velocity = Vector3.zero;    //velocity������

        if (Input.GetKey(KeyCode.LeftShift))//�_�b�V������
        {
            IsDash = true;
            MoveSpeed = Speed * 1.50f * Time.deltaTime;
        }
        else
        {
            IsDash = false;
            MoveSpeed = Speed * Time.deltaTime;
        }

        //�O�㍶�E
        if (Input.GetKey(KeyCode.W))//�O
        {
            velocity.x += MoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))//��
        {
            velocity.x -= MoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))//��
        {
            velocity.z += MoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))//�E
        {
            velocity.z -= MoveSpeed;
        }
        //�W�����v
        if (Input.GetKey(KeyCode.Space))
        {
            if (!IsJump)
            {
                IsJump = true;
                rb.AddForce(0, JumpPower, 0);
            }
        }

        //�ړ�����
        if(velocity != Vector3.zero)
        {
            MyTrans.position += transform.rotation * velocity;
        }

        //��������
        MyRot = Input.mousePosition.x;
        MyTrans.eulerAngles = new Vector3(0, MyRot, 0);

        //�A�j���[�V��������
        AnimCheck();
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
