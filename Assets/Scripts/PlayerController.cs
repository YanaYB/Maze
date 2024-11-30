using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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

        if (movement.x != 0 && movement.y != 0)
        {
            movement = movement.normalized;  // ����������� ������ ��������, ����� �������� ���� ���������� �� ����� ����
        }
    }

    private void FixedUpdate()
    {
        // ����������� ������ ����� Rigidbody2D
        rb.velocity = movement * moveSpeed;

        // ����������� ��������� ������ � ��� ��������
        Debug.Log("Player Velocity: " + rb.velocity);
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

