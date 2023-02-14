using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private ParticleSystem jumpParticles;
	[SerializeField] private ParticleSystem dashParticles;
	[SerializeField] private AudioSource jumpAudio;
	[SerializeField] private AudioSource dashAudio;
	[SerializeField] private AudioSource deathAudio;
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private float m_moveSpeedMultiplier = 2f;                // A collider that will be disabled when crouching
	[SerializeField] public int m_midAirJumps = 1;                // A collider that will be disabled when crouching
	[SerializeField] public int m_currentMidAirJumps = 0;                // A collider that will be disabled when crouching
	[SerializeField] private float m_dashForce = 2f;                // A collider that will be disabled when crouching
	[SerializeField] public int m_midAirDashs = 1;                // A collider that will be disabled when crouching
	[SerializeField] public int m_currentMidAirDashs = 0;                // A collider that will be disabled when crouching
	[SerializeField] private bool allowedToJump = true;
	[SerializeField] private bool allowedToDash = true;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

    private void Update()
    {
		Move(Input.GetAxis("Horizontal") * m_moveSpeedMultiplier, Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W), Input.GetKeyDown(KeyCode.LeftShift));

		if (Input.GetKeyDown(KeyCode.R))
        {
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene("MainMenu");
		}
	}

    public void Move(float move, bool jump, bool dash)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		}

		if (jump && allowedToJump)
			Jump();

        if (dash && allowedToDash)
			Dash();
	}

	private void Jump()
    {
		if (m_Grounded)
		{
			m_Grounded = false;
			m_Rigidbody2D.velocity += Vector2.up * m_JumpForce;
			jumpParticles.transform.position = m_GroundCheck.position;
			jumpParticles.Play();
			//jumpAudio.Play();
			ResetMidAirJumps();
			ResetMidAirDashs();
		}
		else if (!m_Grounded && m_currentMidAirJumps > 0)
		{
			m_currentMidAirJumps--;
			jumpParticles.transform.position = m_GroundCheck.position;
			jumpParticles.Play();
			//jumpAudio.Play();
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0) + (Vector2.up * m_JumpForce);
		}
	}

	private void Dash()
	{
		float dashDirection = Input.GetAxis("Horizontal");

		if (dashDirection == 0)
			return;

		dashDirection = (dashDirection > 0) ? 1 : -1; 

		if (m_Grounded)
		{
			dashParticles.startSpeed = 5 * -dashDirection;
			dashParticles.transform.position = transform.position;
			dashParticles.Play();
			//dashAudio.Play();
			//Destroy(dashParticlesInstance.gameObject, 2f);
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Rigidbody2D.velocity = transform.right * dashDirection * m_dashForce;
			ResetMidAirJumps();
			ResetMidAirDashs();
		}
		else if (!m_Grounded && m_currentMidAirDashs > 0)
		{
			m_currentMidAirDashs--;
			dashParticles.startSpeed = 5 * -dashDirection;
			dashParticles.transform.position = transform.position;
			dashParticles.Play();
			//dashAudio.Play();
			//Destroy(dashParticlesInstance.gameObject, 2f);
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Rigidbody2D.velocity = transform.right * dashDirection * m_dashForce;
		}

	}

	public void ResetMidAirJumps()
	{
		m_currentMidAirJumps = m_midAirJumps;
	}

	public void ResetMidAirDashs()
    {
		m_currentMidAirDashs = m_midAirDashs;
    }

	public void AddMidAirJumps(int count)
    {
		m_currentMidAirJumps += count;
	}

	public void AddMidAirDashs(int count)
	{
		m_currentMidAirDashs += count;
	}
}
