using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackManager : MonoBehaviour
{

    private GameObject[] stack;
    public GameObject panel;
    int stackLength;
    int stackIndex;
    int count = 1;
    bool stackGet = false;
    const float maxLength = 6f;
    const float speedValue = 0.19f;
    const float size = 4f;
    Vector2 stackSize = new Vector2(size, size);
    float speed = speedValue;
    bool moveAxisX;
    float precision;
    Vector3 cameraPosition;
    Vector3 oldStackPosition;
    bool isDead = false;
    float tolerance = 0.4f;
    int combo = 0;
    Color32 color;
    public Color32 colorOne;
    public Color32 colorTwo;
    public Color32 colorThree;
    public Color32 colorFour;
    public Text text;
    public Text highScoreText;
    int score = 0;
    int highScore;
    int counter = 0;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt("highscore");
        highScoreText.text = highScore.ToString();
        text.text = score.ToString();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camera.backgroundColor = colorTwo;
        color = colorOne;
        stackLength = transform.childCount; //stack uzunluğu
        stack = new GameObject[stackLength];
        for (int i = 0; i < stackLength; i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
            stack[i].GetComponent<Renderer>().material.color = color;
        }
        stackIndex = stackLength - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0)) //fare sol tıklama
                {
                    Game();
                }
                MoveStack();
                transform.position = Vector3.Lerp(transform.position, cameraPosition, 0.1f); //kamera pozisyonu düzenleme
            }
            
            else if(Application.platform == RuntimePlatform.Android)
            {
                if(Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began)
                {
                    Game();
                }
                MoveStack();
                transform.position = Vector3.Lerp(transform.position, cameraPosition, 0.1f); //kamera pozisyonu düzenleme
            }
        }
    }
    public void Game()
    {
        if (StackControl())
        {
            ChangeStack();
            count++;
            score++;
            text.text = score.ToString();
            if (score > highScore)
            {
                highScore = score;
            }
            color = new Color32((byte)(color.r), (byte)(color.g + 30), (byte)(color.b), color.a);
            colorTwo = new Color32((byte)(colorTwo.r), (byte)(colorTwo.g + 10), (byte)(colorTwo.b), colorTwo.a);
            if (counter > 5)
            {
                counter = 0;
                colorOne = colorTwo;
                colorTwo = colorThree;
                colorThree = colorFour;
                colorFour = color;
                color = colorOne;
            }
            counter++;
        }
        else
        {
            EndGame();
        }

    }
    void ChangeStack()
    {
        oldStackPosition = stack[stackIndex].transform.localPosition;
        //en alttaki stack üste taşıma
        if (stackIndex <= 0)
        {
            stackIndex = stackLength;
        }
        stackGet = false;
        stackIndex--;
        moveAxisX = !moveAxisX;
        cameraPosition = new Vector3(0, -count, 0);
        stack[stackIndex].transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
        stack[stackIndex].GetComponent<Renderer>().material.color = Color32.Lerp(stack[stackIndex].GetComponent<Renderer>().material.color,color,0.5f);
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camera.backgroundColor = Color32.Lerp(camera.backgroundColor, colorTwo, 0.2f);
    }

    void MoveStack()
    {
        if (moveAxisX)
        {
            if (!stackGet)
            {
                stack[stackIndex].transform.localPosition = new Vector3(maxLength, count, precision);
                stackGet = true;
            }

            if (stack[stackIndex].transform.localPosition.x > maxLength)
            {
                speed = speedValue * -1;
            }

            else if (stack[stackIndex].transform.localPosition.x < -maxLength)
            {
                speed = speedValue;
            }

            stack[stackIndex].transform.localPosition += new Vector3(speed, 0, 0);
        }

        else
        {
            if (!stackGet)
            {
                stack[stackIndex].transform.localPosition = new Vector3(precision, count, maxLength);
                stackGet = true;
            }

            if (stack[stackIndex].transform.localPosition.z > maxLength)
            {
                speed = speedValue * -1;
            }

            else if (stack[stackIndex].transform.localPosition.z < -maxLength)
            {
                speed = speedValue;
            }

            stack[stackIndex].transform.localPosition += new Vector3(0, 0, speed);
        }

    }

    bool StackControl()
    {
        if (moveAxisX)
        {
            float diff = oldStackPosition.x - stack[stackIndex].transform.localPosition.x;
            if (Mathf.Abs(diff) > tolerance)
            {
                combo = 0;
                Vector3 position;
                if (stack[stackIndex].transform.localPosition.x > oldStackPosition.x)
                {
                    position = new Vector3(stack[stackIndex].transform.position.x + stack[stackIndex].transform.localScale.x / 2, stack[stackIndex].transform.position.y, stack[stackIndex].transform.position.z);
                }
                else
                {
                    position = new Vector3(stack[stackIndex].transform.position.x - stack[stackIndex].transform.localScale.x / 2, stack[stackIndex].transform.position.y, stack[stackIndex].transform.position.z);
                }
                Vector3 scale = new Vector3(diff, 1, stackSize.y);
                stackSize.x -= Mathf.Abs(diff);
                if (stackSize.x < 0)
                {
                    return false;
                }
                stack[stackIndex].transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
                float mid = (stack[stackIndex].transform.localPosition.x / 2) + (oldStackPosition.x / 2);
                stack[stackIndex].transform.localPosition = new Vector3(mid, count, oldStackPosition.z);
                precision = stack[stackIndex].transform.localPosition.x;
                CutPart(position, scale, stack[stackIndex].GetComponent<Renderer>().material.color);
            }


            else
            {
                combo++;
                if (combo > 3 )
                {
                    stackSize.x += 0.3f;
                    if (stackSize.x > size)
                    {
                        stackSize.x = size;
                    }
                    stack[stackIndex].transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
                    stack[stackIndex].transform.localPosition = new Vector3(oldStackPosition.x, count, oldStackPosition.z);
                }
                else
                {
                    stack[stackIndex].transform.localPosition = new Vector3(oldStackPosition.x, count, oldStackPosition.z);
                }
                precision = stack[stackIndex].transform.localPosition.x;
            }

        }
        else
        {
            float diff = oldStackPosition.z - stack[stackIndex].transform.localPosition.z;
            if (Mathf.Abs(diff) > tolerance)
            {
                combo = 0;
                Vector3 position;
                if (stack[stackIndex].transform.localPosition.z > oldStackPosition.z)
                {
                    position = new Vector3(stack[stackIndex].transform.position.x, stack[stackIndex].transform.position.y, stack[stackIndex].transform.position.z + +stack[stackIndex].transform.localScale.z / 2);
                }
                else
                {
                    position = new Vector3(stack[stackIndex].transform.position.x, stack[stackIndex].transform.position.y, stack[stackIndex].transform.position.z - stack[stackIndex].transform.localScale.z / 2);
                }
                Vector3 scale = new Vector3(stackSize.x, 1, diff);
                stackSize.y -= Mathf.Abs(diff);
                if (stackSize.y < 0)
                {
                    return false;
                }
                stack[stackIndex].transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
                float mid = (stack[stackIndex].transform.localPosition.z / 2) + (oldStackPosition.z / 2);
                stack[stackIndex].transform.localPosition = new Vector3(oldStackPosition.x, count, mid);
                precision = stack[stackIndex].transform.localPosition.z;
                CutPart(position, scale, stack[stackIndex].GetComponent<Renderer>().material.color);
                combo++;
            }
            else
            {
                combo++;
                if (combo > 3)
                {
                    stackSize.y += 0.3f;
                    if (stackSize.y > size)
                    {
                        stackSize.y = size;
                    }
                    stack[stackIndex].transform.localScale = new Vector3(stackSize.x, 1, stackSize.y);
                    stack[stackIndex].transform.localPosition = new Vector3(oldStackPosition.x, count, oldStackPosition.z);
                }
                else
                {
                    stack[stackIndex].transform.localPosition = new Vector3(oldStackPosition.x, count, oldStackPosition.z);
                }
                precision = stack[stackIndex].transform.localPosition.z;
            }
        }
        return true;
    }

    void CutPart(Vector3 position, Vector3 scale,Color32 partcolor)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = scale;
        go.transform.position = position;
        go.GetComponent<Renderer>().material.color = partcolor;
        go.AddComponent<Rigidbody>();
    }


    void EndGame()
    {
        isDead = true;
        stack[stackIndex].AddComponent<Rigidbody>();
        panel.SetActive(true);  
        PlayerPrefs.SetInt("highscore", highScore);
        highScoreText.text = highScore.ToString();
        text.text = "";
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
