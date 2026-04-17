using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class OctopusController : MonoBehaviour
{
    [SerializeField] private State currentState = State.Idle;

    public float fullness; //How full the Octopus is out of 100
    public float startingFullness; //How full the Octopus starts at out of 100
    public float hungerPS; //How much fullness you lose per second
    public bool becameHungry = false; //Determines whether is Hungry activates at 50
    public Image eaten;

    [Header("Idle Rotation Vars")]
    public float maxRotationSpeed; //Max speed of octopus flailing
    public float rotationSpeed; //Current speed and direction of flailing
    public float idleSwapTimer; //How long since last rotation swap
    public float idleSwapTimerMax; //Time upon which octopus switches rotation speed/ direction
    public float activeIdleTime; //How long the Octopus has been idling for
    public float maxIdleTime; //This is where the Octopus switches to hunting

    [Header("Hunting Vars")]
    public List<Transform> destinations = new List<Transform>(); //Points in path to follow along
    public int currentDestination; //Which # point the Octopus is currently moving towards
    public float distanceToDestination; //How far the Octopus is from the current destination
    public float switchDistance; //How close the Octopus needs to be to the destination to go to the next
    public float activeHuntTime; //How long the Octopus has been hunting for
    public float maxHuntTime; //This is where the Octopus switches to camoflague stalking

    [Header("Chase Vars")]
    public GameObject target; //Target creature currently chasing
    public float distanceToTarget; //How far the Octopus is from the current target
    public float eatDistance; //At this distance the Octopus eats you
    public float moveSpeed; //How fast Octpus Moves while chasing regularily
    public float activeChaseTime; //How long the Octopus has been chasing for
    public float maxChaseTime; //After this the Octopus will give up and go idle
    public float justChasedTimerMax; //When the Octopus is permitted to chase again
    public float justChasedTimer; //How long since the Octopus chased the player
    public bool justChased = false; //Whether the Octopus has just chased the player

    [Header("Disguise Vars")]
    public SpriteRenderer octoSprite; //Sprite for the Octopus for disguise alpha changing
    public float activeDisguiseTime; //How long the Octopus has been disguised for
    public float maxDisguiseTime; //After this the Octopus will return to hunting

    [Header("Disguise Chase Vars")]
    public float disguiseMoveSpeed; //How fast Octopus moves while disguised
    public float activeDChaseTime; //How long the Octopus has been chasing for
    public float maxDChaseTime; //After this the Octopus will give up and go back to hunting



    public enum State 
    {
        Disguise,
        DisguiseChase,
        Idle,
        Hunt,
        Chase
    }

    private void Start()
    {
        fullness = startingFullness; //Set fullnes
        currentState = State.Idle; //Set default state, maybe not necessary :P
        rotationSpeed = maxRotationSpeed; //Set default rotation speed, maybe not necessary :P
        eaten = GameMan.eaten;
    }

    // Update is called once per frame
    void Update()
    {

        if (idleSwapTimer >= idleSwapTimerMax) //If timer elapses
        {
            idleSwapTimer = 0; //Reset Timer
            if (rotationSpeed >= 0) 
            {
                rotationSpeed = Random.Range(-15, -maxRotationSpeed); //Swaps rotation to negative if possitive
            }
            else
            {
                rotationSpeed = Random.Range(15, maxRotationSpeed); //Swaps rotation to positive if negative
            }

        }

        if (currentState == State.Idle || currentState == State.Disguise) idleSwapTimer += Time.deltaTime; //Timer for rotation switching
        fullness -= hungerPS * Time.deltaTime; //Hunger
        if (justChased) justChasedTimer += Time.deltaTime;
        if (justChasedTimer > justChasedTimerMax) justChased = false;


        if (fullness <= 60 && currentState == State.Idle && activeIdleTime > maxIdleTime)
        {
            currentState = State.Hunt; //If the Octopus is hungry GO HUNT!!
            activeIdleTime = 0;
        }
        if (activeHuntTime > maxHuntTime)
        {
            currentState = State.Disguise; //After the hunt is unsuccessful the mighty Octopus goes into hiding
            activeHuntTime = 0;
        }
        if (activeChaseTime > maxChaseTime)
        {
            justChased = true;
            currentState = State.Disguise; //If chase is unsuccessful go into camoflauge to trick prey
            activeChaseTime = 0;
        }
        if (activeDisguiseTime > maxDisguiseTime)
        {
            ActivateCamoflauge(false);
            currentState = State.Idle; //If disguise is unsuccessful return to idle -> hunt
            activeDisguiseTime = 0;
        }
        if (activeDChaseTime > maxDChaseTime)
        {
            ActivateCamoflauge(false);
            currentState = State.Disguise; //If chase is unsuccsesful return to disguise
            activeDChaseTime = 0;
        }

        switch (currentState)
        {
            case State.Disguise:
                Disguise();
                break;

            case State.DisguiseChase:
                DisguiseChase();
                break;

            case State.Idle:
                Idle();
                break;

            case State.Hunt:
                Hunt();
                break;

            case State.Chase:
                Chase();
                break;

        }

        void Disguise()
        {
            activeDisguiseTime += Time.deltaTime;
            ActivateCamoflauge(true);

            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime); //Wiggles while idle
        }

        void DisguiseChase()
        {
            activeDChaseTime += Time.deltaTime;
            ActivateCamoflauge(true);

            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, disguiseMoveSpeed * Time.deltaTime); //Chases target

            TurnTowardsTarget(target.transform);

            distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget < eatDistance)
            {
                Destroy(target.gameObject);
                eaten.gameObject.SetActive(true);
            }
        }

        void Idle()
        {
            activeIdleTime += Time.deltaTime;
            transform.Rotate(0f, 0f,rotationSpeed * Time.deltaTime); //Wiggles while idle
        }

        void Hunt()
        {
            activeHuntTime += Time.deltaTime;

            TurnTowardsTarget(destinations[currentDestination]);

            transform.position = Vector2.MoveTowards
                (transform.position, destinations[currentDestination].position, moveSpeed * Time.deltaTime); //Move Towards Next Point

            distanceToDestination = Vector2.Distance(transform.position, destinations[currentDestination].position);
            if (distanceToDestination < switchDistance)
            {
                currentDestination++;

                if (currentDestination >= destinations.Count)
                {
                    currentDestination = 0;
                }
            }

        }

        void Chase()
        {
            activeChaseTime += Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime); //Chases target

            TurnTowardsTarget(target.transform);

            distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget < eatDistance)
            {
                Destroy(target.gameObject);
                eaten.gameObject.SetActive(true);
            }
        }
    }

    void TurnTowardsTarget(Transform target)
    {
        Vector2 direction = target.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void ActivateCamoflauge(bool activate)
    {
        if (activate)
        {
            octoSprite.color = new Color(1f, 1f, 1f, 0.1f); //10% Opacity
        }
        else
        {
            octoSprite.color = new Color(1f, 1f, 1f, 1f); //100% Opacity
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) //Vision capsule for Octopus
    {
        if (collision.CompareTag("Player") && currentState!= State.Chase && currentState!= State.DisguiseChase && !justChased)
        {
            if (currentState == State.Disguise)
            {
                currentState = State.DisguiseChase;

            }
            else
            {
                currentState = State.Chase;
            }
            target = collision.gameObject;
            activeHuntTime = 0; //Reset state timers upon switching state
            activeChaseTime = 0;
            activeDChaseTime = 0;
            activeIdleTime = 0;
            activeDisguiseTime = 0;
        }
    }

    

}
