using UnityEngine;

public class playerMovement : MonoBehaviour
{
   [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    

    private void Awake()
    {
        //grab references
        //to access rididbody and store it inside body
        //to access animator and store it inside body
        //to access boxCollider2D and store it inside body
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    //when player moves

    private void Update()
    {
        //body velocity how fast player moves
        horizontalInput = Input.GetAxis("Horizontal");

        //flip player when moving left-right
        //player facing right when moving right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        // when moving left 
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);


        //set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //wall jump logic
        if (wallJumpCooldown > 0.2f)
        {

            //body velocity how fast player moves
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                //player gets stuck and wont fall down
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 7;

            //checking if space key is pressed to jump
            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
   
    // jump trigger
    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        //when player is attached to wall
        else if (onWall() && !isGrounded())
        {
            if(horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) *3,6);

            wallJumpCooldown = 0;
        }
    }
 

    private bool isGrounded()
    { 
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
