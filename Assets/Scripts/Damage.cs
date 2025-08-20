using UnityEngine;

public class Damage : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    private float _maxHealth;

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    private float _health = 100;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;

            //If health drops below 0, character dies
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    private bool _isAlive = true;

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.IsAlive, value);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
