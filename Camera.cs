using Godot;
using System;

public partial class Camera : Camera3D
{
    [Export] private Node3D cameraFocus;
    [Export] private float mouseSensitivity = 0.1f;
    [Export] private Node3D followTarget;
    private Vector2 cameraInput;

    private float twist_input;
    private float pitch_input;

	public override void _Ready()
	{
        //Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Process(double delta)
	{
        //cameraFocus.RotateY(cameraInput.Y);
        //cameraFocus.RotateX(cameraInput.X);

        //cameraFocus.GlobalPosition = followTarget.GlobalPosition;

        //if (Input.IsActionJustPressed("pause"))
        //{
        //    Input.MouseMode = Input.MouseModeEnum.Visible;
        //}

        //
        cameraFocus.RotateY(twist_input);
        pitch_input = Math.Clamp(pitch_input, -0.5f, 0.5f);
        cameraFocus.Rotation = new Vector3(pitch_input, 0.0f, 0.0f);

        twist_input = 0.0f;
        //
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                InputEventMouseMotion motion = (InputEventMouseMotion)@event;
                //cameraInput -= new Vector2(motion.Relative.X * mouseSensitivity, motion.Relative.Y * mouseSensitivity);
                //cameraInput = new Vector2(Mathf.Clamp(cameraInput.X, -1.5f, 1.5f), cameraInput.Y);

                //
                twist_input -= motion.Relative.X * mouseSensitivity;
                pitch_input -= motion.Relative.Y * mouseSensitivity;
                //
            }
        }
    }
}
