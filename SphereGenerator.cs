using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayTracingExercise
{
    public class SphereGenerator
    {
        string name;
        Vector3 sphereCenter;
        float sphereRadius;

        public SphereGenerator()
        {
            this.name = "SphereGenerator";
            this.sphereCenter = new Vector3(0f, 0f, -1f);
            this.sphereRadius = 0.5f;
        }

        /*
        The following would be used elsewhere to create the sphere:       
            SphereGenerator myRenderer = new SphereGenerator();
            renderedResult = myRenderer.GenSphere(canvasWidth, canvasHeight);       
        */
        public Texture2D GenSphere(int width, int height)
        {
            Texture2D sphereResult = new Texture2D(width, height); //width and height are width and height of canvas

            Vector3 origin = new Vector3(0, 0, 0);

            //define viewport -- learned how to do this from Peter Shirley's book Ray Tracing in One Weekend
            Vector3 lower_left_corner = new Vector3(-2.0f, -1.0f, -1.0f);
            Vector3 horizontal = new Vector3(4.0f, 0.0f, 0.0f);
            Vector3 vertical = new Vector3(0.0f, 2.0f, 0.0f);
            Vector3 ray_origin = new Vector3(1f,1f,-0.55f);

            for (int j = height-1; j>=0; j--)
            {
                for (int i = 0; i < width; i++)
                {
                    float u = (float)i / (float)width; //normalize ray to size of viewport
                    float v = (float)j / (float)height;
                    Vector3 direction = lower_left_corner+u*horizontal+v*vertical; //direction of ray

                    float t;
                    Vector3 intersectNormal;
                    bool doesIntersect = IntersectSphere(origin, ray_origin,direction, sphereCenter, sphereRadius, out t, out intersectNormal);
                    if (doesIntersect)
                    {
                        Color MyColor = getColor(t, intersectNormal, direction);
                        sphereResult.SetPixel(i, j, MyColor);
                    }

                }
            }
            sphereResult.Apply();
            return sphereResult;
        }

        private Color getColor(float t, Vector3 intersectNormal, Vector3 direction)
        {
            float lambertian = Vector3.Dot(direction.normalized, intersectNormal);

            var yellow = Color.yellow;
            Color lambertian_color = yellow * lambertian;
            Color lambertian_plus_ambient = lambertian_color + yellow * 0.5f;

            Vector3 v = new Vector3(0, 0, -1); //viewing direction     
            Vector3 h = direction.normalized + v.normalized;
            float specular = 500f*Mathf.Pow(Vector3.Dot(intersectNormal.normalized, h),5); //phong-blinn reflection model

            if (specular <= 0) 
            {
                return lambertian_plus_ambient;
            }

            Color with_specular = lambertian_plus_ambient + (Color.white * specular);
            return with_specular;
        }

        private bool IntersectSphere(Vector3 origin, Vector3 ray_origin, Vector3 direction, Vector3 sphereCenter, float sphereRadius, out float t, out Vector3 intersectNormal)
        {

                //ray-sphere intersection calculation
                Vector3 L = sphereCenter - origin;
                float tca = Vector3.Dot(L, direction.normalized);
                float d = Mathf.Sqrt(L.sqrMagnitude - Mathf.Pow(tca,2));
                float thc = Mathf.Sqrt(Mathf.Pow(sphereRadius,2) - Mathf.Pow(d,2));
                t = tca - thc; //distance along ray to hit point

                intersectNormal = L - (ray_origin + t*direction.normalized); //normal of the surface of the sphere at hit point

                if (tca < 0 || Math.Abs(d) > sphereRadius)
                {
                    return false; //outside of sphere, or intersects sphere behind the origin of the ray
                }
                return true; //ray intersects with the sphere
            }
    }
}
