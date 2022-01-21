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

    void Start()
    {
        SetPlaneAmount(planeAmount);
    }

    void Update()
    {
        CheckPlanesParked();
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
        Vector3 hangarPosition = new Vector3(-8.5f, 0, 4);

        for (int i = 0; i < planeAmount; i++)
        {
            Instantiate(hangarPrefab, hangarPosition, transform.rotation);
            Vector3 planePosition = hangarPosition - new Vector3(0, 0, 2);
            Instantiate(planePrefab, planePosition, transform.rotation);
            hangarPosition += new Vector3(2.5f, 0, 0);
        }

        SetNumberForTags("Hangar");
        SetNumberForTags("Airplane");

        startScreen.SetActive(false);
    }

    private void CheckPlanesParked()
    {
        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Airplane"))
        {
            if (!plane.GetComponent<AirplaneFunc>().parked) break;
            parkedText.SetActive(true);
        }
    }

    public void ParkPlanes()
    {
        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Airplane"))
        {
            foreach (GameObject hangar in GameObject.FindGameObjectsWithTag("Hangar"))
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

        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Airplane"))
        {
            AirplaneFunc func = plane.GetComponent<AirplaneFunc>();
            func.lights.SetActive(!func.lights.activeInHierarchy);
        }
    }
}
