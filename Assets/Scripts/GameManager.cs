using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startButton, startScreen, directionalLight;

    public GameObject hangarPrefab, planePrefab;

    public GameObject planeAmountText, parkedText;

    public int planeAmount = 3;

    private List<GameObject> planes = new List<GameObject>();
    private List<GameObject> hangars = new List<GameObject>();

    public List<Airplane> planeTypes = new List<Airplane>();

    private bool gameStarted;

    void Start()
    {
        SetPlaneAmount(planeAmount);
    }

    void Update()
    {
        if (gameStarted && CheckPlanesParked()) parkedText.SetActive(true);
    }

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

    public void SetPlaneAmount(float amount)
    {
        planeAmount = (int)amount;
        planeAmountText.GetComponent<TextMeshProUGUI>().text = planeAmount.ToString();
    }


    public void StartGame()
    {
        Vector3 hangarPosition = new Vector3(-8.75f, 0, 4);

        for (int i = 0; i < planeAmount; i++)
        {
            GameObject hangar = Instantiate(hangarPrefab, hangarPosition, transform.rotation);
            Vector3 planePosition = hangarPosition - new Vector3(0, 0, 2);
            GameObject plane = Instantiate(planePrefab, planePosition, transform.rotation);
            plane.GetComponent<AirplaneFunc>().planeType = planeTypes[Random.Range(0, planeTypes.Count)];
            hangarPosition += new Vector3(2.5f, 0, 0);

            hangars.Add(hangar);
            planes.Add(plane);
        }

        SetNumberForTags("Hangar");
        SetNumberForTags("Airplane");

        startScreen.SetActive(false);
        gameStarted = true;
    }

    private bool CheckPlanesParked()
    {
        foreach (GameObject plane in planes)
        {
            if (!plane.GetComponent<AirplaneFunc>().parked) return false;
        }
        return true;
    }

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
