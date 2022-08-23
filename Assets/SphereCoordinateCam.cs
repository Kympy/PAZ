using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCoordinateCam : MonoBehaviour
{
    private float rotationSpeed = 20f;
    private float scrollSpeed = 100f;
    private GameObject target;

    private float horizontal = 0f;
    private float vertical = 0f;
    private float MouseX = 0f;
    private float MouseY = 0f;
    private float RealX = 0f;
    private float RealY = 0f;

    private float scrollWheel = 0f;

    private bool HasMouseButton = false;

    private SphereCoordinate sphere;
    public class SphereCoordinate
    {
        private float radius;
        private float azimuth;
        private float elevation;

        private float MinRadius = 3f; // Distance with Player
        private float MaxRadius = 10f;

        private float MinAzimuth = 0f;
        private float minAzimuth;

        private float MaxAzimuth = 360f;
        private float maxAzimuth;

        private float MinElevation = 0f;
        private float minElevation;

        private float MaxElevation = 75f;
        private float maxElevation;
        public float Radius { get { return radius; } private set { radius = Mathf.Clamp(value, MinRadius, MaxRadius); } }
        public float Azimuth { get { return azimuth; } private set { azimuth = Mathf.Repeat(value, maxAzimuth - minAzimuth); } }
        public float Elevation { get { return elevation; } private set { elevation = Mathf.Clamp(value, minElevation, maxElevation); } }

        public SphereCoordinate() { }
        public SphereCoordinate(Vector3 Cartesian)
        {
            minAzimuth = Mathf.Deg2Rad * MinAzimuth;
            maxAzimuth = Mathf.Deg2Rad * MaxAzimuth;

            minElevation = Mathf.Deg2Rad * MinElevation;
            maxElevation = Mathf.Deg2Rad * MaxElevation;

            Radius = Cartesian.magnitude; // Camera - Player Distance
            Azimuth = Mathf.Atan2(Cartesian.z, Cartesian.x);
            Elevation = Mathf.Asin(Cartesian.y / Radius);
        }

        public Vector3 SphereToCartesian
        {
            get
            {
                float t = Radius * Mathf.Cos(Elevation);
                return new Vector3(t * Mathf.Cos(Azimuth), radius * Mathf.Sin(Elevation), t * Mathf.Sin(Azimuth));
            }
        }

        public SphereCoordinate Rotate(float newAzimuth, float newElevation)
        {
            Azimuth += newAzimuth;
            Elevation += newElevation;
            return this;
        }
        public SphereCoordinate TranslateRadius(float x)
        {
            Radius += x;
            return this;
        }
    }

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
        sphere = new SphereCoordinate(transform.position);
        transform.position = sphere.SphereToCartesian + target.transform.position;
    }
    private void Update()
    {
        //horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vertical");

        //HasMouseButton = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
        //MouseX = HasMouseButton ? Input.GetAxis("Mouse X") : 0f;
        //MouseY = HasMouseButton ? Input.GetAxis("Mouse Y") : 0f;

        //RealX = Mathf.Abs(horizontal) > Mathf.Abs(MouseX) ? horizontal : MouseX;
        //RealY = Mathf.Abs(vertical) > Mathf.Abs(MouseY) ? vertical : MouseY;
        RealX = -Input.GetAxis("Mouse X");
        RealY = -Input.GetAxis("Mouse Y");

        transform.position = sphere.Rotate(RealX * rotationSpeed * Time.deltaTime,
             RealY * rotationSpeed * Time.deltaTime).SphereToCartesian + target.transform.position;
        /*
        if (Mathf.Abs(RealX) > Mathf.Epsilon || Mathf.Abs(RealY) > Mathf.Epsilon)
        {
            transform.position = sphere.Rotate(RealX * rotationSpeed * Time.deltaTime,
                RealY * rotationSpeed * Time.deltaTime).SphereToCartesian + target.transform.position;
        }
        */

        scrollWheel = -Input.GetAxis("Mouse ScrollWheel");
        if(Mathf.Abs(scrollWheel) > Mathf.Epsilon)
        {
            transform.position = sphere.TranslateRadius(scrollWheel * scrollSpeed * Time.deltaTime).SphereToCartesian + target.transform.position;
        }

        transform.LookAt(target.transform);
    }
}
