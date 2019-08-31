using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyController : MonoBehaviour
{

    public Camera mainCamera;
    public Transform propeler;
    public AudioSource audioSource;

    public static bool boostButtonClicked;

    private float[] boostSpeedMinMax = new float[] { 1f,3f };
    private float[] cameraFOVMinMax = new float[] { 60f, 90f };
    private float[] audioPitchMinMax = new float[] { 1f, 3f };

    public float boostedSpeed = 1;

    public float speed = 40f;

    public static float horizontal, vertical;

    private void Start()
    {
        boostedSpeed = boostSpeedMinMax[0];
    }
    private float ZeroToOneRange()
    {
        return (boostedSpeed - boostSpeedMinMax[0]) / (boostSpeedMinMax[1] - boostSpeedMinMax[0]);
    }
    void FixedUpdate()
    {

        if (boostButtonClicked)
        {
            boostedSpeed = Mathf.Lerp(boostedSpeed, boostSpeedMinMax[1], Time.deltaTime * 0.7f);
        }
        else
        {
            boostedSpeed = Mathf.Lerp(boostedSpeed, boostSpeedMinMax[0], Time.deltaTime * 2f);
        }

        mainCamera.fieldOfView = cameraFOVMinMax[0] + ((cameraFOVMinMax[1] - cameraFOVMinMax[0]) * ZeroToOneRange());
        audioSource.pitch = audioPitchMinMax[0] + ((audioPitchMinMax[1] - audioPitchMinMax[0]) * ZeroToOneRange());


        this.propeler.Rotate(this.propeler.localPosition.x, this.propeler.localPosition.y, -this.propeler.localPosition.z * Time.deltaTime * 300f);
        Vector3 moveCamTo = this.transform.position - transform.forward * 21f + Vector3.up * 5f;

        float bias = 0.96f; // 96%

        mainCamera.transform.position = moveCamTo;// * (1.0f - bias);//Vector3.Lerp(mainCamera.transform.position, mainCamera.transform.position * bias + moveCamTo * (1.0f - bias), 0.2f); 


        mainCamera.transform.LookAt(this.transform.position);//+ this.transform.forward * 10f);

        this.transform.position += transform.forward * Time.deltaTime * speed * boostedSpeed;


        speed -= this.transform.forward.y * Time.deltaTime * 20f;

        if (speed < 35)
        {
            speed = 35;
        }

        KeyboardInput();

        //Debug.Log("Vertical " + Input.GetAxis("Vertical") + ": Horizontal "  + -Input.GetAxis("Horizontal"));
        this.transform.Rotate(vertical, 0.0f, -horizontal);

        float pushFromTerrain = Terrain.activeTerrain.SampleHeight(this.transform.position);

        if (pushFromTerrain > this.transform.position.y - 5)
        {
            Vector3 newPos = new Vector3(this.transform.position.x, pushFromTerrain + 10, this.transform.position.z);
            this.transform.position = newPos;
        }

    }
    IEnumerator AnimateBoost(float origin, float target, float duration)
    {
        float journey = 0f;

        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            boostedSpeed = Mathf.Lerp(origin, target, percent);

            yield return null;
        }
    }

    private void KeyboardInput()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
        }

    }
}
