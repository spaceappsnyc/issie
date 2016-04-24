using UnityEngine;

// Demo class
// inherits of WebSocketUnityDelegate to receive WebSockets events
public class ForceAdapter : MonoBehaviour
{
    public WebSocketController socket;
    private const int NUM_STEPS_TO_HYPERDRIVE = 500;
    private const int HYPERDRIVE_FACTOR = 100;
    private bool wasNeg = true;
    private int numSteps = 0;

    // Use this for initialization
    void Start()
    {
        //init socket here
    }

    private float getRawPower()
    {
        return socket.getHumanPower();
    }

    public float getHumanPower()
    {
        print("\nGot here!!");

        //socket is initialized in the websocketcontroller onGUI() load the first time
        float power = getRawPower(); //this is raw human power

        //this should only return when it gets both steps not just one.
        if ((power < 0 && !wasNeg) || (power > 0 && wasNeg))
        {
            wasNeg = !wasNeg;
            power = getForceFactoredByExerciseType(power);
            power = getForceFactoredByNumSteps(power, numSteps);
            numSteps++;
            return power;
        }
        return (float)0.0;
    }
    public float getForceFactoredByExerciseType(float force)
    {
        //hardcode for now
        float multiplier = (float)1.0;
        /*
         * All the logic to determine exercise types goes here
         * For example, if using an exercise bike, we may need multiplier = 5.0 (assume)
         * 
         * */
        return multiplier * force;
    }

    public float getForceFactoredByNumSteps(float force, int numSteps)
    {
        //hardcode for now
        if (numSteps > NUM_STEPS_TO_HYPERDRIVE)
        {
            //reset number of steps
            Debug.Log("Reset number of steps to 0");
            numSteps = 0;
            return force * HYPERDRIVE_FACTOR;
        }
        return force;
    }
}
