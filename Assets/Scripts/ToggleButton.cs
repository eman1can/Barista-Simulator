using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Vector3 _pressedOffsetPosition = Vector3.zero;
    [SerializeField] private float _toggleButtonTimeLength = 1;
    [SerializeField] private UnityEvent _onPress = null;
    [SerializeField] private UnityEvent _offPress = null;
    private bool _isPressed = false;
    private bool _isExecutingPress = false;

    public void OnInteract()
    {
        if (!_isExecutingPress)
        {
            StartCoroutine(nameof(TogglePress));
        }
    }

    private IEnumerator TogglePress()
    {
        if (!_isExecutingPress)
        {
            _isExecutingPress = true;
            float initialTimeStamp = Time.time;
            float elapsedTime = 0;
            Vector3 startingPosition = transform.position;
            if (_isPressed)
            {
                while (elapsedTime < _toggleButtonTimeLength)
                {
                    yield return new WaitForSeconds(0.025f);
                    elapsedTime = Time.time - initialTimeStamp;
                    float pressPercentage = elapsedTime / _toggleButtonTimeLength;
                    Vector3 offset = -_pressedOffsetPosition * pressPercentage;
                    transform.position = startingPosition + offset;
                }
                _offPress?.Invoke();
            }
            else
            {
                while (elapsedTime < _toggleButtonTimeLength)
                {
                    yield return new WaitForSeconds(0.025f);
                    elapsedTime = Time.time - initialTimeStamp;
                    float pressPercentage = elapsedTime / _toggleButtonTimeLength;
                    Vector3 offset = _pressedOffsetPosition * pressPercentage;
                    transform.position = startingPosition + offset;
                }
                _onPress?.Invoke();
            }
            _isExecutingPress = false;
        }
    }
}
