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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsJump = true;
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

    private void gravity()
    {

    }
}
