using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public float infectionValue = 0f;
    public float diabetesValue = 50f;

    public float diabetesDamage = 1f;
    public float diabetesHeal = .5f;
    public float diabetesMin = 30f;
    public float diabetesMax = 85f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        diabetesValue -= Time.deltaTime;

        if ((diabetesValue < diabetesMin) && (diabetesValue > diabetesMax)) {
            infectionValue += Time.deltaTime*diabetesDamage;
        } else
        {
            infectionValue += Time.deltaTime * diabetesHeal;
        }

        diabetesValue = Mathf.Clamp(diabetesValue, 0f, 100f);
        infectionValue = Mathf.Clamp(infectionValue, 0f, 100f);
    }
}
