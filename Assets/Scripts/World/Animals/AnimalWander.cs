using UnityEngine;
using System.Collections;

public class AnimalWander : MonoBehaviour
{
    [Header("Configurações da Patrulha")]
    [Tooltip("Os pontos que o animal seguirá em ordem.")]
    public Transform[] patrolPoints;

    [Tooltip("A velocidade de movimento do animal.")]
    public float moveSpeed = 2f;

    [Tooltip("O tempo em segundos que o animal espera em cada ponto.")]
    public float waitTimeAtPoint = 3f;

  
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private int currentPointIndex = 0;

    void Awake()
    {
        
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || anim == null)
        {
            Debug.LogError("O animal " + gameObject.name + " precisa de um SpriteRenderer e um Animator!");
        }
    }

    void Start()
    {
        
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
           
            StartCoroutine(PatrolRoutine());
        }
        else
        {
            Debug.LogWarning("Nenhum ponto de patrulha configurado para " + gameObject.name + ". O animal ficará parado.");
            
            if (anim != null) anim.SetBool("isWalking", false);
        }
    }

    private IEnumerator PatrolRoutine()
    {
        
        while (true)
        {
            
            Transform targetPoint = patrolPoints[currentPointIndex];

            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                // Ativa a animação de andar
                if (anim != null) anim.SetBool("isWalking", true);

                
                Vector2 direction = (targetPoint.position - transform.position).normalized;
                if (direction.x > 0)
                {
                    spriteRenderer.flipX = true; 
                }
                else if (direction.x < 0)
                {
                    spriteRenderer.flipX = false; 
                }

                
                transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
                
                
                yield return null;
            }

            
            transform.position = targetPoint.position;
            

            if (anim != null) anim.SetBool("isWalking", false);

            yield return new WaitForSeconds(waitTimeAtPoint);

            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}