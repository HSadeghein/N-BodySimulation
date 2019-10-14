using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderRunner : MonoBehaviour
{
    public int particleCount = 1000;
    public Material material;
    public ComputeShader computeShader;

    private const int SIZE_OF_PARTICLE = 24;
    private int computeShaderKernelID;
    private const int NUMBER_OF_PARTICLE_IN_EACH_THREAD = 256;
    private ComputeBuffer computeBuffer;
    private int threadNumber;

    private AudioSource audioData;
    private struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
    }
    // Use this for initialization
    void Start()
    {
        Inititalize();

    }
    void OnDestroy()
    {
        if (computeBuffer != null)
            computeBuffer.Release();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Inititalize();
        }
        computeShader.SetFloat("_deltaTime", Time.deltaTime);

        computeShader.Dispatch(computeShaderKernelID, threadNumber, 1, 1);
    }

    private void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, particleCount);

    }
    
    private void Inititalize()
    {
        RenderSettings.ambientLight = Color.black;
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
        if (particleCount <= 0)
            particleCount = 1;
        threadNumber = Mathf.CeilToInt((float)particleCount / NUMBER_OF_PARTICLE_IN_EACH_THREAD);

        Particle[] particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            if (i % 4 == 0)
            {
                particles[i].position.x = Random.value * 20 - 4.0f;
                particles[i].position.y = Random.value * 20 - 4.0f;
                particles[i].position.z = Random.value * 10 - 4.0f;
                particles[i].velocity.x = 0;
                particles[i].velocity.y = 0;
                particles[i].velocity.z = 0;

            }
            else if (i % 4 == 1)
            {
                particles[i].position.x = Random.value * 45 + 100.0f;
                particles[i].position.y = Random.value * 4 + 100.0f;
                particles[i].position.z = Random.value * 10 + 10.0f;
                particles[i].velocity.x = 0;
                particles[i].velocity.y = 0;
                particles[i].velocity.z = 0;

            }
            else if (i % 4 == 2)
            {
                particles[i].position.x = Random.value * 40 - 250.0f;
                particles[i].position.y = Random.value * 40 - 4.0f;
                particles[i].position.z = Random.value * 10 - 4.0f;
                particles[i].velocity.x = 0;
                particles[i].velocity.y = 0;
                particles[i].velocity.z = 0;
            }
            else
            {
                particles[i].position.x = Random.value * 40 + -200.0f;
                particles[i].position.y = Random.value * 40 + 250.0f;
                particles[i].position.z = Random.value * 10 + 10.0f;
                particles[i].velocity.x = 0;
                particles[i].velocity.y = 0;
                particles[i].velocity.z = 0;
            }


        }

        computeBuffer = new ComputeBuffer(particleCount, SIZE_OF_PARTICLE);
        computeBuffer.SetData(particles);

        computeShaderKernelID = computeShader.FindKernel("CSMain");

        computeShader.SetBuffer(computeShaderKernelID, "particleBuffer", computeBuffer);
        material.SetBuffer("particleBuffer", computeBuffer);
        computeShader.SetInt("_numofParticles", particleCount);
    }

}
