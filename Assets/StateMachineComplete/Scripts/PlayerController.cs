﻿using UnityEngine;
using System.Collections;

namespace StateMachineSample
{
    public class PlayerController : MonoBehaviour
    {
        public Transform turret;
        public Transform muzzle;
        public GameObject bulletPrefab;

        private float force = 100f;
        private float maxSpeed = 30f;
        private float maxAngularVelocity = 360f;
        private float attackInterval = 0.8f;

        private int groundLayerMask;
        private float lastAttackTime;

        private void Start()
        {
            groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
        }

        private void Update()
        {
            UpdateTank();
            UpdateTurret();
        }

        private void UpdateTank()
        {
            Vector3 velocity = GetComponent<Rigidbody>().velocity;

            if (velocity.sqrMagnitude < 1f)
            {
                return;
            }

            Vector3 direction = new Vector3(velocity.x, 0f, velocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularVelocity * Time.deltaTime);
        }

        private void UpdateTurret()
        {
            // マウス位置を狙う
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                turret.rotation = Quaternion.LookRotation(hit.point - turret.position);
            }

            // マウス左クリックで発射する
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time > lastAttackTime + attackInterval)
                {
                    Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
                    lastAttackTime = Time.time;
                }
            }
        }

        private void FixedUpdate()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(x, 0f, z).normalized;

            GetComponent<Rigidbody>().AddForce(direction * force);
            GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxSpeed);
        }
    }
}
