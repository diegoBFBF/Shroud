
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WheelStar : MonoBehaviour
{
    StarVisualController visualController;
    StarTextController textController;

    [SerializeField] float maxEnabledDistance = 0.2f;
    [SerializeField] float maxLockedDistance = 0.2f;
    [SerializeField] float scaleSpeed = 1;

    float circleRadius = 15f;


    [System.Serializable]
    public class WheelOption
    {
        public string title;
        public UnityEvent onSelect;
    }

    [SerializeField] List<WheelOption> wheelOptions = new List<WheelOption>();

    [SerializeField] WheelOption selectedOption;

    [SerializeField] GameObject optionPrefab;

    [SerializeField] StarState starState;

    [SerializeField] Transform optionContainer;

    public Text selectedOptionTitle;

    [SerializeField]
    List<GameObject> optionObjects = new List<GameObject>();

    public enum StarState { Enabled, Disabled, Locked }

    private void Awake()
    {
        visualController = GetComponent<StarVisualController>();
        textController = GetComponent<StarTextController>();

        optionContainer.localScale = Vector3.zero;
        SetStarState(StarState.Locked);

        //textController.SetTitleText("");

        ChangeOptionTitle("");
    }

    void Start()
    {
        if(wheelOptions.Count> 0)
        {
            CreateOptionsCircle(new List<WheelOption>(wheelOptions));
        }
    }

    public void CreateOptionsCircle(List<WheelOption> newOptions)
    {
        optionContainer.localScale = Vector3.zero;
        if (optionObjects.Count > 0)
        {
            DestroyAllOptions();
        }
        wheelOptions.Clear();
        selectedOption = null;
        ChangeOptionTitle("");

        if(newOptions == null)
        {
            SetStarState(StarState.Locked);
            return;
        }

        if (newOptions.Count <= 0)
        {
            SetStarState(StarState.Locked);
            return;
        }
        else if (newOptions.Count == 1)
        {
            wheelOptions = newOptions;
            selectedOption = newOptions[0];
            ChangeOptionTitle(selectedOption.title);
        }
        else
        {
            wheelOptions = newOptions;

            float optionAngle = 360f / wheelOptions.Count;

            for (int i = 0; i < wheelOptions.Count; i++)
            {
                float angle = i * optionAngle;
                angle = angle + (optionAngle / 2);
                Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * circleRadius, Mathf.Sin(Mathf.Deg2Rad * angle) * circleRadius, 0f);

                GameObject newOptionObject = Instantiate(optionPrefab, optionContainer);
                newOptionObject.transform.localPosition = position;
                newOptionObject.transform.localRotation = Quaternion.identity;
                newOptionObject.GetComponentInChildren<Text>().text = wheelOptions[i].title;

                optionObjects.Add(newOptionObject);

                // Set other Text properties as needed (font, size, color, etc.)
            }
        }

        SetStarState(StarState.Enabled);
    }

    void DestroyAllOptions()
    {
        foreach(GameObject option in optionObjects)
        {
            Destroy(option);
        }
        optionObjects.Clear();
    }

    public void SetStarState(StarState newState)
    {
        if(errorRoutine != null)
        {
            StopCoroutine(errorRoutine);
            errorRoutine = null;
            textController.SetTitleText("");
        }

        starState = newState;
    }

    public void StarHeld(Vector3 handPosition)
    {
        //check for animation routine
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        //move star to closest position to handPosition without going farther than maxStarDistance
        float maxStarDistance;
        switch (starState)
        {
            case StarState.Locked: maxStarDistance = maxLockedDistance; break;

            default: maxStarDistance = maxEnabledDistance; break;
        }

        Vector3 localHandPosition = transform.parent.InverseTransformPoint(handPosition);
        if (localHandPosition.magnitude > maxStarDistance)
        {
            transform.localPosition = localHandPosition.normalized * maxStarDistance;
        }
        else
        {
            transform.localPosition = localHandPosition;
        }

        //scale Wheel
        ScaleWheel();

        //determine key
        if (wheelOptions.Count <= 0)
        {
            
        }
        else if(wheelOptions.Count == 1)
        {

        }
        else
        {
            // Calculate angle in radians
            float angleRadians = Mathf.Atan2(localHandPosition.y, localHandPosition.x);

            // Convert radians to degrees
            float angleDegrees = Mathf.Rad2Deg * angleRadians;

            // Ensure angle is within [0, 360) degrees
            angleDegrees = (angleDegrees + 360) % 360;

            float index = angleDegrees / (360f / wheelOptions.Count);

            // Map angle to options
            int indexRounded = Mathf.FloorToInt(index);

            // Determine the key/letter
            selectedOption = wheelOptions[indexRounded];

            ScaleSelectedOption(indexRounded, index);

            ChangeOptionTitle(selectedOption.title);
        }
    }

    public void StarSelected()
    {
        if (starState != StarState.Enabled || selectedOption == null)
        {
            return;
        }
        try
        {
            selectedOption.onSelect.Invoke();
            Debug.Log($"{selectedOption.title} selected");
        }
        catch { }
    }

    public void StarReleased()
    {
        //if(wheelOptions.Count > 1) selectedOption = null;

        if(animationRoutine != null) StopCoroutine(animationRoutine);
        StartCoroutine(animationRoutine = ResetAnimation());
    }

    IEnumerator animationRoutine;
    IEnumerator ResetAnimation()
    {
        float initialWheelScale = optionContainer.transform.localScale.x;
        Vector3 initialPosition = transform.localPosition;
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime * (1 / 0.2f);

            //move star
            transform.localPosition = Vector3.Lerp(Vector3.zero, initialPosition, timer);
            //scale wheel
            float wheelScale = Mathf.Lerp(0, initialWheelScale, timer);
            optionContainer.transform.localScale = Vector3.one * wheelScale;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        optionContainer.transform.localScale = Vector3.zero;
        if (wheelOptions.Count > 1)
        {
            ChangeOptionTitle("");
        }
        animationRoutine = null;
        yield return null;
    }

    void ScaleWheel()
    {
        float newScale = ((transform.localPosition.magnitude * 2.4f) / maxEnabledDistance);
        newScale = Mathf.Clamp(newScale, 0, 1f);

        switch (starState)
        {
            case StarState.Enabled:
                newScale = Mathf.Lerp(optionContainer.transform.localScale.x, newScale, Time.deltaTime * scaleSpeed);
                optionContainer.transform.localScale = Vector3.one * newScale;
                break;

            case StarState.Disabled:
            case StarState.Locked:
                float newOptionScale = Mathf.Lerp(optionContainer.transform.localScale.x, 0, Time.deltaTime * scaleSpeed);
                optionContainer.transform.localScale = Vector3.one * newOptionScale;
                break;
        }
    }

   
    public void ScaleSelectedOption(int indexRounded, float index)
    {
        float scaleFactor = Mathf.Abs(indexRounded - (index - 0.5f)) + 0.5f;
        scaleFactor = 1 - scaleFactor;

        foreach (GameObject obj in optionObjects)
        {
            //Text textComponent = letterObject.GetComponentInChildren<Text>();
            if (optionObjects[indexRounded] == obj)
            {
                // Scale up the selected letter
                obj.transform.localScale = Vector3.one + ((Vector3.one) * scaleFactor);
            }
            else
            {
                // Reset the scale for other letters
                obj.transform.localScale = Vector3.one;
            }
        }

    }
    public void ScaleStarInfo(float distance)
    {
        textController.ScaleText(distance);
    }

    public void StarSelecting(float selectPercent)
    {
        visualController.ScaleGlow(selectPercent);
    }

    public void ChangeOptionTitle(string title)
    {
        selectedOptionTitle.text = title;
    }

    public void StartErrorRoutine(string message)
    {
        if(errorRoutine != null)
        {
            StopCoroutine(errorRoutine);
            errorRoutine = null;
            textController.SetTitleText("");
        }

        SetStarState(StarState.Disabled);
        StartCoroutine(errorRoutine = ErrorRoutine(message));
    }

    IEnumerator errorRoutine;
    IEnumerator ErrorRoutine(string message)
    {
        textController.SetTitleText(message);
        yield return new WaitForSeconds(2);
        textController.SetTitleText("");
        SetStarState(StarState.Enabled); //disableRoutine = null in SetStarState
    }


}
