using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Camera mCamera;

    private CameraInput cameraInput;

    public float cameraSpeed;


    private void Awake()
    {
        mCamera = gameObject.GetComponent<Camera>();
        cameraInput = new CameraInput();

        cameraInput.cameraControll.Enable();
        cameraInput.cameraControll.Forword.performed += MoveForword;
        cameraInput.cameraControll.Back.performed += MoveBack;
        cameraInput.cameraControll.Left.performed += MoveLeft;
        cameraInput.cameraControll.Right.performed += MoveRight;

    }

    private void Update()
    {
    }


    private void MoveForword(InputAction.CallbackContext ctx)
    {
         transform.position += transform.forward * Time.deltaTime * cameraSpeed;
    }
    private void MoveBack(InputAction.CallbackContext ctx)
    {
        //transform.position += transform. * Time.deltaTime * cameraSpeed;
    }
    private void MoveLeft(InputAction.CallbackContext ctx)
    {

    }
    private void MoveRight(InputAction.CallbackContext ctx)
    {

    }
}
