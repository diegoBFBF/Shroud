
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarGrabber : MonoBehaviour
{
    [SerializeField]
    bool leftHand;

    [SerializeField]
    public List<WheelStar> inReach = new List<WheelStar>();

    IEnumerator currentHold;

    [SerializeField]
    bool holding;


    // Update is called once per frame
    void Update()
    {
        holding = currentHold != null;

        if (inReach.Count <= 0) {  return; }

        InReachCheck();


        //ovr grab

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            Debug.Log("Gripped");
            GrabStar();
        }
    }

    public void GrabStar()
    {
        if (currentHold != null) return;

        StartCoroutine(currentHold = HoldStarRoutine());
    }

    public void SelectStar()
    {
        WheelStar currentStar = inReach[0];
        if((Object)currentStar == null) { return; }

        StartCoroutine(QuickSelectRoutine(currentStar));

    }

    public Vector3 GetClosestInReach()
    {
        if(inReach.Count <= 0) return Vector3.zero;
        if (inReach[0] == null) return Vector3.zero;

        return inReach[0].transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 5)
        {
            if (other.TryGetComponent(out WheelStar newPerkStar))
            {
                if (!inReach.Contains(newPerkStar))
                {
                    inReach.Add(newPerkStar);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (inReach.Count <= 0) return;

        if (other.gameObject.layer == 5)
        {
            if (other.TryGetComponent(out WheelStar newPerkStar))
            {
                if (inReach.Contains(newPerkStar))
                {
                    RemoveStar(inReach.IndexOf(newPerkStar));
                }
            }
        }
    }

    public void InReachCheck()
    {
        int removed = inReach.Count;
        for (int i = 0; i < inReach.Count; i++)
        {
            if ((Object)inReach[i] == null)//inReach[i] == null
            {
                RemoveStar(i);
                continue;
            }

            if (!inReach[i].transform.gameObject.activeInHierarchy)
            {
                RemoveStar(i);
                continue;
            }

            inReach[i].ScaleStarInfo(Vector3.Distance(transform.position, inReach[i].transform.position));
        }

        if (inReach.Count <= 1) return;

        inReach.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));

    }

    void RemoveStar(int starIndex)
    {
        if ((Object)inReach[starIndex] != null) inReach[starIndex].ScaleStarInfo(0);
        inReach.RemoveAt(starIndex);
    }

    IEnumerator HoldStarRoutine()
    {
        if (inReach.Count <= 0) { currentHold = null; yield break; }
        WheelStar currentStar = inReach[0];

        bool holding = true;
        while (holding)
        {
            if(OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) { holding = false; break; }

            if ((Object)currentStar == null) { currentHold = null; yield break; }

            currentStar.StarHeld(transform.position);
            yield return null;
        }

        if ((Object)currentStar == null) { currentHold = null; yield break; }

        currentStar.StarReleased();
        currentStar.StarSelected();

        currentHold = null;
        yield return null;
    }
    IEnumerator QuickSelectRoutine(WheelStar currentStar)
    {
        currentStar.StarSelected();
        float timer = 0;
        float selectTimer = 0.1f;
        while (timer < selectTimer)
        {
            timer += Time.deltaTime;
            currentStar.StarSelecting(timer / selectTimer);
            yield return null;
        }
        currentStar.StarSelecting(0);
        yield return null;
    }

    private void OnDisable()
    {
        foreach (WheelStar s in inReach) s.ScaleStarInfo(0);
        inReach.Clear();

        currentHold = null;

    }

}
