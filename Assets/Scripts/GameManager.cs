using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool startParking;

    void Start()
    {
        SetNumberForTags("Hangar");
        SetNumberForTags("Airplane");
    }

    void Update()
    {
        if (startParking)
        {
            ParkPlanes();
            startParking = false;
        }
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

    private void ParkPlanes()
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
        Vector3 StartParkingPosition = hangar.transform.position + new Vector3(0, 0, 2);
        AirplaneFunc planeFunc = plane.GetComponent<AirplaneFunc>();

        planeFunc.GoToPoint(StartParkingPosition);
        yield return new WaitUntil(() => Vector3.Distance(plane.transform.position, StartParkingPosition) <= planeFunc.stopDistance);
        planeFunc.GoToPoint(hangar.transform.position);
    }

    public void SwitchLights()
    {
        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("Airplane"))
        {
            AirplaneFunc func = plane.GetComponent<AirplaneFunc>();

            func.lights.SetActive(!func.lights.activeInHierarchy);
        }
    }
}
