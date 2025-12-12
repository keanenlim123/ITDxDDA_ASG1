using UnityEngine;
using System.Collections.Generic;

public class AnimalBehaviour : MonoBehaviour
{
    [Header("Animal Info")]
    public string animalName;

    [TextArea]
    public string description;

    public string habitatName;

    public Texture animalImage;

    [Header("Quiz Data")]
    public List<string> questions;                 // Size = 3
    public List<AnswerSet> answers;                 // Each array size = 3
    public List<int> correctAnswerIndexes;           // 0, 1, or 2
}

[System.Serializable]
public class AnswerSet
{
    public List<string> answers;
}
