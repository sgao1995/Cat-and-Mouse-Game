﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CatMovement : MonoBehaviour
{
    // stats
    private int level = 0;
    private float currentEXP = 0;
    private float maxEXP;
    public float power;
    private float speed = 3.0f; //speed value
    private float jumpForce;//amount of jump force
    public float currentHealth = 100;
    private float maxHealth;
    private int skillPoints;
    private int ultimateSkillPoints;
    private int[] learnedSkills = { 3, 4, 5, 6 };
    private float damage = 50f;
    // movement speed
    private int movementModifier = 1;
    private float movementModifierTimer = 10f;

    // attack
    private Animator animator;
    private float attackPower;
    private float attackCooldownDelay;
    private float attackCooldownTimer = 1f;

    private Vector3 moveV; //vector to store movement
    public Rigidbody catrb;

	
	// jump variables
	private bool isGrounded = false;
	// status effects
	private bool onLava = false;
	private bool onIce = false;
	private bool onSpikes = false;
	private bool alive = true;
	private bool canToggleDoor = false;
	private bool canMove = true;
	
	// skills
	private float[] skillCooldownTimers = new float[4]; // the cooldown timer
	private float[] skillCooldowns = new float[4]; // the max cooldown

    /* Vitality System attribute parameters */
    private float[] vitalLevelHP = {100, 125, 160, 200};  // Health Points of Cat per Level
    private float[] vitalLevelEXP = {200, 400, 800, 2000};  // Experience Points Cat per Level

    /* HUD state */
    public Vitality catVitality;  // Vitality System component
    public Skill catSkill;  // Skill System component
	public Text interactText;

    void Start()
    {
        catrb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; //cursor is gone from screen

        LevelUp();  // Starts at the first level
        animator = GetComponent<Animator>();

        /*  Finds and initialises the Vitality System component */
		GameObject catVitalityGameObject = GameObject.Find("Vitality");
		catVitality = catVitalityGameObject.GetComponent<Vitality>();

        /*  Finds and initialises the Skill System component */
        GameObject catSkillGameObject = GameObject.Find("Skill");
        catSkill = catSkillGameObject.GetComponent<Skill>();

        GameObject interactiveText = GameObject.Find("Text");
		interactText = interactiveText.GetComponent<Text>();
		interactText.text = "";

       
    }

	// level up
	public void LevelUp(){
		level += 1;
		if (level == 4)
			ultimateSkillPoints += 1;
		else
			skillPoints += 1;
		power = 5 + level * 5;
		attackPower = power;
		maxHealth = 80 + level * 20;
		jumpForce = 300f;
		attackCooldownDelay = 1.1f;
		
        /* Sets Character Maximum Health for new Level */
        this.maxHealth = this.vitalLevelHP[this.level - 1];
        this.currentHealth = this.maxHealth;

        /* Sets Character Maximum Experience for new level */
        this.maxEXP = this.vitalLevelEXP[this.level - 1];
        this.currentEXP = 0;
    }
	
	// execute a skill (not jump or attack)
	public void useSkill(int skillCode){
		switch (skillCode){
			// skills 1 and 2 are passive
			case 3: 
				Debug.Log("use 3");
				break;
			case 4:
				Debug.Log("use 4");
				break;
			case 5: 
				Debug.Log("use 5");
				break;
			case 6:
				Debug.Log("use 6");
				break;
			case 7: 
				break;
			case 8:
				break;
			case 9: 
				break;
		}
	}
	
    void FixedUpdate()
    {
        /* Updates the HUD state for the current player */
        if (GetComponent<PhotonView>().isMine)
        {
            /* Updates the Vitality System states */

            /* Updates the Level attributes */
            catVitality.setCurrentLevel(this.level);

            /* Updates the Health Points attributes of the Cat */
            catVitality.setMaxHealthPoints(this.maxHealth); // Updates the Maximum Health Points
            catVitality.setCurrentHealthPoints(this.currentHealth);  // Updates the Current Health Points

            /* Updates the Experience Points attributes */
            catVitality.setMaximumExperiencePoints(this.maxEXP);
            catVitality.setCurrentExperiencePoints(this.currentEXP);

            /* Updates the Character Type attribute as a Cat */
            catVitality.setCharacterType("cat");

            /* Updates the number of Skill System states */

            /* Updates the total number of Skill Slots */
            catSkill.setMaxSkillSlots(4);  // Set to 4

            /* Updates the number of Skill Slots enabled */
            catSkill.setNumSkillSlots(this.level);
        }

        // status effects
        if (onLava){
			TakeDamage(0.5f);
		}
		if (onSpikes){
			TakeDamage(0.2f);
		}
		if (canMove)
        {	
			if (onIce){
				moveV = moveV.normalized * speed * movementModifier * Time.deltaTime * 0.1f;
			}
			else{
				moveV = moveV.normalized * speed * movementModifier * Time.deltaTime;
			}
            transform.Translate(moveV);

			// movement control
			if (isGrounded && !onSpikes)
			{
				moveV = new Vector3(0, 0, 0);

				if (Input.GetKey(KeyCode.A))
				{
				   animator.Play("MoveLeft");

					if (onIce)
						catrb.AddRelativeForce(Vector3.left*0.2f, ForceMode.Impulse);
					else{
						moveV = new Vector3(-1, 0, moveV.z);
					}
				}
				if (Input.GetKey(KeyCode.D))
				{
					animator.Play("MoveRight");
					
					if (onIce)
						catrb.AddRelativeForce(Vector3.right*0.2f, ForceMode.Impulse);
					else{
						moveV = new Vector3(1, 0, moveV.z);
					}
				}
				if (Input.GetKey(KeyCode.W))
				{
					if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
						animator.Play("MoveForward");
					
					if (onIce)
						catrb.AddRelativeForce(Vector3.forward*0.2f, ForceMode.Impulse);
					else{
						moveV = new Vector3(moveV.x, 0, 1);
					}
				}
				if (Input.GetKey(KeyCode.S))
				{
					if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
						animator.Play("MoveBackward");
					
					if (onIce)
						catrb.AddRelativeForce(Vector3.back*0.2f, ForceMode.Impulse);
					else{
						moveV = new Vector3(moveV.x, 0, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.Space))
				{
					isGrounded = false;
				   // animator.Play("Unarmed-Jump");
					animator.SetTrigger("JumpTrigger");
					animator.SetInteger("Jumping", 1);
					catrb.AddForce(new Vector3(0, jumpForce, 0));
				}
			}
		}
		
		// left click
		if (Input.GetMouseButtonDown(0) && attackCooldownTimer <= 0 && !Input.GetKey(KeyCode.Escape))
        {
			attackCooldownTimer = attackCooldownDelay;
			StartCoroutine(Attack());
		}
		// skills
		if (Input.GetKeyDown(KeyCode.Alpha1)){
            catSkill.useSkillSlot(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)){
            catSkill.useSkillSlot(2);
        }
		if (Input.GetKeyDown(KeyCode.Alpha3)){
            catSkill.useSkillSlot(3);
        }
		if (Input.GetKeyDown(KeyCode.Alpha4)){
            catSkill.useSkillSlot(4);
        }
		
		if (canToggleDoor){
			if (Input.GetKeyDown(KeyCode.E)){
				InteractWithObject();
			}
		}
		if (Input.GetKeyDown(KeyCode.K)){
			TakeDamage(5f);
		}
		if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None; //if we press esc, cursor appears on screen
        }
		
		// timer actions
		if (movementModifierTimer > 0f)
			movementModifierTimer -= Time.deltaTime;
		else{
			movementModifier = 1;
		}
		if (attackCooldownTimer > 0){
			attackCooldownTimer -= Time.deltaTime;
		}

    }

    // take a certain amount of damage
    public void TakeDamage(float amt)
    {
        transform.GetComponent<PhotonView>().RPC("changeHealth", PhotonTargets.AllBuffered, amt);
    }
    [PunRPC]
    void changeHealth(float dmg)
    {
		if (alive){
			currentHealth -= dmg;
			if (currentHealth <= 0)
			{
				currentHealth = 0;
				Death();
			}
		}
    }

    void Death(){
		Debug.Log("player died");
		alive = false;
		animator.SetTrigger("Death1Trigger");
		//animator.Play("Unarmed-Death1");
	}

    // attack in front of player
    IEnumerator Attack()
    {
		canMove = false;
		float attackType = Random.Range(0f, 1f);
		if (attackType <= 0.5f)
			animator.SetTrigger("Attack3Trigger");
		else if (attackType > 0.5f)
			animator.SetTrigger("Attack6Trigger");
		DealDamage();
		yield return new WaitForSeconds(0.7f);
		canMove = true;
    }
	
    void DealDamage()
    {
		RaycastHit hitInfo;
		
        if (Physics.SphereCast(transform.position, 0.2f, transform.forward, out hitInfo, 1))
        {
            Debug.Log("We hit: " + hitInfo.collider.name);
            if (hitInfo.collider.name == "Character" || hitInfo.collider.name == "Monster(Clone)" || hitInfo.collider.name == "Monster" || hitInfo.collider.tag == "Monster")
            {
                Debug.Log("Trying to hurt " + hitInfo.collider.transform.name + " by calling script " + hitInfo.collider.transform.GetComponent<MonsterAI>().name);
				
				if (hitInfo.collider.transform.GetComponent<MonsterAI>().getHealth() > 0 && hitInfo.collider.transform.GetComponent<MonsterAI>().getHealth() - damage <= 0){
					currentEXP += hitInfo.collider.transform.GetComponent<MonsterAI>().getExpDrop();
					//mouseVitality.setCurrentExperiencePoints(currentEXP);
				}

				hitInfo.collider.transform.GetComponent<MonsterAI>().SendMessage("takeDamage", damage);

            }
            if (hitInfo.collider.tag == "Mouse")
            {
                Debug.Log("Trying to hurt " + hitInfo.collider.transform.name + " by calling script " + hitInfo.collider.transform.GetComponent<MouseMovement>().name);

                if (hitInfo.collider.transform.GetComponent<MouseMovement>().getHealth() > 0 && hitInfo.collider.transform.GetComponent<MouseMovement>().getHealth() - damage <= 0)
                {
                    GameObject.Find("WinObj").GetComponent<WinScript>().setMouseDeaths();
                    currentEXP += 100;
                }
				
				hitInfo.collider.transform.GetComponent<MouseMovement>().SendMessage("TakeDamage", damage);
            }
        }
    }
    
    // allow the player to open and close doors
    void InteractWithObject(){
		Vector3 pushCenter = transform.position + transform.forward * 0.6f;
		Collider[] hitColliders = Physics.OverlapSphere(pushCenter,0.8f);
		// check the hitbox area
        int i = 0;
        while (i < hitColliders.Length) {
			if (hitColliders[i].tag == "Door"){
				hitColliders[i].transform.GetComponent<MazeDoor>().Interact();
			}
            i++;
        }
	}
	
	// collision with objects
	void OnCollisionEnter(Collision collisionInfo){
		isGrounded = true;
		animator.SetInteger("Jumping", 0);
		if (collisionInfo.gameObject.tag == "Ground"){
			onLava = false;
			onIce = false;
		}
		// if enters lava
		if (collisionInfo.gameObject.tag == "Lava"){
			onLava = true;
		}
		if (collisionInfo.gameObject.tag == "Ice")
        {
            onIce = true;
        }
		// if steps on a mine
		if (collisionInfo.gameObject.tag == "Mine"){
			Mine mine = collisionInfo.gameObject.GetComponent<Mine>();
			TakeDamage(mine.mineSize * 50);
            transform.GetComponent<PhotonView>().RPC("destroyMine", PhotonTargets.MasterClient, collisionInfo);
        }
	}
    [PunRPC]
    void destroyMine(Collision collisionInfo)
    {

        PhotonNetwork.Destroy(collisionInfo.gameObject);
    }
    [PunRPC]
    void destroyPU(Collider obj)
    {

        PhotonNetwork.Destroy(obj.gameObject);
    }
	
	
	void OnTriggerStay(Collider obj){
		// enters range to use an object. Certain objects take priority over others
		if (obj.tag == "Door"){
			MazeDoor door = obj.gameObject.GetComponent<MazeDoor>();
			if (door.doorOpen){
				interactText.text = "Press E to close Door";
			}
			else{
				interactText.text = "Press E to open Door";
			}
			canToggleDoor = true;
		}
	}
	
	// when player leaves the trigger
	void OnTriggerExit(Collider obj){

		if (obj.tag == "Spike"){
			onSpikes = false;
			canMove = true;
		}
		if (obj.tag == "Door"){
			interactText.text = "";
			canToggleDoor = false;
		}
	}
	
	// when player enters range of something
	void OnTriggerEnter(Collider obj){
		if(obj.tag == "Powerup"){
			Powerup pup = obj.GetComponent<Powerup>();
			// movement speed boost
			if (pup.powerupType == 0){
				movementModifier = 2;
				movementModifierTimer = 10f;
			}
			// damage boost
			else if (pup.powerupType == 1){
				
			}
			// invis
			else if (pup.powerupType == 2){
				
			}
			// exp boost
			else if (pup.powerupType == 3){
				
			}
            //Debug.Log("destroy " + obj);
            transform.GetComponent<PhotonView>().RPC("destroyPU", PhotonTargets.MasterClient, obj);
    }
		// put spikes here because we dont want spikes displacing the player
		if (obj.tag == "Spike"){
			onSpikes = true;
			canMove = false;
		}
	}
    public float getHealth()
    {
        return this.currentHealth;
    }
}