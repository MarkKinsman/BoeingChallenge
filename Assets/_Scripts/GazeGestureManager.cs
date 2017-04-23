using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public GameObject FocusedObject
    {
        get;
        private set;
    }

    private GestureRecognizer recognizer;

    void Start()
    {
        recognizer = new GestureRecognizer();

        recognizer.TappedEvent += Recognizer_TappedEvent;
        
        recognizer.StartCapturingGestures();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (FocusedObject != null)
        {
            FocusedObject.SendMessageUpwards("OnSelect");
        }
    }

    void Update()
    {
        GameObject previousObject = this.FocusedObject;

        // perform a raycast into the world based upon the user's head position and orientation
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit raycast;

        if (Physics.Raycast(headPosition, gazeDirection, out raycast))
        {
            FocusedObject = raycast.collider.gameObject;
        }
        else
        {
            FocusedObject = null;
        }

        if(this.FocusedObject != previousObject)
        {
            recognizer.CancelGestures();

            recognizer.StartCapturingGestures();
        }
    }
}
