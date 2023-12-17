using Godot;
using System;

public partial class PlayerShip : RigidBody3D
{
    private Vector2 inputDampened = new Vector2(0.0f, 0.0f);
    [Export] private Vector3 rotationSensitivity = new Vector3(1, 1, 1);
    private float sublightThrottle = 0.0f;
	[Export] Node3D playerShip;

    [ExportCategory("Indicators")]
    [Export] private TextureRect inputIndicator;
	[Export] private TextureRect throttleIndicator;
	[Export] private Label labelXYZ;

	[Export] private PID ShipRollPID;

	public override void _PhysicsProcess(double delta)
	{
		HandleShipRotation(delta);
		UpdateXYZLabel();
		ApplyCentralForce(-Transform.Basis.Z * sublightThrottle * 100000);
    }

	private void HandleShipRotation(double delta)
	{
		Vector2 input;
        input.X = Input.GetAxis("turnLeft", "turnRight");
        input.Y = Input.GetAxis("pitchUp", "pitchDown");
        Vector2 inputNormalized = input.Normalized();

		inputIndicator.Position = inputNormalized * 80 + new Vector2(80, 80);

		Vector2 inputRateScaled = inputNormalized * 0.1f;

        inputDampened.X = Math.Clamp(inputDampened.X + inputRateScaled.X, -1.0f, 1.0f);
		inputDampened.Y = Math.Clamp(inputDampened.Y + inputRateScaled.Y, -1.0f, 1.0f);

		if (Math.Abs(inputDampened.X) - 0.025 > 0.0)
		{
			inputDampened.X += -Math.Sign(inputDampened.X) * 0.025f;
		}
		else if (Math.Abs(inputDampened.X) - 0.025 <= 0.0)
		{
			inputDampened.X = 0.0f;
		}

		if (Math.Abs(inputDampened.Y) - 0.025 > 0.0)
		{
			inputDampened.Y += -Math.Sign(inputDampened.Y) * 0.025f;
		}
        else if (Math.Abs(inputDampened.Y) - 0.025 <= 0.0)
        {
            inputDampened.Y = 0.0f;
        }

		RotateY(-inputDampened.X * (float)delta * rotationSensitivity.X);

		//RotateObjectLocal(Vector3.Up, -inputDampened.X * (float)delta * rotationSensitivity.X);
		RotateObjectLocal(Vector3.Right, -inputDampened.Y * (float)delta * rotationSensitivity.Y);
        RotateObjectLocal(Vector3.Forward, ShipRollPID.Controller(delta, -Rotation.Z, inputDampened.X * 0.5f) * rotationSensitivity.Z);
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
    }
}
