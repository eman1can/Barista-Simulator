/*
 * Created by following video by Dapper Dino
 * https://www.youtube.com/watch?v=saM9D1V6uNg
 *
 * Modified extensively by Ethan Wolfe
 * Raycasting that looks for interactable items, as well as receiving events for interaction
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

namespace BaristaSimulator
{
    public class CameraRaycasting : MonoBehaviour
    {
        [SerializeField, Range(0.5f, 100)] private float range;

        private int layerMask = (1 << (int)Utils.Layers.Selection) | (1 << (int)Utils.Layers.Selected);
        private Camera _camera;
        private IInteractable _currentTarget = null;

        private bool blocked = false;

        private float recurrance = 0.125f;
        private float _duration = 0;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            RaycastInteractables();
        }

        private void RaycastInteractables()
        {
            RaycastHit hitInfo;
            Vector3 start = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            Debug.DrawRay(start, _camera.transform.forward, Color.red, 0.05f);

            if (Physics.Raycast(ray, out hitInfo, range, layerMask))
            {
                if (hitInfo.transform.TryGetComponent(out IInteractable interactable))
                {
                    if (_currentTarget != null && !_currentTarget.Equals(interactable))
                    {
                        _currentTarget = interactable;
                        if (interactable is IHoverableEnter hoverableEnter)
                        {
                            hoverableEnter.OnHoverEnter();
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (_currentTarget != null && _currentTarget is IHoverableExit hoverableExit)
                {
                    hoverableExit.OnHoverExit();
                    _currentTarget = null;
                }
            }
        }

        void OnGUI()
        {
            int size = 12;
            float posX = _camera.pixelWidth / 2 - size / 4;
            float posY = _camera.pixelHeight / 2 - size / 2;
            GUI.Label(new Rect(posX, posY, size, size), "*");
        }
    }
}