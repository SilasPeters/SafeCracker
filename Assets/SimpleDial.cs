using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class SimpleDial : MonoBehaviour
{
    public static AllowedInput AllowedInput = AllowedInput.Left | AllowedInput.Right;
    public bool playConstructorStuff;

    int currentNumber = 0;
    [SerializeField] int maxNumber = 10;
    [SerializeField] float startPitch = 1;

    [SerializeField] int steps;
    int step = 0;
    int highestStep = 0; // What is the furthest step the player has gotten to?
    int[] code;

    [SerializeField] AudioClip click;
    [SerializeField] AudioClip altClick;
    [SerializeField] AudioClip failedClick;
    [SerializeField] AudioSource clickSource;

    [SerializeField] UnityEvent winEvent;
    public UnityEvent[] stepEvent;

    private void Start()
    {
        code = GenerateCode(steps);
        stepEvent[0].Invoke();
    }

    void Update()
    {
        if (AllowedInput == 0) return;

        if ((AllowedInput & AllowedInput.Right) != 0 && Input.GetKeyDown(KeyCode.RightArrow))
            ChangeNumber(1);
        else if ((AllowedInput & AllowedInput.Left) != 0 & Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeNumber(-1);
    }

    private void ChangeNumber(int delta)
    {
        if ((delta > 0 && isEven(step)) || (delta < 0 && !isEven(step) && step > 0))
        {
            step = 0;
            clickSource.PlayOneShot(failedClick);
            clickSource.pitch = startPitch;
            Debug.Log("Moved down to step " + step);
        }

        currentNumber += delta;
        if (currentNumber >= maxNumber)
            currentNumber = 0;
        else if (currentNumber < 0)
            currentNumber = maxNumber - 1;

        Debug.Log("Current digit: " + currentNumber);
        Debug.Log(step);

        if (currentNumber == code[step])
        {
            clickSource.pitch = 1;
            clickSource.PlayOneShot(altClick);
            step++;
            Debug.Log("Moved up to step " + step);
            if (step >= code.Length)
            {
                Debug.Log("The safe is open!");
                winEvent.Invoke();
                StopAllCoroutines();
                Controller.c.constructionSource.Stop();
                
            }
        }
        else
        {
            clickSource.pitch = startPitch + step * 0.1f;
            clickSource.PlayOneShot(click);
        }

        if (step > highestStep)
        {
            highestStep = step;
            //if (playConstructorStuff && highestStep < code.Length)
                //StartCoroutine("constructionTimer");
            if(step < stepEvent.Length) 
            {
                stepEvent[step].Invoke();
            }
        }
    }

    private bool isEven(int n) => n % 2 == 0;

    private IEnumerator constructionTimer()
    {
        AudioSource source = Controller.c.constructionSource;
        yield return new WaitForSeconds(Random.Range(1.5f, 2.2f));

        int n = 1;
        if (step < n)
            yield break;

        if (!source.isPlaying)
        {
            source.Play();
        }
        else
        {
            source.volume += 1f / (steps - n);
            Debug.Log("Volume: " + source.volume);
        }
    }

    private int[] GenerateCode(int length)
    {
        int[] result = new int[length];
        for (int i = 0; i < length; i++) 
        { 
            if(i == 0)
                result[i] = Random.Range(0, maxNumber -3);
            else 
                result[i] = Random.Range(0, maxNumber);
        }
            

        return result;
    }
}

[Flags]
public enum AllowedInput
{
    Left = 1 << 0,
    Right = 1 << 1
}