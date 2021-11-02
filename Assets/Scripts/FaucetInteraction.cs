using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Water faucet interaction
 */
public class FaucetInteraction : IInteractable {
    [Header("Handle Variables")] [SerializeField]
    private Transform handlePivot;

    [SerializeField, Range(0, 45)] private float handleRotation;
    [SerializeField, Range(1, 5)] private float duration;
    [SerializeField] private GameObject StreamPrefab;

    [Header("Drain Variables")] [SerializeField]
    private GameObject drainTray;

    [SerializeField, Range(1, 1000)] private float maxFillAmount;

    [Header("Water Variables")] [SerializeField]
    private Transform waterRoot;

    [SerializeField, Range(.0f, 1f)] private float shutOffPercentage;
    [SerializeField, Range(0.5f, 10)] private float shutOffTimeout;

    private LiquidDispenser _dispenser;
    private bool _handleAnimating = false;
    private float _handleProgress;
    private bool _direction = false;
    private Stream _currentStream = null;
    private LiquidContainer _currentContainer = null;
    private void Awake() {
        _dispenser = GetComponent<LiquidDispenser>();
    }
    public void OnStartHover() {
        if (_handleAnimating)
            return;
        base.OnStartHover();
    }

    public override bool OnInteract() {
        if (_handleAnimating)
            return false;
        AnimateHandle();
        if (_direction) {
            StartStream();
            infoStrings[0] = "Turn Off Faucet";
        } else {
            infoStrings[0] = "Turn On Faucet";
        }
        interactionGUI.UpdateStrings(infoStrings, infoCount);

        return false;
    }

    private void AnimateHandle() {
        _handleAnimating = true;
        _handleProgress = 0f;
        _direction = !_direction;
    }

    private void Update() {
        if (_handleAnimating) {
            if (_direction)
                handlePivot.transform.localRotation = Quaternion.Euler(Mathf.Lerp(0, handleRotation,duration / _handleProgress), 0, 0);
            else
                handlePivot.transform.localRotation = Quaternion.Euler(Mathf.Lerp(handleRotation, 0,duration / _handleProgress), 0, 0);
            _handleProgress += Time.deltaTime;
            if (_handleProgress > duration) {
                _handleAnimating = false;
                if (!_direction)
                    EndStream();
            }
        } else if (_direction && _dispenser.CheckDispense(shutOffTimeout)) {
            AnimateHandle();
        }
        
        if (_currentStream != null && _currentStream.Active()) {
            _dispenser.Fill(_currentContainer);
            if (!_handleAnimating && _currentContainer.FillPercent() > shutOffPercentage) {
                AnimateHandle();
            }
        }
    }

    private void StartStream() {
        _currentStream = CreateStream();
        _currentContainer = _currentStream.Begin();
        _currentContainer.SetLiquid(_dispenser.GetLiquid());
    }

    private void EndStream() {
        _currentStream.End();
        _currentStream = null;
    }

    private Stream CreateStream() {
        var streamObject = Instantiate(StreamPrefab, waterRoot.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}