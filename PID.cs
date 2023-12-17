using Godot;
using MonoCustomResourceRegistry;
using System;

[RegisteredType(nameof(PID), "", nameof(Resource))]
public partial class PID : Resource
{
    //Pulled from this video: https://www.youtube.com/watch?v=y3K6FUgrgXw
    //Turned into a resource with this video: https://www.youtube.com/watch?v=yUmY3Gi3z5U


    //Proportional term is the Main corrective attribute. Usually between 0 & 1. This returns the commond opposite to the current error.
    [Export] public float proportionalGain { get; set; }
	//Integral fixes consistant error, like gravity. Usually between 0 & 1.
	[Export] public float integralGain { get; set; }
	//Derivative is the Dampening attribute. It will prevent the object from overshooting its target. Might be a value like 0.1f.
	[Export] public float derivativeGain { get; set; }

    private float integrationStored { get; set; }
    //This clamps how much the integral can build up. Too much integral can cause overshoot. Should be similar to the input values of the system; probably 1.0f.
    [Export] public float integralSaturation { get; set; }

	//What are the inputs the system should be using.
	//Usually -1 to 1 for things that can run forward and reverse, like a motor.
	//0.0f to 1.0f for things that turn on and off, like a thruster
	[Export] public float outputMin { get; set; }
	[Export] public float outputMax { get; set; }


    private float errorLast;
	private float valueLast;
	private bool derivativeInitialized;
    [Export] public DerivativeMeasurement derivativeMeasurement;

    public enum DerivativeMeasurement
	{
		Velocity,
		ErrorRateOfChange
	}

	[Export] public PIDInputType inputType = PIDInputType.Linear;

	public enum PIDInputType
	{
		Linear,
		AngleDegrees,
		AngleRadians,
	}

    //constructor for when you instantiate this class in another script. Make sure it also gets called every frame.
    public PID()
    {
        proportionalGain = 0.5f;
        integralGain = 0.5f;
        derivativeGain = 0.1f;
        integralSaturation = 1.0f;
        outputMin = -1.0f;
        outputMax = 1.0f;
        derivativeMeasurement = DerivativeMeasurement.Velocity;
        inputType = PIDInputType.Linear;
    }

    public float Controller(double delta, float currentValue, float targetValue)
	{
		//Error is the difference between the current value and target value
		float error = targetValue - currentValue;

		//calculate P term.
		float P = proportionalGain * error;

		//calculate both D terms. Select between linear, angle with Degrees and angle with Radians
		float errorRateOfChange = 0;
		float valueRateOfChange = 0;

        if (inputType == PIDInputType.Linear)
		{
            errorRateOfChange = (error - errorLast) / (float)delta;
            errorLast = error;

            valueRateOfChange = (currentValue - valueLast) / (float)delta;
            valueLast = currentValue;
        }
        else if (inputType == PIDInputType.AngleDegrees)
        {
            errorRateOfChange = AngleDifferenceDegrees(error, errorLast) / (float)delta;
            errorLast = error;

            valueRateOfChange = AngleDifferenceDegrees(currentValue, valueLast) / (float)delta;
            valueLast = currentValue;
        }
        else if (inputType == PIDInputType.AngleRadians)
        {
            errorRateOfChange = AngleDifferenceRadians(error, errorLast) / (float)delta;
            errorLast = error;

            valueRateOfChange = AngleDifferenceRadians(currentValue, valueLast) / (float)delta;
            valueLast = currentValue;
        }


        //choose which D term to use.
        float deriveMeasure = 0.0f;

		if (derivativeInitialized)
		{
            if (derivativeMeasurement == DerivativeMeasurement.Velocity)
            {
                deriveMeasure = -valueRateOfChange;
            }
            else
            {
                deriveMeasure = errorRateOfChange;
            }
        }
		else
		{
			derivativeInitialized = true;
		}

		float D = derivativeGain * deriveMeasure;

		//calculate I term
		integrationStored = Mathf.Clamp(integrationStored + (error * (float)delta), -integralSaturation, integralSaturation);
		float I = integralGain * integrationStored;

		//calculate the output
		float result = P + I + D;

		return Mathf.Clamp(result, outputMin, outputMax);
	}

	public void Reset()
	{
		derivativeInitialized = false;
	}

	private float AngleDifferenceDegrees(float a, float b)
	{
		return (a - b + 540) % 360 - 180;
	}

	private float AngleDifferenceRadians(float a, float b)
	{
		return (float)(a - b + (Math.PI * 1.5) % Math.PI - (Math.PI / 2.0));
	}
}
