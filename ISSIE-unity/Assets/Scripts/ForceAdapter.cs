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

    private WebSocketController.PowerSignal getRawPower()
    {
        return socket.getHumanPower();
    }

    public float getHumanPower()
    {
		Debug.Log ("\nGot here!!");

        //socket is initialized in the websocketcontroller onGUI() load the first time
		if (socket == null) {
			Debug.Log ("\nNo connection, no power!");
			return (float)0.0;
		}

        WebSocketController.PowerSignal signal = getRawPower(); //this is raw human power

        //this should only return when it gets both steps not just one.
        if ((signal.getForce() < 0 && !wasNeg) || (signal.getForce() > 0 && wasNeg))
        {
            wasNeg = !wasNeg;
            float power = getForceFactoredByExerciseType(signal.getForce(), signal.getDirection());
            power = getForceFactoredByNumSteps(power, numSteps);
            numSteps++;
            return power;
        }
        return (float)0.0;
    }
    public float getForceFactoredByExerciseType(float force, WebSocketController.DIRECTION direction)
    {
        //hardcode for now
        float multiplier = (direction == WebSocketController.DIRECTION.UP 
             || direction == WebSocketController.DIRECTION.DOWN) ? 2.0f : 1.0f;
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
            numSteps = 0;
            return force * HYPERDRIVE_FACTOR;
        }
        return force;
    }
}
