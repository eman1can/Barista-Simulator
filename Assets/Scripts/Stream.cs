using System;
using System.Collections;
using UnityEngine;
/*
 * Created by following tutorial series by VR with Andrew
 * https://www.youtube.com/watch?v=LXmwlptZtRY
 */
public class Stream : MonoBehaviour {
    private LineRenderer _lineRenderer = null;
    private Vector3 _targetPosition = Vector3.zero;
    private ParticleSystem splashParticle = null;

    private int layerMask = 1 << (int) Utils.Layers.LiquidCollision;
    private Coroutine streamCoroutine;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.025f;
        _lineRenderer.endWidth = 0.05f;
        splashParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start() {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public LiquidContainer Begin() {
        StartCoroutine(UpdateParticle());
        streamCoroutine = StartCoroutine(BeginPour());
        return GetContainer();
    }

    public LiquidContainer GetContainer() {
        RaycastHit hit;
        var ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, 2.0f, layerMask);
        
        if (hit.collider) {
            var containerReference = hit.collider.GetComponent<LiquidContainerReference>();
            if (containerReference != null) {
                var container = containerReference.GetLiquidContianer();
                return container;
            }
        }

        return null;
    }

    public void End() {
        StopCoroutine(streamCoroutine);
        streamCoroutine = StartCoroutine(EndPour());
    }
    private IEnumerator BeginPour() {
        while (gameObject.activeSelf) {
            _targetPosition = FindEndPoint();
            
            MoveToPosition(0, transform.position);
            AnimateToPosition(1, _targetPosition);
            
            yield return null;    
        }
    }

    private IEnumerator EndPour() {
        while (!HasReachedPosition(0, _targetPosition)) {
            AnimateToPosition(0, _targetPosition);
            AnimateToPosition(1, _targetPosition);
            yield return null;
        } 
        Destroy(gameObject);
    }

    public bool Active() {
        return HasReachedPosition(1, _targetPosition);
    }

    private Vector3 FindEndPoint() {
        RaycastHit hit;
        var ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, 2.0f, layerMask);

        return hit.collider ? hit.point : ray.GetPoint(2.0f);
    }

    private void MoveToPosition(int index, Vector3 targetPosition) {
        _lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition) {
        Vector3 currentPoint = _lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);
        _lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachedPosition(int index, Vector3 targetPosition) {
        Vector3 currentPosition = _lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    private IEnumerator UpdateParticle() {
        while (gameObject.activeSelf) {
            splashParticle.gameObject.transform.position = _targetPosition;
            
            bool isHitting = HasReachedPosition(1, _targetPosition);
            splashParticle.gameObject.SetActive(isHitting);
            
            yield return null;
        }
    }
}