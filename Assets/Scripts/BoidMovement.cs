using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    
    public GameObject[] boids;
    float xpos_avg, ypos_avg, xvel_avg, yvel_avg, neighboring_boids, close_dx, close_dy = 0;
    public float visualRange = 5.0f;
    public float criticalDistance = 2.0f;
    public float centeringFactor = 0.0005f;
    public float matchingFactor = 0.05f;
    public float avoidanceFactor = 0.05f;
    public float turnFactor = 0.2f;
    public float minSpeed = 2.0f;
    public float maxSpeed = 3.0f;

    public float bounds = 15.0f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        boids = GameObject.FindGameObjectsWithTag("Boid");
    }

    // Update is called once per frame
    void Update()
    {
        // Loop through all boids
        foreach (GameObject boid in boids)
        {

            rb = boid.GetComponent<Rigidbody2D>();
            xpos_avg = ypos_avg = xvel_avg = yvel_avg = neighboring_boids = close_dx = close_dy = 0;
            Vector2 boidVel=rb.velocity;
            // Loop through all other boids again
            foreach (GameObject neighbor in boids)
            {
                if(neighbor==boid) continue;
                Vector2 neighborVel=neighbor.GetComponent<Rigidbody2D>().velocity;
                //distance between boid and neighbor
                float dx = boid.transform.position.x - neighbor.transform.position.x;
                float dy = boid.transform.position.y - neighbor.transform.position.y;
                float objDx = boid.transform.position.x - 10;
                float objDy = boid.transform.position.y - 10;
                //if the neighbor is within the visual range
                if(Mathf.Abs(dx)<visualRange&&Mathf.Abs(dy)<visualRange)
                {
                    // get the distance - hypotenuse
                    float squareDist = dx * dx + dy * dy;
                    float obstacleSquareDist = objDx * objDx + objDy * objDy;
                    //if the neighbor is within the critical distance(avoidance range)
                    if(squareDist<criticalDistance){
                        close_dx += boid.transform.position.x - neighbor.transform.position.x;
                        close_dy += boid.transform.position.y - neighbor.transform.position.y;
                    }
                    else if(obstacleSquareDist<criticalDistance+2){
                        close_dx += boid.transform.position.x - 10;
                        close_dy += boid.transform.position.y - 10;
                    }
                    //if the neighbor is within the visual range
                    else if(squareDist<visualRange){
                        xpos_avg += neighbor.transform.position.x;
                        ypos_avg += neighbor.transform.position.y;
                        xvel_avg += neighborVel.x;
                        yvel_avg += neighborVel.y;
                        neighboring_boids++;
                    }
                }
            }
     
            //if there are neighbors within the visual range
            if(neighboring_boids>0){
                // normalize velocity/position based on the number of neighbors
                xpos_avg /= neighboring_boids;
                ypos_avg /= neighboring_boids;
                xvel_avg /= neighboring_boids;
                yvel_avg /= neighboring_boids;
                
                boidVel.x = rb.velocity.x + (xpos_avg - boid.transform.position.x) * centeringFactor + (xvel_avg - rb.velocity.x) * matchingFactor;
                boidVel.y = rb.velocity.y + (ypos_avg - boid.transform.position.y) * centeringFactor + (yvel_avg - rb.velocity.y) * matchingFactor;
                rb.velocity = boidVel;
                boidVel = rb.velocity;
            }
            boidVel.x  = rb.velocity.x + close_dx * avoidanceFactor;
            boidVel.y = rb.velocity.y + close_dy * avoidanceFactor;
            rb.velocity = boidVel;
            boidVel = rb.velocity;
            if(boid.transform.position.x>bounds){
                boidVel.x = rb.velocity.x - turnFactor;
            }
            else if(boid.transform.position.x<-bounds){
                boidVel.x = rb.velocity.x + turnFactor;
            }
            else if(boid.transform.position.y>bounds){
                boidVel.y = rb.velocity.y - turnFactor;
            }
            else if(boid.transform.position.y<-bounds){
                boidVel.y = rb.velocity.y + turnFactor;
            }
            rb.velocity = boidVel;
            boidVel = rb.velocity;

            float speed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y);

            if(speed<minSpeed){
                boidVel.x = rb.velocity.x / speed * minSpeed;
                boidVel.y = rb.velocity.y / speed * minSpeed;
            }else if(speed>maxSpeed){
                boidVel.x = rb.velocity.x / speed * maxSpeed;
                boidVel.y = rb.velocity.y / speed * maxSpeed;
            }
            rb.velocity = boidVel;
            boidVel = rb.velocity;
            /*
            uncomment for cursor-based boid movement
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float xDist = boid.transform.position.x - worldPosition.x;
            float yDist = boid.transform.position.y - worldPosition.y;
            Vector2 pos = new Vector2(boid.transform.position.x-(xDist*0.05f)+rb.velocity.x, boid.transform.position.y-(yDist*0.05f)+rb.velocity.y);
            */
            Vector2 pos = new(boid.transform.position.x+rb.velocity.x, boid.transform.position.y+rb.velocity.y);
            boid.transform.position = pos;
        }
    }
}
