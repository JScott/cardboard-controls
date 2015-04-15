using UnityEngine;
using System.Collections;

public class RotatingCube : MonoBehaviour {
  public float xRotationSpeed = 0.0f;
  public float yRotationSpeed = 0.0f;
  public float zRotationSpeed = 0.0f;
	void Update () {
    transform.Rotate(Vector3.right * (Time.deltaTime * xRotationSpeed));
    transform.Rotate(Vector3.up * (Time.deltaTime * yRotationSpeed));
    transform.Rotate(Vector3.forward * (Time.deltaTime * zRotationSpeed));
	}
}
