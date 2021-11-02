using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
/*
 * Created by Minions Art
 * https://www.patreon.com/posts/quick-game-art-18245226
 */
public class Wobble : MonoBehaviour {
    
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;


    private Vector3 objectSize;
    private Vector3 worldPos;

    private Renderer _renderer;

    private Mesh _mesh;
    void Start() {
        _renderer = GetComponent<Renderer>();
        _mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Update() {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        _renderer.material.SetFloat("_WobbleX", wobbleAmountX);
        _renderer.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
    
    Vector4 RotateAroundYInDegrees(Vector3 vertex, float degrees) {
        float alpha = degrees * Mathf.PI / 180;
        float sina = Mathf.Sin(alpha);
        float cosa = Mathf.Cos(alpha);

        float a = cosa * vertex.x + -sina * vertex.z;
        float b = sina * vertex.x + cosa * vertex.z;
        return new Vector4(vertex.y, vertex.z, a, b);
    }
}