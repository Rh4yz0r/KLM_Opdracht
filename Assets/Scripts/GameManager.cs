using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Prefabs")]                                             //Prefab GameObjects
    public GameObject hangarPrefab;
    public List<GameObject> planePrefabs = new List<GameObject>();
    //public GameObject planePrefab;

    [Header("Scene Components")]                                    //Scene Component GameObjects
    public GameObject directionalLight;

    [Header("User Interface Components")]                           //User Interface GameObjects
    public GameObject startButton;
    public GameObject startScreen;
    public GameObject planeAmountText;
    public GameObject parkedText;

    [Header("Parent Components")]                                   //Instantiated GameObject Parents
    public Transform hangarParent;
    public Transform planeParent;

    [Header("Airplane Scriptable Objects")]                         //List with Airplane ScriptableObjects
    public List<Airplane> planeTypes = new List<Airplane>();

    private List<GameObject> planes = new List<GameObject>();
    private List<GameObject> hangars = new List<GameObject>();

    private int planeAmount = 3;

    private bool gameStarted;
    #endregion

    void Start()
    {
        SetPlaneAmount(planeAmount);
    }

    void Update()
    {
        if (gameStarted && CheckPlanesParked()) parkedText.SetActive(true);
    }

    /// <summary>
    /// Add numbers to every GameObject with given tag.
    /// </summary>
    /// <param name="tag"></param>
    private void SetNumberForTags(string tag)
    {
        int number = 1;

        foreach (GameObject child in GameObject.FindGameObjectsWithTag(tag))
        {
            TextMeshPro TMP = child.GetComponentInChildren<TextMeshPro>();
            TMP.text = number.ToString();
            number++;
        }
    }

    /// <summary>
    /// Set amount of planes to be instantiated.
    /// </summary>
    /// <param name="amount"></param>
    public void SetPlaneAmount(float amount)
    {
        planeAmount = (int)amount;
        planeAmountText.GetComponent<TextMeshProUGUI>().text = planeAmount.ToString();
    }

    /// <summary>
    /// Instantiate and set tags for planes and hangars, remove start screen menu and start the game.
    /// </summary>
    public void StartGame()
    {
        Vector3 hangarPosition = new Vector3(-8.75f, 0, 4);

        for (int i = 0; i < planeAmount; i++)
        {
            GameObject hangar = Instantiate(hangarPrefab, hangarPosition, transform.rotation, hangarParent);
            Vector3 planePosition = hangarPosition - new Vector3(0, 0, 2);
            InstantiateRandomPlane(planePosition);
            hangarPosition += new Vector3(2.5f, 0, 0);

            hangars.Add(hangar);
        }

        SetNumberForTags("Hangar");
        SetNumberForTags("Airplane");

        startScreen.SetActive(false);
        gameStarted = true;
    }

    private void InstantiateRandomPlane(Vector3 position)
    {
        GameObject planeType = planePrefabs[Random.Range(0, planePrefabs.Count)];

        GameObject plane = Instantiate(planeType, position, transform.rotation, planeParent);

        AirplaneFunc planeFunc = plane.GetComponent<AirplaneFunc>();
        
        if (planeType == planePrefabs[0]) planeFunc.planeType = planeTypes[Random.Range(0, planeTypes.Count - 1)];
        else if (planeType == planePrefabs[1]) planeFunc.planeType = planeTypes[2];

        planes.Add(plane);
    }

    /// <summary>
    /// Check if all the planes have parked.
    /// </summary>
    /// <returns></returns>
    private bool CheckPlanesParked()
    {
        foreach (GameObject plane in planes)
        {
            if (!plane.GetComponent<AirplaneFunc>().parked) return false;
        }
        return true;
    }

    /// <summary>
    /// Start parking Coroutine for every plane.
    /// </summary>
    public void ParkPlanes()
    {
        foreach (GameObject plane in planes)
        {
            foreach (GameObject hangar in hangars)
            {
                TextMeshPro TMPPlane = plane.GetComponentInChildren<TextMeshPro>();
                TextMeshPro TMPHangar = hangar.GetComponentInChildren<TextMeshPro>();

                if (TMPPlane.text == TMPHangar.text) StartCoroutine(StartParking(plane, hangar));
            }
        }
    }

    /// <summary>
    /// Initiate parking for given plane to given hangar.
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="hangar"></param>
    /// <returns></returns>
    private IEnumerator StartParking(GameObject plane, GameObject hangar)
    {
        Vector3 StartParkingPosition = hangar.transform.position - new Vector3(0, 0, 2);
        AirplaneFunc planeFunc = plane.GetComponent<AirplaneFunc>();

        planeFunc.GoToPoint(StartParkingPosition);
        yield return new WaitUntil(() => Vector3.Distance(plane.transform.position, StartParkingPosition) <= planeFunc.stopDistance);
        planeFunc.GoToPoint(hangar.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(plane.transform.position, hangar.transform.position) <= planeFunc.stopDistance);
        planeFunc.parked = true;
    }

    /// <summary>
    /// Switch lights on and off for every plane.
    /// </summary>
    public void SwitchLights()
    {
        directionalLight.SetActive(!directionalLight.activeInHierarchy);

        foreach (GameObject plane in planes)
        {
            AirplaneFunc func = plane.GetComponent<AirplaneFunc>();
            func.lights.SetActive(!func.lights.activeInHierarchy);
        }
    }
}
