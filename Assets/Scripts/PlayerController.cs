using System;
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
    private TextMeshProUGUI boostCountText;  // UI TextMeshProUGUI ��� ����������� ��������

    [SerializeField] private GameObject olivka;
    [SerializeField] private float throwForce;
    [SerializeField] private int olivkaCount = 30;
    [SerializeField] private Transform[] shotPositions;
    private int currentShotPosition;
    private TextMeshProUGUI olivkaCountText;  // UI TextMeshProUGUI ��� ����������� ��������
    private Vector2 lookingAt;

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

        lookingAt = new Vector2(0, -1);

        // ����� ��������� ������ �� Canvas
        if (boostCountText == null)
        {
            boostCountText = GameObject.Find("BoostCountText").GetComponent<TextMeshProUGUI>();
            if (boostCountText == null)
            {
                Debug.LogError("BoostCountText �� ������. ���������, ��� ������ � ����� ������ ���������� �� Canvas.");
            }
        }
        if (olivkaCountText == null)
        {
            olivkaCountText = GameObject.Find("OlivkaCountText").GetComponent<TextMeshProUGUI>();
            if (olivkaCountText == null)
            {
                Debug.LogError("OlivkaCountText �� ������. ���������, ��� ������ � ����� ������ ���������� �� Canvas.");
            }
        }

        UpdateBoostCountText();
        UpdateOlivkaCount();
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
        
        // shooting
        if (Input.GetMouseButtonDown(0))
        {
            ThrowOlivka();
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
        {
            animator.SetInteger("move", 3);
            currentShotPosition = 2;
        }
        if (movement.x == 0 && movement.y == 1)
        {
            animator.SetInteger("move", 1);
            currentShotPosition = 0;
        }
        if (movement.x == -1 && movement.y == 0)
        {
            animator.SetInteger("move", 4);
            currentShotPosition = 1;
        }
        if (movement.x == 0 && movement.y == -1)
        {
            animator.SetInteger("move", 2);
            currentShotPosition = 0;
        }

        lookingAt = movement;
        

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

    private void UpdateOlivkaCount()
    {
        olivkaCountText.text = olivkaCount.ToString();
    }

    private void ThrowOlivka()
    {
        if(olivkaCount <= 0)
            return;
        
        var newOlivka = Instantiate(olivka, shotPositions[currentShotPosition].position, Quaternion.identity);
        var olivkaRb = newOlivka.GetComponent<Rigidbody2D>();
        olivkaRb.AddForce(lookingAt * throwForce);

        olivkaCount--;
        UpdateOlivkaCount();
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
                SceneManager.LoadSceneAsync("Win");  // ������� �� ����� Win
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        // losing
        if(other.gameObject.CompareTag("Buter"))
        {
            Debug.Log("Touching buter");
            SceneManager.LoadSceneAsync("Lose");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �������� ������������ � �������� �������
        //Debug.Log("OnCollisionStay2D: ������������ � �������� " + collision.gameObject.name);

        // ���������, ���� ������ - �����
        if (collision.gameObject.CompareTag("Wall"))
        {
            // ���� ����� ���������� ������������ �� ������, �� ������������� ��� ��������
            //Debug.Log("����� ���������� ������������ �� ������.");
        }
    }

}

