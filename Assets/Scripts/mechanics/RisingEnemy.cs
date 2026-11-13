using Unity.VisualScripting;
using UnityEngine;

public class RisingEnemy : MonoBehaviour
{
    /// <summary>
    /// Manager houses the player information
    /// </summary>
    [Header("Movement")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField, Min(0f)] private float riseSpeed = 2f;
    
    [Header("References")]
    private PlayerManager manager;
    private LevelManager levelManager;

    private Vector3 startingPosition;
    
    private Collider collider;
    public float EnemySurface => collider.bounds.max.y;

    void Awake()
    {
        manager = FindAnyObjectByType<PlayerManager>();
        levelManager = FindAnyObjectByType<LevelManager>();
        
        collider = GetComponent<Collider>();

        transform.position = startPos.position;
        startingPosition = startPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        float newY = Mathf.MoveTowards(
            transform.position.y,
            endPos.position.y,
            riseSpeed * Time.deltaTime
        );

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    public void ResetEnemy()
    {
        transform.position = startingPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            levelManager.CompleteLevel();
        }
    }

}