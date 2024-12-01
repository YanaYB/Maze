using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    // Паттерн Singleton
    public static PlayerController Instance;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private BoxCollider2D playerCollider;

    // Добавляем переменную для хранения количества собранных бустов
    private int boostCount = 0;
    [SerializeField] private TextMeshProUGUI boostCountText;  // UI TextMeshProUGUI для отображения счетчика

    private void Awake()
    {
        // Паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Если уже существует экземпляр, уничтожаем этот объект
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        // Найти текстовый объект на Canvas
        if (boostCountText == null)
        {
            boostCountText = GameObject.Find("BoostCountText").GetComponent<TextMeshProUGUI>();
            if (boostCountText == null)
            {
                Debug.LogError("BoostCountText не найден. Убедитесь, что объект с таким именем существует на Canvas.");
            }
        }

        UpdateBoostCountText();
    }

    private void Update()
    {

        // Получаем ввод для перемещения игрока
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        if (movement.x != 0.0f && movement.y != 0.0f)
        {
            movement = movement.normalized;  // Нормализуем вектор движения, чтобы скорость была одинаковой по обеим осям
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
        // Перемещение игрока через Rigidbody2D
        rb.velocity = movement * moveSpeed;

        
        
    }

    public void AddBoost()
    {
        boostCount++;
        UpdateBoostCountText();  // Обновляем UI с количеством бустов

        if (boostCount == 3)
        {
            GenerateMaze.Instance.OpenExit();  // Открываем выход
        }
    }

    private void UpdateBoostCountText()
    {
        Debug.Log("Boost count обновляется: " + boostCount);
        if (boostCountText != null)
        {
            boostCountText.text = boostCount.ToString();  // Обновляем текст
            Debug.Log("Boost count text обновлен: " + boostCountText.text);
        }
        else
        {
            Debug.LogError("boostCountText не установлен в Inspector!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Логируем столкновения
        Debug.Log("OnTriggerEnter2D: столкновение с объектом " + other.gameObject.name);

        if (other.CompareTag("Exit"))
        {
            if (boostCount == 3)
            {
                Debug.Log("Вы успешно покинули лабиринт!");
                SceneManager.LoadScene("Win");  // Переход на сцену Win
            }
            else
            {
                Debug.Log("Сначала соберите все бусты!");
            }
        }

        if (other.CompareTag("Boost"))
        {
            AddBoost();
            Debug.Log("Буст собран");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Логируем столкновения в реальном времени
        Debug.Log("OnCollisionStay2D: Столкновение с объектом " + collision.gameObject.name);

        // Проверяем, если объект - стена
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Если игрок продолжает сталкиваться со стеной, мы предотвращаем его движение
            Debug.Log("Игрок продолжает столкновение со стеной.");
        }
    }

}

