using Godot;
using System;

public class Player : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export] // sends it to the editor
    private float playerMass;
    [Export]
    private Vector3 playerVelocity; //Should be in meters per second
    [Export]
    private Vector3 playerForce;
    [Export]
    private float playerGravity;
    [Export]
    private float forwardForceIncrements; // change the forward and backwards force
    [Export]
    private float sidewaysForceIncrements; // change the sideways force
    [Export]
    private float frictionConstant;
    [Export]
    private float forceDeadZone;
    // the amount of force the WASD buttons do to the players

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // using kilograms as mass. Yay
        playerMass = 55; // kilograms, 121 pounds in American units
        playerForce = new Vector3(); //sets everything to 0
        playerVelocity = new Vector3(); //ditto
        playerGravity = 9.8f;
        forwardForceIncrements = 10;
        sidewaysForceIncrements = forwardForceIncrements;
        frictionConstant = 0.5f;
        forceDeadZone = 1.0f;
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  // delta is in seconds
    public override void _PhysicsProcess(float delta)
    {
      // Input Stuff
      ProcessInput(delta);
      ProcessMovement(delta);
        
    }

    private void ProcessInput(float delta)
    {
      //movement is forced based because I want it to be based on real life physics
      if(Input.IsActionPressed("move_up"))
      {
          playerForce.x += forwardForceIncrements;
          GD.Print("Move up");
      }
      if(Input.IsActionPressed("move_down"))
      {
          playerForce.x -= forwardForceIncrements;
          GD.Print("Move down");
      }
      if(Input.IsActionPressed("move_left"))
      {
          playerForce.z -= sidewaysForceIncrements;
          GD.Print("Move Left");
      }
      if(Input.IsActionPressed("move_right"))
      {
          playerForce.z += sidewaysForceIncrements;
          GD.Print("Move right");
      }
    }
    
    private void ProcessMovement(float delta)
    {
      /*
      * Physics is based on actual AP Physics Kinematic Equations lol
      */
      ProcessFriction();
      // x and z is 2d movement, and y is vertical movement
      // Force = mass * acceleration
      Vector3 playerAcceleration = new Vector3(playerForce.x / playerMass, 
        playerForce.y / playerMass, playerForce.z / playerMass);

      // speed is in meters per second.
      // new veloctiy = old velocity + acceleration * time (in this case 1 sec)
      playerVelocity = new Vector3(playerVelocity.x + playerAcceleration.x, 
        playerVelocity.y + playerAcceleration.y, playerVelocity.z + playerAcceleration.z);

      MoveAndSlide(playerVelocity);
      //Console.WriteLine(playerVelocity.ToString());
      GD.Print(playerVelocity.ToString());
    }

    private void ProcessFriction()
    {
      // first calculate friction, to make sure that the player slows down over time
      // friction is supposed to be the friction constant * normal
      // normal force = mass * gravity * cos(angle of slope the object is at)
      // for now, the angle is going to be 0, making normal = mg
      // which makes friction = friction constant * mass * gravity
      float frictionForce = frictionConstant * playerMass * playerGravity;
      GD.Print(frictionForce);
      // Friction on the x axis
      if(playerForce.x > forceDeadZone)
        playerForce.x -= frictionForce;
      if(playerForce.x < -forceDeadZone)
        playerForce.x += frictionForce;
      // make this 0 when its inside the force dead zone
      if((-forceDeadZone < playerForce.x) && (playerForce.x < forceDeadZone))
      {
        playerForce.x = 0;
      }

      // Firction on the z axis
      if(playerForce.z > forceDeadZone)
        playerForce.z -= frictionForce;
      if(playerForce.z < -forceDeadZone)
        playerForce.z += frictionForce;
      // make this 0 when its inside the force dead zone
      if((-forceDeadZone < playerForce.z) && (playerForce.x < forceDeadZone))
      {
        playerForce.z = 0;
      }
    }
}
