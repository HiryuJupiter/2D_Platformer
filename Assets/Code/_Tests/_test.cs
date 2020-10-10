using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class _test : MonoBehaviour
{
    public LayerMask groundLayer;

    [Range(-2f, 2f)]
    public float moveX = 1f;
    [Range(1f, 89f)]
    public float maxSlopeAngle = 45f;


    float slopeAngle;
    float descendableRange;
    RaycastHit2D hit;
    Vector3 BR_offset;
    Vector3 BL_offset;
    Vector3 TR_offset;
    Vector3 TL_offset;

    Vector3 BR => transform.position + BR_offset;
    Vector3 BL => transform.position + BL_offset;
    Vector3 TR => transform.position + TR_offset;
    Vector3 TL => transform.position + TL_offset;

    const float SkinWidth = 0.015f;


    void Start()
    {
        Bounds b = GetComponent<BoxCollider2D>().bounds;
        BR_offset = new Vector3(b.extents.x, -b.extents.y, 0f);
        BL_offset = new Vector3(-b.extents.x, -b.extents.y, 0f);
        TR_offset = new Vector3(b.extents.x, b.extents.y, 0f);
        TL_offset = new Vector3(-b.extents.x, b.extents.y, 0f);
    }

    void Update()
    {
        //If ascending slop, do not decend.
        DecendeSlope();
    }

    //void AscendSlope ()
    //{
        
    //    float facingSign = Mathf.Sign(moveX);

    //    float rayDistance = Mathf.Abs(moveX) + SkinWidth;
    //    Vector2 rayOrigin = facingSign < 0 ? BL: BR;
    //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, facingSign * Vector2.right, rayDistance, groundLayer);

    //    Debug.DrawRay(rayOrigin, facingSign * Vector2.right * rayDistance, Color.yellow);

    //    if (hit && hit.distance != 0f)
    //    {
    //        float slopAngle = Vector2.Angle(hit.normal, Vector2.up);

    //        if (slopeAngle < maxSlopeAngle)
    //        {

    //        }

    //    }
    //    else
    //    {
    //        DecendeSlope()
    //    }

    //}

    void DecendeSlope()
    {
        //Forward moving ray
        Debug.DrawRay(BL, Vector3.left * moveX, Color.yellow);

        //Use max raycast dist to check if we've hit a slope
        Debug.DrawRay(BL, Vector3.down * 100f, Color.green);

        hit = Physics2D.Raycast(BL, Vector3.down, 100f, groundLayer);
        if (hit)
        {
            slopeAngle = Vector2.Angle(Vector2.up, hit.normal);

            //If the slope is less than maxSlope angle
            if (slopeAngle != 0 && slopeAngle < maxSlopeAngle)
            {
                //Check if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
                if (Mathf.Sign(hit.normal.x) == Mathf.Sign(moveX))
                {
                    //Check if we are standing close enough to the platform to begin decend calculation. 
                    descendableRange = (Mathf.Abs(moveX) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad));
                    Debug.DrawRay(BL, Vector3.down * descendableRange, Color.blue);
                    if (hit.distance - SkinWidth < descendableRange)
                    {
                        //Specify the decend amount
                        float moveDist = Mathf.Abs(moveX);
                        Debug.DrawRay(hit.point, hit.normal, Color.red);
                        Vector2 move = Vector2.zero;
                        move.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(move.x);
                        move.y = -Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                        transform.Translate(move * Time.deltaTime);
                    }
                    else
                    {
                        Debug.DrawRay(hit.point, hit.normal, Color.yellow);
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 20), "Max slop raycast dist: " + Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad));
        GUI.Label(new Rect(20, 40, 500, 20), "moveX: " + moveX);

        if (hit)
        {
            GUI.Label(new Rect(20, 70, 500, 20), "slopeAngle: " + slopeAngle);
            GUI.Label(new Rect(20, 90, 500, 20), "descendable range: " + descendableRange);
            GUI.Label(new Rect(20, 110, 500, 20), "");
        }
    }
}


/*
 public class HealthBar : MonoBehaviour
{
    public Sprite[] images;
    public Image healthBar;

    public void SetHealth (int newHealth)
    {
        newHealth -= 1;
        if (newHealth < images.Length && newHealth >= 0)
        {
            healthBar.sprite = images[newHealth];
        }
        else
        {
            Debug.LogWarning("Index " + newHealth + " is out of bounds");
        }
    }
}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneManager.instance.CoinPickup();
            Destroy(gameObject);
        }
    }
 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneManager.instance.ReduceHealth();
            Destroy(gameObject);
        }
    }

    public void UpdateTimer()
    {
        timeElapsed += Time.deltaTime;
        string minutes = Mathf.Floor(timeElapsed / 60f).ToString("00");
        string seconds = Mathf.Floor(timeElapsed % 60).ToString("00");
        string miliseconds = Mathf.Floor((timeElapsed * 100) % 100).ToString("00");

        timer.text = minutes + ":" + seconds + ":" + miliseconds;
    }

 */