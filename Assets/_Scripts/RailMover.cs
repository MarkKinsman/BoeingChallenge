using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMover : MonoBehaviour {

    public Rail rail;
    public PlayMode mode;
    public bool moveWorld;

    public float speed = 2.5f;
    public bool isReversed;
    public bool isLooping;
    public bool pingPong;

    private int currentSeg;
    private float transition;
    private bool isCompleted;

    private void Update()
    {
        {
            if (!rail)
                return;

            if (!isCompleted)
                Play(!isReversed);
        }
    }

    private void Start()
    {
        if (moveWorld)
        {
            rail.InvertNodes();
        }
    }

    private void Play(bool forward = true)
    {
        float magnitude = (rail.nodes[currentSeg + 1].position - rail.nodes[currentSeg].position).magnitude;
        float normalizedSpeed = (Time.deltaTime * 1 / magnitude) * speed;

        transition += (forward) ? normalizedSpeed : -normalizedSpeed;

        if(transition > 1)
        {
            transition = 0;
            currentSeg++;
            if(currentSeg == rail.nodes.Length -1)
            {
                if(isLooping)
                {
                    if(pingPong)
                    {
                        transition = 1;
                        currentSeg = rail.nodes.Length - 2;
                        isReversed = !isReversed;
                    }
                    else
                    {
                        currentSeg = 0;

                    }
                }
                else
                {
                    isCompleted = true;
                    return;
                }
            }
        }
        else if(transition < 0)
        {
            transition = 1;
            currentSeg--;
            if (currentSeg ==  - 1)
            {
                if (isLooping)
                {
                    if (pingPong)
                    {
                        transition = 0;
                        currentSeg = 0;
                        isReversed = !isReversed;
                    }
                    else
                    {
                        currentSeg = rail.nodes.Length - 2;

                    }
                }
                else
                {
                    isCompleted = true;
                    return;
                }
            }
        }

        transform.position = rail.PositionOnRail(currentSeg, transition, mode);
        transform.rotation = rail.Orientation(currentSeg, transition, moveWorld);
    }
}
