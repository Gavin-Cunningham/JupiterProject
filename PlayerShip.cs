using Godot;
using System;

public partial class PlayerShip : RigidBody3D
{
	private Vector2 input;
    [Export] private Vector3 rotationSensitivity = new Vector3(1, 1, 1);
    private float sublightThrottle = 0.0f;
	[Export] Node3D playerShip;

    [ExportCategory("Indicators")]
    [Export] private TextureRect inputIndicator;
	[Export] private TextureRect throttleIndicator;
	[Export] private Label labelXYZ;

	[Export] private PID ShipRollPID;

	public override void _Ready()
	{
		//rollPID = new PID(proportionalGain, integralGain, derivativeGain, integralSaturation, minOutput, maxOutput, measurementType, pidInputType);
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleShipRotation(delta);
		UpdateXYZLabel();
		ApplyCentralForce(-Transform.Basis.Z * sublightThrottle * 100000);
    }

	private void HandleShipRotation(double delta)
	{
        input.X = Input.GetAxis("turnLeft", "turnRight");
        input.Y = Input.GetAxis("pitchUp", "pitchDown");
		input = input.Normalized();

		inputIndicator.Position = input * 80 + new Vector2(80, 80);

		RotateObjectLocal(Vector3.Up, -input.X * (float)delta * rotationSensitivity.X);
		RotateObjectLocal(Vector3.Right, -input.Y * (float)delta * rotationSensitivity.Y);
        RotateObjectLocal(Vector3.Forward, ShipRollPID.Controller(delta, -Rotation.Z, 0.0f) * rotationSensitivity.Z);
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("increaseSublightSpeed"))
		{
			sublightThrottle = Mathf.Clamp(sublightThrottle + 0.25f, 0.0f, 1.0f);
            HandleThrottleIndicator(sublightThrottle);
        }

		if (@event.IsActionPressed("decreaseSublightSpeed"))
		{
            sublightThrottle = Mathf.Clamp(sublightThrottle - 0.25f, 0.0f, 1.0f);
			HandleThrottleIndicator(sublightThrottle);
        }
	}

	private void HandleThrottleIndicator(float throttle)
	{
		throttleIndicator.Position = new Vector2(throttleIndicator.Position.X, 160.0f - (throttle * 160.0f));
	}

	private void UpdateXYZLabel()
	{
		labelXYZ.Text = "X: " + RotationDegrees.X + "\n Y: " + RotationDegrees.Y + "\n Z: " + RotationDegrees.Z;
        //labelXYZ.Text = "X: " + Rotation.X + "\n Y: " + Rotation.Y + "\n Z: " + Rotation.Z;
    }
}
