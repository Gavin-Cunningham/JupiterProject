using Godot;
using System;

public partial class CameraTest : Node3D
{
    private float mouse_sensitivity = 0.001f;
    private float twist_input = 0.0f;
    private float pitch_input = 0.0f;

    [Export] private Node3D _twistPivot = null;
    [Export] private Node3D _pitchPivot = null;
    [Export] private Node3D viewTarget = null;

    public override void _Ready()
    {
        //Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {

        //if (Input.IsActionJustPressed("pause"))
        //{
        //    Input.MouseMode = Input.MouseModeEnum.Visible;
        //}

        _twistPivot.RotateY(twist_input);
        pitch_input = Math.Clamp(pitch_input, -0.5f, 0.5f);
        _pitchPivot.Rotation = new Vector3(pitch_input, 0.0f, 0.0f);
        twist_input = 0.0f;

        if (viewTarget != null )
        {
            _twistPivot.GlobalPosition = viewTarget.GlobalPosition;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                InputEventMouseMotion motion = (InputEventMouseMotion)@event;
                twist_input -= motion.Relative.X * mouse_sensitivity;
                pitch_input -= motion.Relative.Y * mouse_sensitivity;
            }
        }
    }
}