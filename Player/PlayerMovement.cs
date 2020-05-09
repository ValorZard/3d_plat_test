using Godot;
using System;

public class PlayerMovement : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    
    // Physics Constants
    [Export]
    private float gravity;
    [Export]
    private float maxSpeed;
    [Export]
    private float jumpSpeed;
    [Export]
    private float groundAccel;
    [Export]
    private float groundDecel;
    [Export]
    private float friction;
    [Export]
    private float frictionDeadZone;
    [Export]
    private Vector3 playerVelocity;
    [Export]
    private Vector3 playerDirection;
    
    [Export]
    private float currentGroundSpeed;

    [Export]
    private bool hasInputMovement;

    private bool pressedJump;

    private Camera _camera;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gravity = 60f;
        maxSpeed = 50.0f; // speed is magnitude, Velocity is vector
        jumpSpeed = 30.0f;
        groundAccel = 30f;
        groundDecel = 60f;
        friction = 25.0f;

        frictionDeadZone = 0.3f;

        hasInputMovement = false;

        pressedJump = false;

        // direction of the speed
        playerVelocity = new Vector3();
        // direction of player input
        playerDirection = new Vector3();

        // the magnitude of player's current speed
        currentGroundSpeed = 0;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        ProcessInput(delta);
        ProcessMovement(delta);
    }

    private void ProcessInput(float delta)
    {

        // the magnitute of the input should be 1, but the vector values themselves can be less than that
        
        //forward and backward
        float forwardMovement = Input.GetActionStrength("move_up");
        float backwardMovement = Input.GetActionStrength("move_down");

        playerDirection.x = forwardMovement - backwardMovement;

        //horizontal direction
        float leftMovement = Input.GetActionStrength("move_left");
        float rightMovement = Input.GetActionStrength("move_right"); // right is positive

        playerDirection.z = rightMovement - leftMovement;

        playerDirection = playerDirection.Normalized(); //makes the maginitude equal to 1.

        pressedJump = Input.IsActionPressed("jump");
    }

    private void ProcessMovement(float delta)
    {
        ProcessHorizontalMovement(delta);
        
        ProcessJump(delta);

        MoveAndSlide(playerVelocity, upDirection : new Vector3(0, 1, 0)); // makes the ground a floor
        
    }

    private void ProcessHorizontalMovement(float delta)
    {
        /* 
        * accel starts at 0, and then times delta with accel, which should make it smooth
        */

        //Vector3 playerAcceleration = new Vector3(playerDirection.x * accel, 0, playerDirection.z * accel);

        //Vector3 playerFriction = new Vector3(playerDirection.x * friction, 0, playerDirection.z * friction);

        hasInputMovement = playerDirection != Vector3.Zero; // if the player is holding a direction

        if(hasInputMovement){

            GD.Print("Old Velocity: " + playerVelocity);

            float currentGroundAccel = groundAccel * delta; //should make the acceleration plenty smooth

            playerVelocity += playerDirection * currentGroundAccel; // Velocity is bein increased by the player direction * magitude of the current acceleration

            
            if(playerDirection != Horizontalize(playerVelocity).Normalized())
            {
                float currentGroundDeAccel = groundAccel * delta; //should make the decceleration plenty smooth

                // the purpose of the below code is to make sure that it changes direction faster.
                // it takes the normalized unity of player velocity and then turns it into a decelleration unit
                // so basically i hate my life or wife if i was a boomer
                playerVelocity -= Horizontalize(playerVelocity).Normalized() * currentGroundDeAccel; // to make deceleartion much less slippery
            }
            
        }
        else
        {
            float currentFriction = friction * delta;

            // makes friction act on velocity, slows it down
            playerVelocity -= (Horizontalize(playerVelocity).Normalized() * currentFriction);

            // if velocity small enough, just make it 0
            if(Horizontalize(playerVelocity).Length() < frictionDeadZone)
                playerVelocity = new Vector3(0, playerVelocity.y, 0);
        }

        GD.Print("New Velocity: " + playerVelocity);
    }

    private void ProcessJump(float delta)
    {
        // If the thing is touching another object (and right now it thinks everything is a wall), then it can jump
        if(IsOnFloor() && pressedJump)
        {
            playerVelocity.y = jumpSpeed;
        }
        else if(!IsOnFloor())
        {
            playerVelocity.y -= gravity * delta;
            GD.Print(playerVelocity.y);
        }
        else if(IsOnFloor() && !pressedJump)
        {
            playerVelocity.y = 0;
        }
    }

    private Vector3 Horizontalize(Vector3 vector)
    {
        // removes vertical component
        return new Vector3(vector.x, 0, vector.z);
    }
}
