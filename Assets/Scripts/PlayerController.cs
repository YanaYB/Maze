using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    // ������� Singleton
    public static PlayerController Instance;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private BoxCollider2D playerCollider;

    // ��������� ���������� ��� �������� ���������� ��������� ������
    private int boostCount = 0;
    [SerializeField] private TextMeshProUGUI boostCountText;  // UI TextMeshProUGUI ��� ����������� ��������

    private void Awake()
    {
        // ������� Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // ���� ��� ���������� ���������, ���������� ���� ������
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        // ����� ��������� ������ �� Canvas
        if (boostCountText == null)
        {
            boostCountText = GameObject.Find("BoostCountText").GetComponent<TextMeshProUGUI>();
            if (boostCountText == null)
            {
                Debug.LogError("BoostCountText �� ������. ���������, ��� ������ � ����� ������ ���������� �� Canvas.");
            }
        }

        UpdateBoostCountText();
    }

    private void Update()
    {

        // �������� ���� ��� ����������� ������
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        if (movement.x != 0.0f && movement.y != 0.0f)
        {
            movement = movement.normalized;  // ����������� ������ ��������, ����� �������� ���� ���������� �� ����� ����
        }
        //Debug.Log("x: " + movement.x+ " y: " + movement.y);
        if (movement.x == 0.0f && movement.y == 0.0f)
        {
            animator.SetBool("isIdle", true);
            animator.SetInteger("move", 0);
            return;
        }
        else
            animator.SetBool("isIdle", false);


        if(movement.x==1&& movement.y==0)
            animator.SetInteger("move", 3);
        if (movement.x == 0 && movement.y == 1)
            animator.SetInteger("move", 1);
        if (movement.x == -1 && movement.y == 0)
            animator.SetInteger("move", 4);
        if (movement.x == 0 && movement.y == -1)
            animator.SetInteger("move", 2);


        //if (movement.x >= movement.y)
        //{

        //    if (movement.x >= 0.0f)
        //        animator.SetInteger("move", 3);
        //    else
        //        animator.SetInteger("move", 4);

        //}
        //else
        //{
        //    if (movement.y >= 0.0f)
        //        animator.SetInteger("move", 1);
        //    else
        //        animator.SetInteger("move", 2);
        //}

    }

    private void FixedUpdate()
    {
        // ����������� ������ ����� Rigidbody2D
        rb.velocity = movement * moveSpeed;

        
        
    }

    public void AddBoost()
    {
        boostCount++;
        UpdateBoostCountText();  // ��������� UI � ����������� ������

        if (boostCount == 3)
        {
            GenerateMaze.Instance.OpenExit();  // ��������� �����
        }
    }

    private void UpdateBoostCountText()
    {
        Debug.Log("Boost count �����������: " + boostCount);
        if (boostCountText != null)
        {
            boostCountText.text = boostCount.ToString();  // ��������� �����
            Debug.Log("Boost count text ��������: " + boostCountText.text);
        }
        else
        {
            Debug.LogError("boostCountText �� ���������� � Inspector!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �������� ������������
        Debug.Log("OnTriggerEnter2D: ������������ � �������� " + other.gameObject.name);

        if (other.CompareTag("Exit"))
        {
            if (boostCount == 3)
            {
                Debug.Log("�� ������� �������� ��������!");
                SceneManager.LoadScene("Win");  // ������� �� ����� Win
            }
            else
            {
                Debug.Log("������� �������� ��� �����!");
            }
        }

        if (other.CompareTag("Boost"))
        {
            AddBoost();
            Debug.Log("���� ������");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // �������� ������������ � �������� �������
        Debug.Log("OnCollisionStay2D: ������������ � �������� " + collision.gameObject.name);

        // ���������, ���� ������ - �����
        if (collision.gameObject.CompareTag("Wall"))
        {
            // ���� ����� ���������� ������������ �� ������, �� ������������� ��� ��������
            Debug.Log("����� ���������� ������������ �� ������.");
        }
    }

}

