using Meta.XR.BuildingBlocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class SpatialAnchorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _saveableAnchorPrefab;
    public GameObject SaveableAnchorPrefab => _saveableAnchorPrefab;

    [SerializeField, FormerlySerializedAs("_saveablePreview")]
    private GameObject _saveablePreview;

    [SerializeField, FormerlySerializedAs("_saveableTransform")]
    private Transform _saveableTransform;

    [SerializeField]
    private GameObject _nonSaveableAnchorPrefab;
    public GameObject NonSaveableAnchorPrefab => _nonSaveableAnchorPrefab;

    [SerializeField, FormerlySerializedAs("_nonSaveablePreview")]
    private GameObject _nonSaveablePreview;

    [SerializeField, FormerlySerializedAs("_nonSaveableTransform")]
    private Transform _nonSaveableTransform;

    //
    private OVRSpatialAnchor _workingAnchor;            // general purpose anchor
    private List<OVRSpatialAnchor> _allSavedAnchors;    //anchors written these to local storage (green only)
    private List<OVRSpatialAnchor> _allRunningAnchors;  //anchors currently running (red and green)

    private int _anchorSavedUUIDListSize;                //current size of the tracking list
    private const int _anchorSavedUUIDListMaxSize = 50;  //max size of the tracking list
    private System.Guid[] _anchorSavedUUIDList;          //simulated external location, like PlayerPrefs

    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor; //delegate used for binding unbound anchors

    [SerializeField]
    SharedSpatialAnchorCore sharedSpatialAnchorCore;

    private void Awake()
    {
        //....
        //other statements
        _allSavedAnchors = new List<OVRSpatialAnchor>();
        _allRunningAnchors = new List<OVRSpatialAnchor>();
        _anchorSavedUUIDList = new System.Guid[_anchorSavedUUIDListMaxSize];
        _anchorSavedUUIDListSize = 0;
        _onLoadAnchor = OnLocalized;
        //....
        //other statements
    }

    private void Update()
    {
        //create
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) //create a green capsule
        {
            sharedSpatialAnchorCore.InstantiateSpatialAnchor(_saveableAnchorPrefab, _saveableTransform.position, _saveableTransform.rotation);
            //GameObject gs = PlaceAnchor(_saveableAnchorPrefab, _saveableTransform.position, _saveableTransform.rotation); //anchor A
            //_workingAnchor = gs.AddComponent<OVRSpatialAnchor>();
            //CreateAnchor(_workingAnchor, true); //true==save the anchor to local storage
        }
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))// create a red capsule
        {
            List<OVRSpaceUser> users = new List<OVRSpaceUser>();
            sharedSpatialAnchorCore.ShareSpatialAnchors(_allRunningAnchors, users);

            //GameObject gs = PlaceAnchor(_nonSaveableAnchorPrefab, _nonSaveableTransform.position, _nonSaveableTransform.rotation); //anchor b
            //_workingAnchor = gs.AddComponent<OVRSpatialAnchor>();
            //CreateAnchor(_workingAnchor, false);
        }

        //delete
        if (OVRInput.GetDown(OVRInput.Button.Three)) //x button
        {
            //Destroy all anchors from the scene, but don't erase them from storage
            using (var enumerator = _allRunningAnchors.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var spAnchor = enumerator.Current;
                    Destroy(spAnchor.gameObject);
                }
            }
            //clear the list of running anchors
            _allRunningAnchors.Clear();
        }

        //load
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            LoadAllAnchors(); // load saved anchors
        }

        // erase all saved (green) anchors
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            EraseAllAnchors();
        }
    }

    private GameObject PlaceAnchor(GameObject prefab, Vector3 p, Quaternion r)
    {
        return Instantiate(prefab, p, r);
    }

    public void CreateAnchor(OVRSpatialAnchor spAnchor, bool saveAnchor)
    {
        StartCoroutine(anchorCreated(_workingAnchor, saveAnchor));  //use a coroutine to manage the async save
    }

    public IEnumerator anchorCreated(OVRSpatialAnchor osAnchor, bool saveAnchor)
    {
        while (!osAnchor.Created && !osAnchor.Localized)
        {
            yield return new WaitForEndOfFrame(); //keep checking
        }

        //Save the anchor to a local List so we can refer to it
        _allRunningAnchors.Add(osAnchor);

        if (saveAnchor)  // we save the saveable (green) anchors only
        {
            osAnchor.Save((anchor, success) =>
            {
                if (success)
                {
                    //keep tabs on anchors in local storage
                    _allSavedAnchors.Add(anchor);
                    //if we wanted to save UUID to external storage so
                    // we could refer to it in a future session, we
                    // would do so here also
                }
            });
        }
    }

    public void LoadAllAnchors()
    {
        OVRSpatialAnchor.LoadOptions options = new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = GetSavedAnchorsUuids()
        };

        OVRSpatialAnchor.LoadUnboundAnchors(options, _anchorSavedUUIDList =>
        {
            if (_anchorSavedUUIDList == null)
            {
                return;
            }

            foreach (var anchor in _anchorSavedUUIDList)
            {
                if (anchor.Localized)
                {
                    _onLoadAnchor(anchor, true);
                }
                else if (!anchor.Localizing)
                {
                    anchor.Localize(_onLoadAnchor);
                }
            }
        });
    }

    private System.Guid[] GetSavedAnchorsUuids()
    {
        var uuids = new Guid[_allSavedAnchors.Count];
        using (var enumerator = _allSavedAnchors.GetEnumerator())
        {
            int i = 0;
            while (enumerator.MoveNext())
            {
                var currentUuid = enumerator.Current.Uuid;
                uuids[i] = new Guid(currentUuid.ToByteArray());
                i++;
            }
        }
        //Debug.Log("Returned All Anchor UUIDs!");
        return uuids;
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        var pose = unboundAnchor.Pose;
        GameObject go = PlaceAnchor(_saveableAnchorPrefab, pose.position, pose.rotation);
        _workingAnchor = go.AddComponent<OVRSpatialAnchor>();

        unboundAnchor.BindTo(_workingAnchor);

        // add the anchor to the running total
        _allRunningAnchors.Add(_workingAnchor);
    }

    public void EraseAllAnchors()
    {
        foreach (var tmpAnchor in _allSavedAnchors)
        {
            OVRSpatialAnchor spAnchor = tmpAnchor;
            if (spAnchor)
            {
                //use a Unity coroutine to manage the async save
                StartCoroutine(anchorErased(spAnchor));
            }
        }

        _allSavedAnchors.Clear();
        //if we were saving to PlayerPrefs, we would alse delete those here
        return;
    }

    public IEnumerator anchorErased(OVRSpatialAnchor osAnchor)
    {
        while (!osAnchor.Created)
        {
            yield return new WaitForEndOfFrame();
        }

        osAnchor.Erase((anchor, success) =>
        {
            if (!success)
            {
                Debug.Log("Anchor " + osAnchor.Uuid.ToString() + " NOT Erased!");
            }
            return;
        });
    }

}
