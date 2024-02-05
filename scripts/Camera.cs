using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Camera : Node3D
{
    private float mouse_sensitivity = 0.001f;
    private float twist_input = 0.0f;
    private float pitch_input = 0.0f;

    private float cameraDistance = 40.0f;

    [Export] private Node3D _twistPivot = null;
    [Export] private Node3D _pitchPivot = null;
    [Export] private Camera3D _camera = null;
    [Export] private Node3D viewTarget = null;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        if (viewTarget != null )
        {
            _twistPivot.GlobalPosition = viewTarget.GlobalPosition;
        }
    }

    //Turns out the reason this wasn't working was the UI Control node MouseFilter being set to stop instead of ignore.
    public override void _Input(InputEvent @event)
    {
        CheckPauseButton(@event);
        CheckMouseWheel(@event);
        GetMouseInput(@event);
        RotateCameraPivots();
    }

    private void CheckPauseButton(InputEvent theEvent)
    {
        if (theEvent.IsActionPressed("pause") && Input.MouseMode != Input.MouseModeEnum.Visible)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if (theEvent.IsActionPressed("pause"))
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    private void CheckMouseWheel(InputEvent theEvent)
    {
        float cameraMinDistance = 10.0f;
        float cameraMaxDistance = 750.0f;
        float cameraDistanceChange = 0.0f;
        //GD.Print("CheckMouseWheel has run");
        if (theEvent.IsActionPressed("zoomIn") && Input.MouseMode != Input.MouseModeEnum.Visible)
        {
            //GD.Print("zoomIn");
            //cameraDistance += Math.Clamp(cameraDistance + (cameraDistance / 10), cameraMinDistance, cameraMaxDistance);
            if (_camera.Transform.Origin.Z < cameraMaxDistance)
            {
                cameraDistanceChange = (_camera.Transform.Origin.Z / 10);
            }
            else
            {
                //cameraDistanceChange = -(_camera.Transform.Origin.Z / 10);
            }
        }
        else if(theEvent.IsActionPressed("zoomOut") && Input.MouseMode != Input.MouseModeEnum.Visible)
        {
            //cameraDistance = Math.Clamp(cameraDistance - (cameraDistance / 10), cameraMinDistance, cameraMaxDistance);
            //GD.Print("zoomOut");
            if (_camera.Transform.Origin.Z > cameraMinDistance)
            {
                cameraDistanceChange = -(_camera.Transform.Origin.Z / 10);
            }
            else
            {
                //cameraDistanceChange = (_camera.Transform.Origin.Z / 10);
            }

        }
        //GD.Print("distance: " + cameraDistance);
        //GD.Print("Camera Position: " + _camera.Transform.Origin);
        //_camera.TranslateObjectLocal(new Vector3 (0, ((_camera.Transform.Origin.Y - cameraDistance) / 4.0f), _camera.Transform.Origin.Z - cameraDistance));
        _camera.TranslateObjectLocal(new Vector3 (0.0f, cameraDistanceChange / 4.0f, cameraDistanceChange));
    }

    private void GetMouseInput(InputEvent theEvent)
    {
        if (theEvent is InputEventMouseMotion eventMouseMotion)
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                InputEventMouseMotion motion = (InputEventMouseMotion)theEvent;
                twist_input -= motion.Relative.X * mouse_sensitivity;
                pitch_input -= motion.Relative.Y * mouse_sensitivity;
            }
        }
    }

    private void RotateCameraPivots()
    {
        _twistPivot.RotateY(twist_input);
        pitch_input = Math.Clamp(pitch_input, -0.5f, 0.5f);
        _pitchPivot.Rotation = new Vector3(pitch_input, 0.0f, 0.0f);
        twist_input = 0.0f;
    }

}